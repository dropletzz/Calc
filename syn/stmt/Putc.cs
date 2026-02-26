public class Putc : Stmt {
    private Expr expr;

    public Putc(Expr expr) {
        this.expr = expr;
    }

    public override Value exec(Scope _) {
        Value val = expr.eval(_);
        if (val.kind != Value.Kind.Number)
            throw new Exception("can't putc a " + val.kind);

        Console.Write((char)(int)val.num); 
        return val;
    }

    public override string ToString() {
        return "PUTC " + expr;
    }
}