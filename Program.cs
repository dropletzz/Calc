// simboli consentiti per operazioni binarie
char[] BINOP_SYMBOLS = { '+', '-', '*', '/' };

Expr parseExpr(string s) {
    s = s.Trim(); // tolgo spazi inizali e finali da s

    double value;
    bool isLiteral = Double.TryParse(s, out value);
    if (isLiteral)       return new Literal(value);
    foreach (char sym in BINOP_SYMBOLS) {
        int index = s.IndexOf(sym);
        if (index > 0 && index < s.Length-1) {
            return parseBinOp(sym, index, s);
        }
    }

    throw new Exception("Espressione non calcolabile");
}

BinOp parseBinOp(char symbol, int symbolIndex, string s) {
    BinOp.Kind kind = BinOp.kindFromSymbol(symbol);

    string[] split = s.Split(symbol, 2);
    string beforeSymbol = s.Substring(0, symbolIndex);
    string afterSymbol = s.Substring(symbolIndex + 1);

    Expr l = parseExpr(beforeSymbol);
    Expr r = parseExpr(afterSymbol);
    return new BinOp(kind, l, r);
}

string s = "12 / 3.7 + 2 * 0.5";

try {
    Expr expr = parseExpr(s);
    Console.WriteLine(s + " = " + expr.eval());
} catch (Exception e) {
    Console.WriteLine(e.ToString());
}
