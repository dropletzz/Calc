public class ArrayDecl : Stmt {
    private int capacity;
    private Identifier id;

    public ArrayDecl(int capacity, Identifier id) {
        this.capacity = capacity;
        this.id = id;
    }

    public override Value exec(Scope _) {
        Value result = Value.array(capacity);
        _.set(id.name, result);
        return result;
    }

    public override string ToString() {
        return "ASSIGN ARRAY("+capacity+") TO " + id;
    }
}