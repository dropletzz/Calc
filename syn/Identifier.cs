// An identifier is a variable that contains a value in the current Scope
public class Identifier : Expr {
    public readonly string name;

    public Identifier(string name) {
        this.name = name;
    }

    public double eval(Scope _) {
        return _.get(name);
    }

    public override string ToString() {
        return '"' + name + '"';
    }
}
