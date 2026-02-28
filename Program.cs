public static class Program {
    public static void Main(string[] args) {
        if (args.Length == 0) runRepl();
        else if (args[0].Equals("tests")) runTests();
        else runFile(args[0]);
    }

    public static void runRepl() {
        Console.WriteLine(
            "Welcome to Calc (type 'bye' to exit)\n" +
            "Type an expression like '2 + 2' to get the result"
        );
        string? input = Console.ReadLine();
        Interpreter t = new Interpreter();
        while (input != null && !input.Equals("bye")) {
            if (!String.IsNullOrEmpty(input)) run(t, input);
            input = Console.ReadLine();
        }
        Console.WriteLine("bye!");
    }

    public static void runTests() {
        // string[] tests = {
        //     "a[2^10]",
        //     "a = [1,2,3,4]",
        //     "if 1>2 {23} else {333}",
        //     "x=23; if x>24 {23} else if x > 12 {333}",
        //     "x=23; if x>24 {23} else if x > 23.5 {neg12} else if x < 0 {333}",
        // };
        Console.WriteLine("---------------------");
        foreach (string test in tests) {
            Interpreter t = new Interpreter(true);
            Console.WriteLine(test);
            run(t, test);
            Console.WriteLine("---------------------");
        }
    }

    public static void runFile(string fileName) {
        string code = File.ReadAllText(fileName);
        Interpreter t = new Interpreter();
        run(t, code, false);
    }

    private static void run(Interpreter t, string code, bool showResult) {
        string[] lines = code.Split('\n', StringSplitOptions.None);
        try {
            Value result = t.run(code);
            if (showResult) Console.WriteLine("= " + result);
        } catch (Lex.Error e) {
            Console.WriteLine(lines[e.loc.line]);
            for (int i = 0; i < e.loc.position; i++) Console.Write(" ");
            Console.WriteLine("^ [Lex] " + e.message);
        } catch (Syn.Error e) {
            Console.WriteLine(lines[e.loc.line]);
            for (int i = 0; i < e.loc.position; i++) Console.Write(" ");
            Console.WriteLine("^ [Syn] " + e.message);
        } catch (Exception e) {
            Console.WriteLine("[Err] " + e.Message);
        }
    }

    private static void run(Interpreter t, string code) {
        run(t, code, true);
    }

    private static string[] tests = {
        // Expressions
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

        // Statements
        "x = 3; x times 10 plus 7;",
        "x = 3; x times 10 plus 7",
        ";x = 3;; x times 10 plus 7;;;",

        "x=1; x > 4 ? x : 4",
        "x=8; x > 4 ? x : 4",
        "x=5; x > 4 ? 4 : x > 2 ? 2 : 0",
        "x=3; x > 4 ? 4 : x > 2 ? 2 : 0",
        "x=1; x > 4 ? 4 : x > 2 ? 2 : 0",
        "x=5; x > 2 ? x > 4 ? 4 : 2 : 0",
        "x=3; x > 2 ? x > 4 ? 4 : 2 : 0",
        "x=1; x > 2 ? x > 4 ? 4 : 2 : 0",
        "x=1; x > 0 and x < 2 ? 1 : 0",
        "x=4; x > 0 and x < 2 ? 1 : 0",
        "x=1; x < 0 or x > 2 ? 1 : 0",
        "x=4; x < 0 or x > 2 ? 1 : 0",

        "if 1>2 {23} ",
        "if 1<2 {23}",
        "if 1>2 {23} else {333}",
        "x=23; if x>24 {23} else if x > 12 {333}",
        "x=23; if x>24 {23} else if x > 23.5 {neg12} else if x < 0 {333}",

        "{ in = 2; in }",
        "x = 37; while x > 3 { print x; x = x - 10 }",
        "{ in = 2 }; 3",
        "outer = 1; { inner = 12; outer = outer + 1; print inner }; print outer",
        "x[6]; i = 0; while (i < 6) { [i]x = i*i; i = i+1 }; i = 0; while (i < 6) { print [i]x; i = i+1 }",

        "x[12]; [4]x=37",
        "x[12]; [4]x=37; [4]x",
        "x[12]; [4]x=37;[2+2]x",

        // Error messages
        "3/ ",
        "7 pl",
        "log 10 * 2 - sin 20 - 7 log",
        "log * 10 * 2 - sin 20 - 7 ",
        "(2 + 2) + 3 * ((2)",
        "2 * ((13 - 12) * log 2",
        "(13 - 12)) * log 2",
        "x",
        "x[12]; [4]x=37;[2*12]x",
    };
}