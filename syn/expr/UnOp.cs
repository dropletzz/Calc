// A unary operator (takes 1 argument)
public class UnOp : Expr {
    
    public enum Kind {
        LOG, SIN, NEG, LEN, NOT
    }

    private Kind kind;
    private Expr arg;

    public UnOp(Kind kind, Expr arg) {
        this.kind = kind;
        this.arg = arg;
    }

    public Value eval(Scope _) {
        Value val = arg.eval(_);
        if (val.kind == Value.Kind.Number) {
            switch (kind) {
                case Kind.LOG: return Value.number(Math.Log(val.num));
                case Kind.SIN: return Value.number(Math.Sin(val.num));
                case Kind.NEG: return Value.number(-val.num);
                case Kind.NOT: return Value.number(val.num == 0 ? 1 : 0);
            }
        }
        else if (val.kind == Value.Kind.Array) {
            switch (kind) {
                case Kind.LEN: return Value.number(val.capacity);
            }
        }
        throw new Exception("UnOp.eval unimplemented for " + kind + " " + val.kind);
    }

    public static Kind kindFromToken(Token t) {
        switch (t.kind) {
            case Token.Kind.LOG: return Kind.LOG;
            case Token.Kind.SIN: return Kind.SIN;
            case Token.Kind.NEG: return Kind.NEG;
            case Token.Kind.LEN: return Kind.LEN;
            case Token.Kind.NOT: return Kind.NOT;
        }
        throw new Syn.Error("UnOp.kindFromToken unimplemented for '" + t.kind + "'", t.loc);
    }

    public override string ToString() {
        return kind + "(" + arg + ")";
    }
}