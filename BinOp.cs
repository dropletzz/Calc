// A bynary operator (takes 2 arguments)
public class BinOp : Expr {
    // BinOp symbols                          low precedence >>>>>>>>>>>>>>>>>>>>>>>>>>>> high precedence
    public static readonly string[] SYMBOLS = { "+", "plus", "-", "minus", "*", "times", "/", "by", "^" };
    
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
            case "+": case "plus":  return Kind.SUM;
            case "-": case "minus": return Kind.SUB;
            case "*": case "times": return Kind.MUL;
            case "/": case "by":    return Kind.DIV;
            case "^":               return Kind.POW;
        }
        throw new Exception("UNIMPLEMENTED: BinOp.kindFromSymbol for '" + c + "'");
    }

    public override string ToString() {
        return "(" + l + " " + kind + " " + r + ")";
    }
}