// A bynary operator (takes 2 arguments)
public class BinOp : Expr {
    
    public enum Kind {
        SUM, SUB, MUL, DIV
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
            case Kind.SUM:
                return l.eval() + r.eval();
            case Kind.SUB:
                return l.eval() - r.eval();
            case Kind.MUL:
                return l.eval() * r.eval();
            case Kind.DIV:
                return l.eval() / r.eval();
            default:
                throw new Exception("Operazione sconosciuta: " + kind);
        }
    }
}