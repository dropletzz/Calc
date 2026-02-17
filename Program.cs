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
        Interpreter t = new Interpreter();
        while (input != null && !input.Equals("bye")) {
            try {
                double result = t.run(input);
                Console.WriteLine("= " + result);
            } catch (Syn.Error e) {
                for (int i = 0; i < e.position; i++) Console.Write(" ");
                Console.WriteLine("^ [Syn] " + e.message);
            } catch (Lex.Error e) {
                for (int i = 0; i < e.position; i++) Console.Write(" ");
                Console.WriteLine("^ [Lex] " + e.message);
            }
            input = Console.ReadLine();
        }
        Console.WriteLine("bye!");
    }

    public static void runTests() {
        string[] tests = {
            "2 + 2",
            "12 / 37 + 2 * 5",
            "10 - 7 - 3",
            "44/2/2",
            "(37 - 4) * 3",
            "12 / (37 + 2) * 5",
            "log(13 - 12)",
            "log sin 1.57",
            "log 10 * 2 - sin 20 - 7",
            "3 plus 7 by 10",

            // Tests for error messages
            "3/ ",
            "7 pl",
            "log 10 * 2 - sin 20 - 7 log",
            "log * 10 * 2 - sin 20 - 7 ",
            "(2 + 2) + 3 * ((2)",
            "2 * ((13 - 12) * log 2",
            "(13 - 12)) * log 2",
        };

        Console.WriteLine("---------------------");
        foreach (string test in tests) {
            Interpreter t = new Interpreter(true);
            Console.WriteLine(test);
            try {
                double result = t.run(test);
                Console.WriteLine("= " + result);
            } catch (Lex.Error e) {
                for (int i = 0; i < e.position; i++) Console.Write(" ");
                Console.WriteLine("^ [Lex] " + e.message);
            } catch (Syn.Error e) {
                for (int i = 0; i < e.position; i++) Console.Write(" ");
                Console.WriteLine("^ [Syn] " + e.message);
            }
            Console.WriteLine("---------------------");
        }
    }
}