using System;
using StencilORM.Parsers;

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
            var expr6 = ExprParser.Instance.Parse("a + ?a");
            var expr7 = ExprParser.Instance.Parse("Articles.ArticleDescription == 'RTUY /(89)' ");
            var expr8 = ExprParser.Instance.Parse(" FF1(a+b, 89, (9+1) * 2)");
            var expr9 = ExprParser.Instance.Parse(" FF2(a+b )");
            Console.WriteLine("Hello World!");
        }
    }
}
