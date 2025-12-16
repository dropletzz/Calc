public static class Program {
    public static void Main(string[] args) {
        bool testsCommand = args.Length == 1 && args[0].Equals("tests");

        if (testsCommand) runTests();
        else runRepl();
    }

    public static void runRepl() {
        String input = Console.ReadLine();
        while (input != null && !input.Equals("bye")) {
            try {
                int length;
                Token[] tokens = Lex.tokenize(input, out length);
                Expr x = Syn.parse(tokens, length);
                Console.WriteLine("= " + x.eval());
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
            "(37 - 4) * 3",
            "12 / (37 + 2) * 5",
            "log(13 - 12)",
            "log sin 1.57",
            "log 10 * 2 - sin 20 - 7",
            "3 plus 7 by 10",

            // Tests for error messages
            "3/ ",
            "log 10 * 2 - sin 20 - 7 log",
            "log * 10 * 2 - sin 20 - 7 ",
            "(2 + 2) + 3 * ((2)",
            "2 * ((13 - 12) * log 2",
            "(13 - 12)) * log 2",
        };

        Console.WriteLine("---------------------");
        foreach (string test in tests) {
            Console.WriteLine(test);
            try {
                int length = 0;
                Token[] tokens = Lex.tokenize(test, out length);
                Expr x = Syn.parse(tokens, length);
                Console.WriteLine("= " + x.eval());

                // print tokens
                Console.Write("tokens: ");
                for (int i = 0; i < length - 1; i++)
                    Console.Write(tokens[i] + ", ");
                Console.WriteLine(tokens[length - 1]);

                // print expr
                Console.WriteLine("expr: " + x);

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