// Recursive function that takes a string and returns an Expr
Expr parseExpr(string s) {
    s = s.Trim();

    double value;
    bool isLiteral = Double.TryParse(s, out value);
    if (isLiteral) return new Literal(value);

    if (s.Contains('+')) return parseBinOp('+', BinOp.KIND.SUM, s);
    if (s.Contains('-')) return parseBinOp('-', BinOp.KIND.SUB, s);
    if (s.Contains('*')) return parseBinOp('*', BinOp.KIND.MUL, s);
    if (s.Contains('/')) return parseBinOp('/', BinOp.KIND.DIV, s);

    throw new Exception("Expression can't be parsed");
}

BinOp parseBinOp(char symbol, BinOp.KIND kind, string s) {
    string[] split = s.Split(symbol, 2);
    Expr l = parseExpr(split[0]);
    Expr r = parseExpr(split[1]);
    return new BinOp(kind, l, r);
}

// REPL
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
