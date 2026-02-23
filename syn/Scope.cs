
public class Scope {
    private Dictionary<string, double> bindings;
    private Scope? parent;

    public Scope() {
        this.bindings = new Dictionary<string, double>();
        this.parent = null;
    }

    public Scope(Scope parent) {
        this.bindings = new Dictionary<string, double>();
        this.parent = parent;
    }

    public void set(string name, double value) {
        if (this.parent.isSet(name)) {
            this.parent.set(name, value);
        } else {
            this.bindings[name] = value;
        }
    }

    public bool isSet(string name) {
        if (this.bindings.ContainsKey(name)) return true;
        else if (this.parent == null) return false;
        return this.parent.isSet(name);
    }

    public bool get(string name, out double value) {
        try {
            value = this.bindings[name];
            return true;
        } catch (KeyNotFoundException _e) {
            if (this.parent != null) {
                return this.parent.get(name, out value);
            }
            value = 0;
            return false;
        }
    }
}