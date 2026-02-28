public class ArrayDecl : Stmt {
    private Expr capacity;
    private Identifier id;

    public ArrayDecl(Identifier id, Expr capacity) {
        this.id = id;
        this.capacity = capacity;
    }

    public override Value exec(Scope _) {
        Value capacityValue = capacity.eval(_);
        if (capacityValue.kind != Value.Kind.Number)
            throw new Exception("Array capacity must be a number, found " + capacityValue.kind);

        Value result = Value.array((int)capacityValue.num);
        if (_.isSet(id.name)) throw new Exception("Can't redeclare an array");
        _.set(id.name, result);
        return result;
    }

    public override string ToString() {
        return "ASSIGN ARRAY("+capacity+") TO " + id;
    }
}