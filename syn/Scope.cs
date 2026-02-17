
public class Scope {
    private Dictionary<string, double> bindings;

    public Scope() {
        this.bindings = new Dictionary<string, double>();
    }

    public void set(string name, double value) {
        this.bindings[name] = value;
    }

    public double get(string name) {
        return this.bindings[name];
    }
}