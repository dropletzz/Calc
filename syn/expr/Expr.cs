// An expression: something that can be evaluated
public interface Expr {
    public abstract double eval(Scope _);
}