public class While : Stmt {
    private Expr cond;
    private Block body;

    public While(Expr cond, Block body) {
        this.cond = cond;
        this.body = body;
    }

    public override Value exec(Scope _) {
        Value result = new Value();
        while (cond.eval(_).num != 0) {
            result = body.exec(_);
        }
        return result;
    }

    public override string ToString() {
        return "WHILE "+cond+" DO "+body.ToString();
    }
}