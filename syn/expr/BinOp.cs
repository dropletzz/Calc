// A binary operator (takes 2 arguments)
public class BinOp : Expr {

    public enum Kind {
        SUM, SUB, MUL, DIV, POW, GT, LT, AND, OR, EQ, MOD
    }

    private Kind kind;
    private Expr l; // first argument (left hand side)
    private Expr r; // second argument (right hand side)

    public BinOp(Kind kind, Expr l, Expr r) {
        this.kind = kind;
        this.l = l;
        this.r = r;
    }

    public override Value eval(Scope _) {
        Value lval = l.eval(_);
        Value rval = r.eval(_);
        if (lval.kind == Value.Kind.Number && rval.kind == Value.Kind.Number) {
            switch (kind) {
                case Kind.SUM: return Value.number(lval.num + rval.num);
                case Kind.SUB: return Value.number(lval.num - rval.num);
                case Kind.MUL: return Value.number(lval.num * rval.num);
                case Kind.DIV: return Value.number(lval.num / rval.num);
                case Kind.POW: return Value.number(Math.Pow(lval.num, rval.num));
                case Kind.LT: return Value.number((lval.num < rval.num) ? 1 : 0);
                case Kind.GT: return Value.number((lval.num > rval.num) ? 1 : 0);
                case Kind.AND: return Value.number((lval.num != 0 && rval.num != 0) ? 1 : 0);
                case Kind.OR: return Value.number((lval.num != 0 || rval.num != 0) ? 1 : 0);
                case Kind.EQ: return Value.number((lval.num - rval.num == 0) ? 1 : 0);
                case Kind.MOD: {
                    int l = (int)lval.num;
                    int r = (int)rval.num;
                    return Value.number(((l % r) + r) % r);
                }
            }
        }
        throw new Exception("BinOp.eval unimplemented for "+lval.kind+" "+kind+" "+rval.kind);
    }

    public static Kind kindFromToken(Token t) {
        switch (t.kind) {
            case Token.Kind.PLUS_SIGN: case Token.Kind.PLUS: return Kind.SUM;
            case Token.Kind.DASH: case Token.Kind.MINUS:     return Kind.SUB;
            case Token.Kind.ASTERISK: case Token.Kind.TIMES: return Kind.MUL;
            case Token.Kind.SLASH: case Token.Kind.BY:       return Kind.DIV;
            case Token.Kind.CARET:                           return Kind.POW;
            case Token.Kind.OPAR_ANG:                        return Kind.LT;
            case Token.Kind.CPAR_ANG:                        return Kind.GT;
            case Token.Kind.AND:                             return Kind.AND;
            case Token.Kind.OR:                              return Kind.OR;
            case Token.Kind.DOUBLE_EQUALS:                   return Kind.EQ;
            case Token.Kind.PERCENT:                         return Kind.MOD;
        }
        throw new Syn.Error("BinOp.kindFromToken unimplemented for '" + t.kind + "'", t.loc);
    }

    public static int priorityFor(Token t) {
        Kind kind = kindFromToken(t);
        switch (kind) {
            case Kind.AND: case Kind.OR: return -3; // low priority
            case Kind.LT: case Kind.GT: case Kind.EQ: return -2;
            case Kind.MOD: return -1;
            case Kind.SUM: case Kind.SUB: return 0;
            case Kind.MUL: return 1;
            case Kind.DIV: return 2;
            case Kind.POW: return 3;
        }
        throw new Syn.Error("BinOp.priorityFor unimplemented for '" + kind + "'", t.loc);
    }

    public override string ToString() {
        return "(" + l + " " + kind + " " + r + ")";
    }
}