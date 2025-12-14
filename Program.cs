string[] tests = {
    "12 / 3.7 + 2 * 0.5",
    "log 2.7",
    "2 ^ 8",
    "log 10 * 2 - sin 20 - 7",
    "3/ ",
    "log 10 * 2 - sin 20 - 7 log",
    "log * 10 * 2 - sin 20 - 7 ",
};

Console.WriteLine("---------------------");
foreach (string test in tests) {
    Console.WriteLine(test);
    try {
        Expr expr = Syn.parseExpr(test);
        Console.WriteLine("= " + expr.eval());
        Console.WriteLine(expr.ToString());
    } catch (Exception e) {
        Console.WriteLine("= Syntax error");
        // Console.WriteLine(e.ToString());
    }
    Console.WriteLine("---------------------");
}