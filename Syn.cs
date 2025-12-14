// Analizzatore sintattico
public static class Syn {
    public static Expr parseExpr(string s) {
        s = s.Trim(); // tolgo spazi inizali e finali da s

        // Valore letterale
        double value;
        bool isLiteral = Double.TryParse(s, out value);
        if (isLiteral) return new Literal(value);

        // Operatori binari
        foreach (string sym in BinOp.SYMBOLS) {
            int index = s.LastIndexOf(sym);
            if (index > 0 && index < s.Length-1) {
                return parseBinOp(sym, index, s);
            }
        }

        // Operatori unari
        foreach (string sym in UnOp.SYMBOLS) {
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

        throw new Error("Could not parse expression '" + s + "'");
    }

    private static BinOp parseBinOp(string symbol, int symbolIndex, string s) {
        BinOp.Kind kind = BinOp.kindFromSymbol(symbol);

        string beforeSymbol = s.Substring(0, symbolIndex);
        string afterSymbol = s.Substring(symbolIndex + symbol.Length);

        Expr l = parseExpr(beforeSymbol);
        Expr r = parseExpr(afterSymbol);
        return new BinOp(kind, l, r);
    }

    public class Error : Exception {
        public readonly string message = "";

        public Error(string message) {
            this.message = message;
        }
    }
}