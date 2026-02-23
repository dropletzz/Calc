// An identifier is a variable that contains a value in the current Scope
public class Identifier : Expr {
    public readonly string name;

    public Identifier(string name) {
        this.name = name;
    }

    public double eval(Scope _) {
        double value;
        if (_.get(name, out value)) {
            return value;
        }
        throw new Exception("Undefined identifier '"+name+"'");
    }

    public override string ToString() {
        return name;
    }
}
