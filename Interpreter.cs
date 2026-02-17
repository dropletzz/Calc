class Interpreter {
    public bool debug;

    public Interpreter() {
        this.debug = false;
    }

    public Interpreter(bool debug) {
        this.debug = debug;
    }

    public double run(String code) {
        int length = 0;
        Token[] tokens = Lex.tokenize(code, out length);
        if (debug) Console.WriteLine(string.Join(", ", tokens.Take(length)));

        Expr expr = Syn.parse(tokens, length);
        if (debug) Console.WriteLine(expr);

        return expr.eval();
    }
}