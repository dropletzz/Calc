Expr parseExpr(string s) {
    s = s.Trim(); // tolgo spazi inizali e finali da s

    double value;
    bool isLiteral = Double.TryParse(s, out value);
    if (isLiteral)       return new Literal(value);
    if (s.Contains('+')) return parseBinOp(s, '+', BinOp.Kind.SUM); // << precedenza bassa
    if (s.Contains('-')) return parseBinOp(s, '-', BinOp.Kind.SUB);
    if (s.Contains('*')) return parseBinOp(s, '*', BinOp.Kind.MUL);
    if (s.Contains('/')) return parseBinOp(s, '/', BinOp.Kind.DIV); // >> precedenza alta

    throw new Exception("Espressione non calcolabile");
}

BinOp parseBinOp(string s, char symbol, BinOp.Kind kind) {
    string[] split = s.Split(symbol, 2); // divido la stringa in due dove trovo il simbolo
    Expr l = parseExpr(split[0]);
    Expr r = parseExpr(split[1]);
    return new BinOp(kind, l, r);
}

string s = "12 / 3.7 + 2 * 0.5";

try {
    Expr result = parseExpr(s);
    Console.WriteLine(s + " = " + result.eval());
} catch (Exception e) {
    Console.WriteLine("Espressione non valida");
}
