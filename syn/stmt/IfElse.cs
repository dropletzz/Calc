public class IfElse : Stmt {
    private Expr cond;
    private Stmt ifTrue;
    private Stmt? ifFalse;

    public IfElse(Expr cond, Stmt ifTrue, Stmt? ifFalse) {
        this.cond = cond;
        this.ifTrue = ifTrue;
        this.ifFalse = ifFalse;
    }

    public override Value exec(Scope _) {
        Value result = new Value();
        if (cond.eval(_).num != 0) {
            result = ifTrue.exec(_);
        }
        else if (ifFalse != null) {
            result = ifFalse.exec(_);
        }
        return result;
    }

    public override string ToString() {
        string s = "IF "+cond+" DO "+ifTrue;
        if (ifFalse != null) s += " ELSE "+ifFalse;
        return s;
    }
}
