public class Scope {
    private Dictionary<string, Value> bindings;
    private List<Value> tempValues;
    private Scope? parent;

    public Scope() {
        this.bindings = new Dictionary<string, Value>();
        this.tempValues = new List<Value>();
        this.parent = null;
    }

    public Scope(Scope parent) {
        this.bindings = new Dictionary<string, Value>();
        this.tempValues = new List<Value>();
        this.parent = parent;
    }

    public void set(string name, Value value) {
        if (this.parent != null && this.parent.isSet(name)) {
            this.parent.set(name, value);
        } else {
            if (this.bindings.ContainsKey(name)) {
                this.bindings[name].free();
            }
            this.bindings[name] = value;
        }
    }

    public bool isSet(string name) {
        if (this.bindings.ContainsKey(name)) return true;
        else if (this.parent == null) return false;
        return this.parent.isSet(name);
    }

    public bool tryGet(string name, out Value value) {
        if (this.bindings.TryGetValue(name, out value)) return true;
        if (this.parent != null) return this.parent.tryGet(name, out value);
        value = new Value();
        return false;
    }

    public void addTempValue(Value val) {
        this.tempValues.Add(val);
    }

    public void clearTempMemory() {
        foreach (var val in this.tempValues) val.free();
        this.tempValues.Clear();
    }

    public void clearMemory() {
        foreach (var (name, val) in this.bindings) val.free();
        this.clearTempMemory();
    }
}