// Operatore unario (con 1 argomento)
public class UnOp : Expr {
    
    public enum Kind {
        LOG, SIN
    }

    private Kind kind;
    private Expr arg;

    public UnOp(Kind kind, Expr arg) {
        this.kind = kind;
        this.arg = arg;
    }

    public double eval() {
        switch (kind) {
            case Kind.LOG: return Math.Log(arg.eval());
            case Kind.SIN: return Math.Sin(arg.eval());
        }
        throw new Exception("UNIMPLEMENTED: eval for UnOp.Kind" + kind);
    }

    public static Kind kindFromToken(Token t) {
        switch (t.kind) {
            case Token.Kind.LOG: return Kind.LOG;
            case Token.Kind.SIN: return Kind.SIN;
        }
        throw new Syn.Error("UnOp.kindFromToken unimplemented for '" + t.kind + "'", t.position);
    }

    public override string ToString() {
        return kind + "(" + arg.ToString() + ")";
    }
}