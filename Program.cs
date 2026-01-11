// Recursive function that takes a string and returns an Expr
Expr parseExpr(string s) {
    s = s.Trim();

    double value;
    bool isLiteral = Double.TryParse(s, out value);
    if (isLiteral) return new Literal(value);

    if (s.Contains('+')) {
        string[] split = s.Split('+', 2);
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

    throw new Exception("Expression can't be parsed");
}

string exprString = "2 + 12 / 4 * 0.5";

try {
    Expr result = parseExpr(exprString);
    Console.WriteLine(exprString + " = " + result.eval());
} catch (Exception e) {
    Console.WriteLine(e.Message);
}
