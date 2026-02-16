// Recursive function that takes a string and returns an Expr
Expr parseExpr(string s) {
    s = s.Trim();

    double value;
    bool isLiteral = Double.TryParse(s, out value);
    if (isLiteral) return new Literal(value);

    if (s.Contains('+')) {
        int opIndex = s.LastIndexOf('+');
        Expr l = parseExpr(s.Substring(0, opIndex));
        Expr r = parseExpr(s.Substring(opIndex + 1));
        return new BinOp(BinOp.Kind.SUM, l, r);
    }
    if (s.Contains('-')) {
        int opIndex = s.LastIndexOf('-');
        Expr l = parseExpr(s.Substring(0, opIndex));
        Expr r = parseExpr(s.Substring(opIndex + 1));
        return new BinOp(BinOp.Kind.SUB, l, r);
    }
    if (s.Contains('*')) {
        int opIndex = s.LastIndexOf('*');
        Expr l = parseExpr(s.Substring(0, opIndex));
        Expr r = parseExpr(s.Substring(opIndex + 1));
        return new BinOp(BinOp.Kind.MUL, l, r);
    }
    if (s.Contains('/')) {
        int opIndex = s.LastIndexOf('/');
        Expr l = parseExpr(s.Substring(0, opIndex));
        Expr r = parseExpr(s.Substring(opIndex + 1));
        return new BinOp(BinOp.Kind.DIV, l, r);
    }

    throw new Exception("Expression can't be parsed");
}

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