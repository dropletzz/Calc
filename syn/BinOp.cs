// A binary operator (takes 2 arguments)
public class BinOp : Expr {

    public enum Kind {
        SUM, SUB, MUL, DIV, POW, ASS
    }

    private Kind kind;
    private Expr l; // first argument (left hand side)
    private Expr r; // second argument (right hand side)

    public BinOp(Kind kind, Expr l, Expr r) {
        this.kind = kind;
        this.l = l;
        this.r = r;
    }

    public double eval(Scope _) {
        switch (kind) {
            case Kind.SUM: return l.eval(_) + r.eval(_);
            case Kind.SUB: return l.eval(_) - r.eval(_);
            case Kind.MUL: return l.eval(_) * r.eval(_);
            case Kind.DIV: return l.eval(_) / r.eval(_);
            case Kind.POW: return Math.Pow(l.eval(_), r.eval(_));
            case Kind.ASS: {
                if (l is not Identifier) throw new Exception("Only identifiers can be assigned");
                Identifier id = (Identifier) l;
                double val = r.eval(_);
                _.set(id.name, val);
                return val;
            }
        }
        throw new Exception("BinOp.eval unimplemented for " + kind);
    }

    public static Kind kindFromToken(Token t) {
        switch (t.kind) {
            case Token.Kind.PLUS_SIGN: case Token.Kind.PLUS: return Kind.SUM;
            case Token.Kind.DASH: case Token.Kind.MINUS:     return Kind.SUB;
            case Token.Kind.ASTERISK: case Token.Kind.TIMES: return Kind.MUL;
            case Token.Kind.SLASH: case Token.Kind.BY:       return Kind.DIV;
            case Token.Kind.CARET:                           return Kind.POW;
            case Token.Kind.EQUALS:                          return Kind.ASS;
        }
        throw new Syn.Error("BinOp.kindFromToken unimplemented for '" + t.kind + "'", t.position);
    }

    public static int priorityFor(Token t) {
        Kind kind = kindFromToken(t);
        switch (kind) {
            case Kind.SUM: case Kind.SUB: return 0; // low priority
            case Kind.MUL: return 1;
            case Kind.DIV: return 2;
            case Kind.POW: return 3;
            case Kind.ASS: return 4; // high priority
        }
        throw new Syn.Error("BinOp.priorityFor unimplemented for '" + kind + "'", t.position);
    }

    public override string ToString() {
        return "(" + l + " " + kind + " " + r + ")";
    }
}