public class ExprStmt : Stmt {
    private Expr expr;

    public ExprStmt(Expr expr) {
        this.expr = expr;
    }

    public override Value exec(Scope _) {
        return expr.eval(_);
    }

    public override string ToString() {
        return "EXPR "+expr.ToString();
    }
}