using System;
using System.Collections.Generic;
using StencilORM.Parsers;
using StencilORM.Queries;
using Test;

namespace Test3
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            var expr = ExprParser.Instance.Parse("a + 'ola mundo \n \t \\'mundo\\' ola' * (6 + -9)");
            var expr2 = ExprParser.Instance.Parse("-a");
            var expr3 = ExprParser.Instance.Parse("-a + -(4 + 6)");
            var expr4 = ExprParser.Instance.Parse("6");
            var expr5 = ExprParser.Instance.Parse("6.0");
            var expr6 = ExprParser.Instance.Parse("a + $a");
            var expr7 = ExprParser.Instance.Parse("Articles.ArticleDescription == 'RTUY /(89)' ");
            var expr8 = ExprParser.Instance.Parse(" FF1(a+b, 89, (9+1) * 2)");
            var expr9 = ExprParser.Instance.Parse(" FF2(a+b )");
            var expr10 = ExprParser.Instance.Parse("  a+ b >= 3 && 1");
            var expr11 = ExprParser.Instance.Parse("  a+ b >= 3 && 1 ? (3 + 9) * 5:  a <= 3");
            var expr12 = ExprParser.Instance.Parse("-(  a+ b >= 3 && 1 ? (3 + 9) * 5:  a <= 3)");
            var expr13 = Expr.Parse("true");
            var expr14 = Expr.Parse("false");
            var expr15 = Expr.Parse("!true");
            Guid guid = Guid.NewGuid();
            var literal = (Literal)guid;
            var update = new Update<ExampleTable>(new ExampleTable { Key = guid });
            update.Execute(new Compiler(), out int n1);
            var update2 = new Update<ExampleTable>(new ExampleTable { Key = guid }, "Description");
            var select = new Query<ExampleTable>().InnerJoin(new Query("SomeTable"),
                                                             new string[] { "ForeignKey" },
                                                             new string[] { "SomeKey" });
            var select2 = new Query<ExampleTable>().LeftJoin(new Query("SomeTable"),
                                                             new string[] { "ForeignKey" },
                                                             new string[] { "SomeKey" });
            select2.Execute(new Compiler());

            var select3 = new Query<ExampleTable>().LeftJoin("SomeTable",
                                                            new string[] { "ForeignKey" },
                                                            new string[] { "SomeKey" });
            select3.Execute(new Compiler());
            var select4 = new Query<ExampleTable>()
            .Select("a", "b", "c")
            .Where(
                Expr.Eq("a", "ola")
                .And(Expr.Eq("b", 3))
                );
            select4.Execute(new Compiler());
            var insertOrUpdate = new Update<ExampleTable>(new ExampleTable { Key = guid }).InsertOrUpdate();
            insertOrUpdate.Execute(new Compiler(), out n1);
            
            var select5 = new Query<ExampleTable>().Where(Expr.NotNull("ForeignKey"));
            select5.Execute(new Compiler());
            
            var select6 = new Query<ExampleTable>().Where(Expr.NotIn("ForeignKey", new int[] { 8, 9, 10 }));
            select6.Execute(new Compiler());
            
            var select7 = new Query<ExampleTable>().Where(Expr.NotIn("ForeignKey", new List<string> { "8", "9", "10" }));
            select7.Execute(new Compiler());
            
            Console.WriteLine("Hello World!");
        }
    }
}
