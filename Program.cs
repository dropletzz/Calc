// Simboli consentiti per operazioni unarie.
// Precedenza: bassa >>>>>>>>>>>>>>> alta 
string[] UNOP_SYMBOLS = { "log", "sin" };

// Simboli consentiti per operazioni binarie.
// Precedenza: bassa >>>>>>>>>>>>>>>>>>>> alta 
char[] BINOP_SYMBOLS = { '+', '-', '*', '/' };

Expr parseExpr(string s) {
    s = s.Trim(); // tolgo spazi inizali e finali da s

    // Valore letterale
    double value;
    bool isLiteral = Double.TryParse(s, out value);
    if (isLiteral) return new Literal(value);

    // Operatori binari
    foreach (char sym in BINOP_SYMBOLS) {
        int index = s.LastIndexOf(sym);
        if (index > 0 && index < s.Length-1) {
            return parseBinOp(sym, index, s);
        }
    }

    // Operatori unari
    foreach (string sym in UNOP_SYMBOLS) {
        int index = s.IndexOf(sym);
        int afterSymbol = index + sym.Length;
        if (
            s.Length > afterSymbol + 2 &&
            s[afterSymbol] == ' ' &&
            s[afterSymbol + 1] != ' '
        ) {
            UnOp.Kind kind = UnOp.kindFromSymbol(sym);
            Expr arg = parseExpr(s.Substring(afterSymbol));
            return new UnOp(kind, arg);
        }
    }

    throw new Exception("Could not parse expression: " + s);
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

string[] tests = {
    "12 / 3.7 + 2 * 0.5",
    "log 10 * 2 - sin 20 - 7",
    "3/ ",
    "log 10 * 2 - sin 20 - 7 log",
    "log * 10 * 2 - sin 20 - 7 ",
};

Console.WriteLine("---------------------");
foreach (string test in tests) {
    Console.WriteLine(test);
    try {
        Expr expr = parseExpr(test);
        Console.WriteLine("= " + expr.eval());
        Console.WriteLine(expr.ToString());
    } catch (Exception e) {
        Console.WriteLine("= Syntax error");
    }
        Console.WriteLine("---------------------");
}
