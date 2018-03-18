using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using StencilORM.Queries;
using StencilORM.Utils;
using StencilORM.Metadata;

namespace StencilORM.Compilers
{
    public abstract class SimpleSQLCompiler<T> : IQueryCompiler, IASTVisitor<T, StringBuilder>
    {

        protected abstract T NewState();
        public abstract void Process(T state, StringBuilder builder, Param param);
        protected abstract void Process(T state, StringBuilder builder, DataType dataType, decimal value);
        protected abstract void Process(T state, StringBuilder builder, DataType dataType, DateTimeOffset value);
        protected abstract void Process(T state, StringBuilder builder, DataType dataType, DateTime value);
        protected abstract void ProcessConcat(T state, StringBuilder builder, IExpr left, IExpr right);
        protected abstract void ProcessBeforeSelectedColumns(T state, StringBuilder builder, Query query);
        protected abstract void ProcessAfterQueryEnd(T state, StringBuilder builder, Query query);

        public abstract IEnumerable<R> Execute<R>(CompiledQuery<T> query, params Value[] parameters);
        public abstract object Execute(Type type, CompiledQuery<T> query, params Value[] parameters);
        public abstract IEnumerable<string[]> Execute(CompiledQuery<T> query, params Value[] parameters);
        public abstract bool Execute(CompiledQuery<T> query, out int rowsAltered, params Value[] parameters);
        public abstract IPreparedStatement Prepare(CompiledQuery<T> query);

        public abstract bool CreateOrAlter<R>();

        public abstract void EscapeName(StringBuilder builder, string name, bool isTableName);

        public IEnumerable<R> Execute<R>(Query query, params Value[] parameters)
        {
            return Execute<R>(CompileQuery(query), parameters);
        }

        public object Execute(Type type, Query query, params Value[] parameters)
        {
            return Execute(type, CompileQuery(query), parameters);
        }

        public IEnumerable<string[]> Execute(Query query, params Value[] parameters)
        {
            return Execute(CompileQuery(query), parameters);
        }

        public bool Execute(Insert query, out int rowsAltered, params Value[] parameters)
        {
            return Execute(CompileInsert(query), out rowsAltered, parameters);
        }

        public bool Execute(Update query, out int rowsAltered, params Value[] parameters)
        {
            if (!query.CreateNewIfNone)
                return Execute(CompileUpdate(query), out rowsAltered, parameters);
            bool exists = Execute<bool>(CompileExistsRecord(query), parameters).FirstOrDefault();
            if (exists)
                return Execute(CompileUpdate(query), out rowsAltered, parameters);
            return Execute(CompileInsertFromUpdate(query), out rowsAltered, parameters);
        }

        public bool Execute(Delete query, out int rowsAltered, params Value[] parameters)
        {
            return Execute(CompileDelete(query), out rowsAltered, parameters);
        }

        public IPreparedStatement Prepare(Query query)
        {
            return Prepare(CompileQuery(query));
        }

        public IPreparedStatement Prepare(Insert query)
        {
            return Prepare(CompileInsert(query));
        }

        public IPreparedStatement Prepare(Update query)
        {
            if (!query.CreateNewIfNone)
                return Prepare(CompileUpdate(query));
            return new PreparedInsertOrUpdate
            {
                Exists = Prepare(CompileExistsRecord(query)),
                Update = Prepare(CompileUpdate(query)),
                Insert = Prepare(CompileInsertFromUpdate(query))
            };
        }

        public IPreparedStatement Prepare(Delete query)
        {
            return Prepare(CompileDelete(query));
        }

        public CompiledQuery<T> CompileQuery(Query query)
        {
            T state = NewState();
            StringBuilder builder = new StringBuilder();
            Process(state, builder, query);
            return new CompiledQuery<T>(builder.ToString(), state);
        }

        public CompiledQuery<T> CompileInsert(Insert insert)
        {
            T state = NewState();
            StringBuilder builder = new StringBuilder();
            Process(state, builder, insert);
            return new CompiledQuery<T>(builder.ToString(), state);
        }

        public CompiledQuery<T> CompileUpdate(Update update)
        {
            T state = NewState();
            StringBuilder builder = new StringBuilder();
            Process(state, builder, update);
            return new CompiledQuery<T>(builder.ToString(), state);
        }

        public CompiledQuery<T> CompileInsertFromUpdate(Update update)
        {
            T state = NewState();
            StringBuilder builder = new StringBuilder();
            ProcessInsertFromUpdate(state, builder, update);
            return new CompiledQuery<T>(builder.ToString(), state);
        }

        public CompiledQuery<T> CompileExistsRecord(Update update)
        {
            T state = NewState();
            StringBuilder builder = new StringBuilder();
            ProcessExistsRecordWhere(state, builder, update.TableName, update.WhereExpr);
            return new CompiledQuery<T>(builder.ToString(), state);
        }

        public CompiledQuery<T> CompileDelete(Delete delete)
        {
            T state = NewState();
            StringBuilder builder = new StringBuilder();
            Process(state, builder, delete);
            return new CompiledQuery<T>(builder.ToString(), state);
        }

        public void Process(T state, StringBuilder builder, Update update)
        {
            builder.Append("UPDATE ");
            EscapeName(builder, update.TableName, true);
            ProcessSets(state, builder, update.Sets);
            ProcessWhere(state, builder, update.WhereExpr);
        }

        public void ProcessExistsRecordWhere(T state, StringBuilder builder, string tableName, IExpr expr)
        {
            builder.Append("SELECT EXISTS (SELECT * FROM ");
            EscapeName(builder, tableName, true);
            builder.Append(" WHERE ");
            expr.Visit(state, builder, this);
            builder.Append(")");
        }

        public void ProcessInsertFromUpdate(T state, StringBuilder builder, Update update)
        {
            IDictionary<string, Set> whereSets = GetSetsFromWhere(update.WhereExpr);
            Insert insert = new Insert(update.TableName);
            foreach (var item in whereSets)
            {
                insert.Set(item.Value);
            }
            foreach (var item in update.Sets)
            {
                if (whereSets.ContainsKey(item.Name))
                    continue;
                insert.Set(item);
            }
            Process(state, builder, insert);
        }

        private IDictionary<string, Set> GetSetsFromWhere(IExpr where)
        {
            IDictionary<string, Set> sets = new Dictionary<string, Set>();
            if (where.Type == ExprType.EMPTY)
                return sets;
            GetSetsFromWhere(sets, where);
            return sets;
        }

        private void GetSetsFromWhere(IDictionary<string, Set> sets, IExpr where)
        {
            if (where.Type != ExprType.EXPR)
                throw new Exception("Invalid Insert or Update Where Expression");
            GetSetsFromWhere(sets, (Expr)where);
        }

        private void GetSetsFromWhere(IDictionary<string, Set> sets, Expr where)
        {
            switch (where.Operation)
            {
                case Operation.ADD:
                    GetSetsFromWhere(sets, where.Left);
                    GetSetsFromWhere(sets, where.Right);
                    break;
                case Operation.EQUALS:
                    GetSetFromEquals(sets, where.Left, where.Right);
                    break;
                default:
                    throw new Exception("Invalid Insert Or Update Operation: only AND and EQUALS allowed");
            }
        }

        private void GetSetFromEquals(IDictionary<string, Set> sets, IExpr left, IExpr right)
        {
            Variable? variable = null;
            IExpr value = null;
            if (left.Type == ExprType.VARIABLE)
            {
                variable = (Variable)left;
                value = right;
            }
            else if (right.Type == ExprType.VARIABLE)
            {
                variable = (Variable)right;
                value = left;
            }
            if (variable == null)
                throw new Exception("Insert Or Update: missing column name");
            if (value == null)
                throw new Exception("Insert Or Update: missing column value");
            if (variable.Value.Scope != null)
                throw new Exception("Insert Or Update: column name cannot be scoped");
            sets.Add(variable.Value.Name, new Set(variable.Value.Name, value));
        }

        public void Process(T state, StringBuilder builder, Insert insert)
        {
            builder.Append("INSERT INTO ");
            EscapeName(builder, insert.TableName, true);
            ProcessSetColumns(state, builder, insert.Sets);
            builder.Append(" VALUES ");
            ProcessSetValues(state, builder, insert.Sets);
        }

        public void Process(T state, StringBuilder builder, Delete delete)
        {
            builder.Append("DELETE FROM");
            EscapeName(builder, delete.TableName, true);
            ProcessWhere(state, builder, delete.WhereExpr);
        }

        public string Process(T state, StringBuilder builder, Query query)
        {
            builder.Append("SELECT ");
            ProcessBeforeSelectedColumns(state, builder, query);
            if (query.Columns.Any())
                builder.AppendJoin(", ", query.Columns, (sb, x) => Process(state, sb, x));
            else
                builder.Append(" * ");
            builder.Append(" FROM ");
            var name = Process(state, builder, query.TableSource, query.Alias);
            query.Joins.ListForEach(x => Process(state, builder, x));
            ProcessWhere(state, builder, query.WhereExpr);
            if (query.GroupByColumns.Any())
            {
                builder.Append(" GROUP BY ");
                builder.AppendJoin(", ", query.GroupByColumns, (sb, x) => Process(state, sb, x));
            }
            if (query.GroupWhereExpr.Type != ExprType.EMPTY)
            {
                builder.Append(" HAVING ");
                query.GroupWhereExpr.Visit(state, builder, this);
            }
            if (query.OrderByExprs.Any())
            {
                builder.Append(" ORDER BY ");
                builder.AppendJoin(", ", query.GroupByColumns, (sb, x) => Process(state, sb, x));
            }
            ProcessAfterQueryEnd(state, builder, query);
            return name;
        }

        private void ProcessSets(T state, StringBuilder builder, List<Set> sets)
        {
            builder.Append(" SET ")
                   .AppendJoin(", ", sets, (sb, x) => Process(state, sb, x));
        }

        private void Process(T state, StringBuilder builder, Set set)
        {
            EscapeName(builder, set.Name, false);
            builder.Append(" = ");
            set.Value.Visit(state, builder, this);
        }

        private void ProcessSetValues(T state, StringBuilder builder, List<Set> sets)
        {
            builder.Append("(")
                   .AppendJoin(", ", sets, (sb, x) => x.Value.Visit(state, builder, this))
                   .Append(")");
        }

        private void ProcessSetColumns(T state, StringBuilder builder, List<Set> sets)
        {
            builder.Append("(")
                   .AppendJoin(", ", sets, (sb, x) => EscapeName(sb, x.Name, false))
                   .Append(")");
        }

        private void ProcessWhere(T state, StringBuilder builder, IAppendableExpr where)
        {
            if (where.Type != ExprType.EMPTY)
            {
                builder.Append(" WHERE ");
                where.Visit(state, builder, this);
            }
        }

        private string Process(T state, StringBuilder builder, QuerySource query, string alias)
        {
            string name;
            if (query.TableName != null)
            {
                EscapeName(builder, query.TableName, true);
                name = alias ?? query.TableName;
            }
            else
            {
                builder.Append("(");
                var subName = Process(state, builder, query.TableQuery);
                builder.Append(")");
                alias = alias ?? subName;
                name = alias;
            }

            if (!StencilUtils.IsNullOrWhiteSpace(alias))
            {
                builder.Append(" AS ");
                EscapeName(builder, alias, false);
            }
            return name;
        }

        public void Process(T state, StringBuilder builder, Column column)
        {
            column.Value.Visit(state, builder, this);
            if (!StencilUtils.IsNullOrWhiteSpace(column.Alias))
            {
                builder.Append(" AS ");
                EscapeName(builder, column.Alias, false);
            }
        }

        public void Process(T state, StringBuilder builder, Join join)
        {
            if (join.LeftJoin)
                builder.Append(" LEFT ");
            else
                builder.Append(" INNER ");
            builder.Append("JOIN ");
            Process(state, builder, join.Inner, join.Alias);
            builder.Append(" ON ");
            join.On.Visit(state, builder, this);
        }

        public void Process(T state, StringBuilder builder, Expr expr)
        {
            if (expr.Operation == Operation.CONCAT)
            {
                ProcessConcat(state, builder, expr.Left, expr.Right);
                return;
            }
            bool isBinary = expr.IsBinaryOperation();
            if (isBinary)
            {
                expr.Left.Visit(state, builder, this);
                builder.Append(' ');
            }
            switch (expr.Operation)
            {
                case Operation.ADD:
                    builder.Append('+');
                    break;
                case Operation.SUB:
                    builder.Append('-');
                    break;
                case Operation.MUL:
                    builder.Append('*');
                    break;
                case Operation.DIV:
                    builder.Append('/');
                    break;
                case Operation.MOD:
                    builder.Append('%');
                    break;
                /*case Operation.MINUS:
                    builder.Append('-');
                    break;*/
                case Operation.GT:
                    builder.Append('>');
                    break;
                case Operation.LT:
                    builder.Append('<');
                    break;
                case Operation.GTE:
                    builder.Append(">=");
                    break;
                case Operation.LTE:
                    builder.Append("<=");
                    break;
                case Operation.EQUALS:
                    builder.Append('=');
                    break;
                case Operation.NOTEQUALS:
                    builder.Append("<>");
                    break;
                case Operation.LIKE:
                    builder.Append("like");
                    break;
                case Operation.IN:
                    builder.Append("in");
                    break;
                case Operation.NOTIN:
                    builder.Append("not in");
                    break;
                case Operation.ISNULL:
                    builder.Append("is null");
                    break;
                case Operation.NOTNULL:
                    builder.Append("is not null");
                    break;
                case Operation.AND:
                    builder.Append("AND");
                    break;
                case Operation.OR:
                    builder.Append("OR");
                    break;
                case Operation.NOT:
                    builder.Append("NOT");
                    break;
                case Operation.EXISTS:
                    builder.Append("EXISTS");
                    break;
            }
            builder.Append(' ');
            if (isBinary)
                expr.Right.Visit(state, builder, this);
            else
                expr.Left.Visit(state, builder, this);
        }

        public void Process(T state, StringBuilder builder, Variable variable)
        {
            if (variable.Scope != null)
            {
                EscapeName(builder, variable.Scope, false);
                builder.Append(".");
            }
            EscapeName(builder, variable.Name, false);
        }

        public void Process(T state, StringBuilder builder, If @if)
        {
            builder.Append("(");
            builder.Append("CASE WHEN (");
            @if.Condition.Visit(state, builder, this);
            builder.Append(") THEN (");
            @if.ThenExpr.Visit(state, builder, this);
            builder.Append(")");
            if (@if.ElseExpr != null)
            {
                builder.Append(" ELSE (");
                @if.ElseExpr.Visit(state, builder, this);
                builder.Append(")");
            }
            builder.Append("END )");
        }

        public void Process(T state, StringBuilder builder, Empty empty)
        {
        }

        public void Process(T state, StringBuilder builder, Literal literal)
        {
            var value = literal.Value;
            var dataType = literal.DataType;
            ProcessLiteral(state, builder, dataType, value);
        }

        private void ProcessLiteral(T state, StringBuilder builder, DataType dataType, object value)
        {
            if (value == null)
            {
                builder.Append("null");
                return;
            }
            if (value.Is<decimal>() || value.Is<double>() || value.Is<float>())
                Process(state, builder, dataType, (decimal)value);
            else if (value is int || value is long || value is uint || value is ulong)
                builder.Append(value.ToString());
            else if (value.Is<DateTime>())
                Process(state, builder, dataType, (DateTime)value);
            else if (value.Is<DateTimeOffset>())
                Process(state, builder, dataType, (DateTimeOffset)value);
            else if (value is IEnumerable<object>)
            {
                var enumerable = ((IEnumerable<object>)value);
                builder
                .Append("(")
                .AppendJoin(", ", enumerable, (sb, x) => ProcessLiteral(state, sb, DataType.UNKNOWN, x))
                .Append(")");
            }
            else if (value is Query)
            {
                builder.Append("(");
                Process(state, builder, (Query)value);
                builder.Append(")");
            }
            else
                builder.AppendFormat("'{0}'", value);
        }

        public void Process(T state, StringBuilder builder, Function function)
        {
            builder.Append(function.Name);
            if (function.Args == null)
                return;
            foreach (var item in function.Args)
            {
                builder.Append(" ");
                item.Visit(state, builder, this);
            }
        }
    }
}
