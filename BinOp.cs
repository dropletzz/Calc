
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

    public static BinOp.Kind fromSymbol(char c) {
        switch (c) {
            case '+': return Kind.SUM;
            case '-': return Kind.SUB;
            case '*': return Kind.MUL;
            case '/': return Kind.DIV;
        }
        throw new Exception("Unkown BinOp symbol: " + c);
    }
}