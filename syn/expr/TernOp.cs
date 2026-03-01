// A ternary operator (takes 3 arguments)
public class TernOp : Expr {

    public enum Kind {
        CONDITIONAL
    }

    private Kind kind;
    private Expr arg0; // first argument
    private Expr arg1; // second argument
    private Expr arg2; // third argument

    public TernOp(Kind kind, Expr arg0, Expr arg1, Expr arg2) {
        this.kind = kind;
        this.arg0 = arg0;
        this.arg1 = arg1;
        this.arg2 = arg2;
    }

    public override Value eval(Scope _) {
        Value arg0Val = arg0.eval(_);
        if (arg0Val.kind == Value.Kind.Number) {
            switch (kind) {
                case Kind.CONDITIONAL: {
                    if (arg0Val.num != 0.0)
                        return arg1.eval(_);
                    else
                        return arg2.eval(_);
                }
            }
        }
        throw new Exception("TernOp.eval unimplemented for " + kind + " " + arg0Val.kind);
    }

    public static Kind kindFromTokens(Token a, Token b) {
        return (a.kind, b.kind) switch {
            (Token.Kind.QUESTION_MARK, Token.Kind.COLON) => Kind.CONDITIONAL,
            _ => throw new Syn.Error("TernOp.kindFromToken unimplemented for '" + a.kind + "' and '" + b.kind + "'", a.loc)
        };
    }

    public override string ToString() {
        return kind + "(" + arg0 + ", " + arg1 + ", " + arg2 + ")";
    }
}