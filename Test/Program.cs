using System;
using StencilORM.Parsers;
using StencilORM.Query;

namespace Test
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
            var update2 = new Update<ExampleTable>(new ExampleTable { Key = guid }, "Description");
            var select = new Query<ExampleTable>().InnerJoin(new Query("SomeTable"),
                                                             new string[] { "ForeignKey" },
                                                             new string[] { "SomeKey" });
            var select2 = new Query<ExampleTable>().LeftJoin(new Query("SomeTable"),
                                                             new string[] { "ForeignKey" },
                                                             new string[] { "SomeKey" });
            Console.WriteLine("Hello World!");
        }
    }
}
