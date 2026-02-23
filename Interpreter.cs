class Interpreter {
    private bool debug;
    private Scope globalScope;

    public Interpreter() {
        this.debug = false;
        this.globalScope = new Scope();
    }

    public Interpreter(bool debug) {
        this.debug = debug;
        this.globalScope = new Scope();
    }

    public Value run(String code) {
        int length = 0;
        Token[] tokens = Lex.tokenize(code, out length);
        if (debug) Console.WriteLine(string.Join(", ", tokens.Take(length)));

        Stmt s = Syn.parse(tokens, length);
        if (debug) Console.WriteLine(s);

        return s.exec(globalScope);
    }
}