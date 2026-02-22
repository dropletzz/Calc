
public class ExprStmt : Stmt {
    private Expr expr;

    public ExprStmt(Expr expr, Scope _) : base(_) {
        this.expr = expr;
    }

    public override double exec() {
        return expr.eval(_);
    }

    public override string ToString() {
        return "EXPR "+expr.ToString();
    }
}