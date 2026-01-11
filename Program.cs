// Recursive function that takes a string and returns an Expr
Expr parseExpr(string s) {
    s = s.Trim();

    double value;
    bool isLiteral = Double.TryParse(s, out value);
    if (isLiteral) return new Literal(value);

    if (s.Contains('+')) return parseBinOp('+', BinOp.Kind.SUM, s);
    if (s.Contains('-')) return parseBinOp('-', BinOp.Kind.SUB, s);
    if (s.Contains('*')) return parseBinOp('*', BinOp.Kind.MUL, s);
    if (s.Contains('/')) return parseBinOp('/', BinOp.Kind.DIV, s);

    throw new Exception("Expression can't be parsed");
}

BinOp parseBinOp(char symbol, BinOp.Kind kind, string s) {
    string[] split = s.Split(symbol, 2);
    Expr l = parseExpr(split[0]);
    Expr r = parseExpr(split[1]);
    return new BinOp(kind, l, r);
}

// REPL
Console.WriteLine(
    "Welcome to Calc (type 'bye' to exit)\n" +
    "Type an expression like '2 + 2' to get the result"
);
string? input = Console.ReadLine();
while (input != null && !input.Equals("bye")) {
    try {
        Expr expr = parseExpr(input);
        Console.WriteLine("= " + expr.eval());
    } catch (Exception e) {
        Console.WriteLine("! " + e.Message);
    }

    input = Console.ReadLine();
}
Console.WriteLine("bye!");
