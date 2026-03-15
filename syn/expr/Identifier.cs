// An identifier is a variable that contains a value in the current Scope
public class Identifier : Expr, Assignable {
    public readonly string name;

    public Identifier(string name) {
        this.name = name;
    }

    public override Value eval(Scope _) {
        if (_.tryGet(name, out Value val)) return val;
        throw new Exception("Undefined identifier '"+name+"'");
    }

    public void set(Value val, Scope _) {
        _.set(name, val);
    }

    public override string ToString() {
        return name;
    }
}
