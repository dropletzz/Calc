// A literal value
public class Literal : Expr {
    private double value;

    public Literal(double value) {
        this.value = value;
    }

    public double eval(Scope _) {
        return value;
    }

    public override string ToString() {
        return "" + value;
    }
}
