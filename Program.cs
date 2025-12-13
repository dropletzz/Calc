Expr parseExpr(string s) {
    s = s.Trim(); // tolgo spazi inizali e finali da s

    double value;
    bool isLiteral = Double.TryParse(s, out value);
    if (isLiteral)       return new Literal(value);
    if (s.Contains('+')) return parseBinOp('+', BinOp.Kind.SUM, s);
    if (s.Contains('-')) return parseBinOp('-', BinOp.Kind.SUB, s);
    if (s.Contains('*')) return parseBinOp('*', BinOp.Kind.MUL, s);
    if (s.Contains('/')) return parseBinOp('/', BinOp.Kind.DIV, s);

    throw new Exception("Espressione non calcolabile");
}

BinOp parseBinOp(char symbol, BinOp.Kind kind, string s) {
    string[] split = s.Split(symbol, 2);
    Expr l = parseExpr(split[0]);
    Expr r = parseExpr(split[1]);
    return new BinOp(kind, l, r);
}

string s = "12 / 3.7 + 2 * 0.5";

try {
    Expr expr = parseExpr(s);
    Console.WriteLine(s + " = " + expr.eval());
} catch (Exception e) {
    Console.WriteLine(e.ToString());
}
