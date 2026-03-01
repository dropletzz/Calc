public class Scope {
    private Dictionary<string, Value> bindings;
    private Scope? parent;

    public Scope() {
        this.bindings = new Dictionary<string, Value>();
        this.parent = null;
    }

    public Scope(Scope parent) {
        this.bindings = new Dictionary<string, Value>();
        this.parent = parent;
    }

    public void set(string name, Value value) {
        if (this.parent != null && this.parent.isSet(name)) {
            this.parent.set(name, value);
        } else {
            if (this.bindings.ContainsKey(name)) {
                Value val = this.bindings[name];
                if (val.kind == Value.Kind.Array) val.freeArray();
            }
            this.bindings[name] = value;
        }
    }

    public bool isSet(string name) {
        if (this.bindings.ContainsKey(name)) return true;
        else if (this.parent == null) return false;
        return this.parent.isSet(name);
    }

    public bool get(string name, out Value value) {
        if (this.bindings.TryGetValue(name, out value)) return true;
        if (this.parent != null) return this.parent.get(name, out value);
        value = new Value();
        return false;
    }

    public void exit() {
        foreach (var (name, val) in bindings) {
            if (val.kind == Value.Kind.Array) val.freeArray();
        }
    }
}