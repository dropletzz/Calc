Expr parseExpr(string s) {
    s = s.Trim(); // tolgo spazi inizali e finali da s

    double value;
    bool isLiteral = Double.TryParse(s, out value);
    if (isLiteral)       return new Literal(value);
    if (s.Contains('+')) {
        string[] split = s.Split('+', 2); // divido la stringa in due dove trovo il simbolo
        Expr l = parseExpr(split[0]);
        Expr r = parseExpr(split[1]);
        return new BinOp(BinOp.Kind.SUM, l, r);
    }
    if (s.Contains('-')) {
        string[] split = s.Split('-', 2);
        Expr l = parseExpr(split[0]);
        Expr r = parseExpr(split[1]);
        return new BinOp(BinOp.Kind.SUB, l, r);
    }
    if (s.Contains('*')) {
        string[] split = s.Split('*', 2);
        Expr l = parseExpr(split[0]);
        Expr r = parseExpr(split[1]);
        return new BinOp(BinOp.Kind.MUL, l, r);
    }
    if (s.Contains('/')) {
        string[] split = s.Split('/', 2);
        Expr l = parseExpr(split[0]);
        Expr r = parseExpr(split[1]);
        return new BinOp(BinOp.Kind.DIV, l, r);
    }

    throw new Exception("Espressione non calcolabile");
}

string s = "12 / 3.7 + 2 * 0.5";

try {
    Expr result = parseExpr(s);
    Console.WriteLine(s + " = " + result.eval());
} catch (Exception e) {
    Console.WriteLine("Espressione non valida");
}
