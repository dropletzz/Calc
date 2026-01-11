public static class Program {
    public static void Main(string[] args) {
        bool testsCommand = args.Length == 1 && args[0].Equals("tests");

        if (testsCommand) runTests();
        else runRepl();
    }

    public static void runRepl() {
        Console.WriteLine(
            "Welcome to Calc (type 'bye' to exit)\n" +
            "Type an expression like '2 + 2' to get the result"
        );
        string? input = Console.ReadLine();
        while (input != null && !input.Equals("bye")) {
            try {
                Expr expr = Syn.parseExpr(input);
                Console.WriteLine("= " + expr.eval());
            } catch (Syn.Error e) {
                Console.WriteLine("= Syntax error: " + e.message);
            }
            input = Console.ReadLine();
        }
        Console.WriteLine("bye!");
    }

    public static void runTests() {
        string[] tests = {
            "12 / 3.7 + 2 * 0.5",
            "log 2.7",
            "2 ^ 8",
            "log 10 * 2 - sin 20 - 7",
            "3/ ",
            "log 10 * 2 - sin 20 - 7 log",
            "log * 10 * 2 - sin 20 - 7 ",
            "3 plus 7 by 10",
        };

        Console.WriteLine("---------------------");
        foreach (string test in tests) {
            Console.WriteLine(test);
            try {
                Expr expr = Syn.parseExpr(test);
                Console.WriteLine("= " + expr.eval());
                Console.WriteLine(expr.ToString());
            } catch (Syn.Error e) {
                Console.WriteLine("= Syntax error: " + e.message);
            }
            Console.WriteLine("---------------------");
        }
    }
}