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
            run(t, input);
            input = Console.ReadLine();
        }
        Console.WriteLine("bye!");
    }

    public static void runTests() {
        // string[] tests = {
        //     // "x[1025] ; i := 0; while i < 1024 { x[i] := 0; print i; print x[i]; i := i+1 };",
        //     "x := 24; x",
        // };
        Console.WriteLine("---------------------");
        foreach (string test in tests) {
            Interpreter t = new Interpreter(true);
            Console.WriteLine(test);
            run(t, test);
            Console.WriteLine("---------------------");
        }
    }

    private static void run(Interpreter t, string code) {
        try {
            Value result = t.run(code);
            Console.WriteLine("= " + result);
        } catch (Lex.Error e) {
            for (int i = 0; i < e.position; i++) Console.Write(" ");
            Console.WriteLine("^ [Lex] " + e.message);
        } catch (Syn.Error e) {
            for (int i = 0; i < e.position; i++) Console.Write(" ");
            Console.WriteLine("^ [Syn] " + e.message);
        } catch (Exception e) {
            Console.WriteLine("[Err] " + e.Message);
        }
    }

    private static string[] tests = {
        // Espressions
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
        "2 > 2+2 ? 1 : 2",
        "neg 12 * neg1",
        "x",

        // Statements
        "x := 3; x times 10 plus 7;",
        "x := 3; x times 10 plus 7",
        ";x := 3;; x times 10 plus 7;;;",

        "x:=1; x > 4 ? x : 4",
        "x:=8; x > 4 ? x : 4",
        "x:=5; x > 4 ? 4 : x > 2 ? 2 : 0",
        "x:=3; x > 4 ? 4 : x > 2 ? 2 : 0",
        "x:=1; x > 4 ? 4 : x > 2 ? 2 : 0",
        "x:=5; x > 2 ? x > 4 ? 4 : 2 : 0",
        "x:=3; x > 2 ? x > 4 ? 4 : 2 : 0",
        "x:=1; x > 2 ? x > 4 ? 4 : 2 : 0",
        "x:=1; x > 0 and x < 2 ? 1 : 0",
        "x:=4; x > 0 and x < 2 ? 1 : 0",
        "x:=1; x < 0 or x > 2 ? 1 : 0",
        "x:=4; x < 0 or x > 2 ? 1 : 0",

        "x := 37; while x > 3 { print x; x := x - 10 }",
        "{ in := 2 }; 3",
        "outer := 1; { inner := 12; outer := outer + 1; print inner }; print outer",

        // Error messages
        "3/ ",
        "7 pl",
        "log 10 * 2 - sin 20 - 7 log",
        "log * 10 * 2 - sin 20 - 7 ",
        "(2 + 2) + 3 * ((2)",
        "2 * ((13 - 12) * log 2",
        "(13 - 12)) * log 2",
    };
}