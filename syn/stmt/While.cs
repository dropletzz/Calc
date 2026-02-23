
public class While : Stmt {
    private Expr cond;
    private Block body;

    public While(Expr cond, Block body) {
        this.cond = cond;
        this.body = body;
    }

    public override double exec(Scope _) {
        double result = 0;
        while (cond.eval(_) != 0) {
            result = body.exec(_);
        }
        return result;
    }

    public override string ToString() {
        return "WHILE "+cond+" DO "+body.ToString();
    }
}