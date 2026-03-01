// An expression: something that can be evaluated
public abstract class Expr {
    protected bool isAssigned;

    public Expr() {
        this.isAssigned = false;
    }

    public void setIsAssigned(bool isAssigned) {
        this.isAssigned = isAssigned;
    }

    public abstract Value eval(Scope _);
}