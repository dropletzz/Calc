// An identifier is a variable that contains a value in the current Scope
public class Identifier : Expr, Assignable {
    public readonly string name;

    public Identifier(string name) {
        this.name = name;
    }

    public override Value eval(Scope _) {
        Value value;
        if (_.get(name, out value)) return value;
        throw new Exception("Undefined identifier '"+name+"'");
    }

    public void set(double num, Scope _) {
        _.set(name, Value.number(num));
    }

    public override string ToString() {
        return name;
    }
}
