// A literal value
public class Literal : Expr {
    private Value value;

    public Literal(Value value) {
        this.value = value;
    }

    public Value eval(Scope _) {
        return value;
    }

    public override string ToString() {
        return "" + value;
    }
}
