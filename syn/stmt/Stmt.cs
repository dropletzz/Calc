
public abstract class Stmt {
    protected Scope _;

    public Stmt(Scope _) {
        this._ = _;
    }

    public abstract double exec();
}