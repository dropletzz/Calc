// Un valore letterale
public class Literal : Expr {
    private double value;

    public Literal(double value) {
        this.value = value;
    }

    public double eval() {
        return value;
    }
}
