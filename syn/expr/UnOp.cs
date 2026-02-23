// A unary operator (takes 1 argument)
public class UnOp : Expr {
    
    public enum Kind {
        LOG, SIN, NEG
    }

    private Kind kind;
    private Expr arg;

    public UnOp(Kind kind, Expr arg) {
        this.kind = kind;
        this.arg = arg;
    }

    public double eval(Scope _) {
        switch (kind) {
            case Kind.LOG: return Math.Log(arg.eval(_));
            case Kind.SIN: return Math.Sin(arg.eval(_));
            case Kind.NEG: return - arg.eval(_);
        }
        throw new Exception("UnOp.eval unimplemented for " + kind);
    }

    public static Kind kindFromToken(Token t) {
        switch (t.kind) {
            case Token.Kind.LOG: return Kind.LOG;
            case Token.Kind.SIN: return Kind.SIN;
            case Token.Kind.NEG: return Kind.NEG;
        }
        throw new Syn.Error("UnOp.kindFromToken unimplemented for '" + t.kind + "'", t.position);
    }

    public override string ToString() {
        return kind + "(" + arg + ")";
    }
}