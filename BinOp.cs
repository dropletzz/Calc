// Operatore binario (con due argomenti: l e r)
public class BinOp : Expr {
    
    public enum Kind {
        SUM, SUB, MUL, DIV, POW
    }

    private Kind kind;
    private Expr l; // left hand side
    private Expr r; // right hand side

    public BinOp(Kind kind, Expr l, Expr r) {
        this.kind = kind;
        this.l = l;
        this.r = r;
    }

    public double eval() {
        switch (kind) {
            case Kind.SUM: return l.eval() + r.eval();
            case Kind.SUB: return l.eval() - r.eval();
            case Kind.MUL: return l.eval() * r.eval();
            case Kind.DIV: return l.eval() / r.eval();
            case Kind.POW: return Math.Pow(l.eval(), r.eval());
        }
        throw new Exception("UNIMPLEMENTED: eval for BinOp.Kind" + kind);
    }

    public static Kind kindFromSymbol(string c) {
        switch (c) {
            case "+": return Kind.SUM;
            case "-": return Kind.SUB;
            case "*": return Kind.MUL;
            case "/": return Kind.DIV;
            case "^": return Kind.POW;
        }
        throw new Exception("UNIMPLEMENTED: BinOp.kindFromSymbol for '" + c + "'");
    }

    public override string ToString() {
        return "(" + l + " " + kind + " " + r + ")";
    }
}