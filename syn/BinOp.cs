// A binary operator (takes 2 arguments)
public class BinOp : Expr {

    public enum Kind {
        SUM, SUB, MUL, DIV, POW
    }

    private Kind kind;
    private Expr l; // first argument (left hand side)
    private Expr r; // second argument (right hand side)

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
        throw new Exception("BinOp.eval unimplemented for " + kind);
    }

    public static Kind kindFromToken(Token t) {
        switch (t.kind) {
            case Token.Kind.PLUS_SIGN: case Token.Kind.PLUS: return Kind.SUM;
            case Token.Kind.DASH: case Token.Kind.MINUS:     return Kind.SUB;
            case Token.Kind.ASTERISK: case Token.Kind.TIMES: return Kind.MUL;
            case Token.Kind.SLASH: case Token.Kind.BY:       return Kind.DIV;
            case Token.Kind.TICK:                            return Kind.POW;
        }
        throw new Syn.Error("BinOp.kindFromToken unimplemented for '" + t.kind + "'", t.position);
    }

    public static int priorityFor(Token t) {
        Kind kind = kindFromToken(t);
        switch (kind) {
            case Kind.SUM: case Kind.SUB: return 0; // low priority
            case Kind.MUL: return 1;
            case Kind.DIV: return 2;
            case Kind.POW: return 3; // high priority
        }
        throw new Syn.Error("BinOp.priorityFor unimplemented for '" + kind + "'", t.position);
    }

    public override string ToString() {
        return "(" + l + " " + kind + " " + r + ")";
    }
}