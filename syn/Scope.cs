
public class Scope {
    private Dictionary<string, double> bindings;

    public Scope() {
        this.bindings = new Dictionary<string, double>();
    }

    public void set(string name, double value) {
        this.bindings[name] = value;
    }

    public bool get(string name, out double value) {
        try {
            value = this.bindings[name];
            return true;
        } catch (KeyNotFoundException _e) {
            value = 0;
            return false;
        }
    }
}