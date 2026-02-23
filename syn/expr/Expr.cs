// An expression: something that can be evaluated
public interface Expr {
    public abstract Value eval(Scope _);
}