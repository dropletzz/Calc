public class Print : Stmt {
    private Expr expr;

    public Print(Expr expr) {
        this.expr = expr;
    }

    public override Value exec(Scope _) {
        Value result = expr.eval(_);
        Console.WriteLine(result);
        return result;
    }

    public override string ToString() {
        return "PRINT "+expr.ToString();
    }
}