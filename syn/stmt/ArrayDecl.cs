public class ArrayDecl : Stmt {
    private Identifier id;
    private Expr capacityExpr;
    private LiteralArray? literal;

    public ArrayDecl(Identifier id, Expr capacityExpr) {
        this.id = id;
        this.capacityExpr = capacityExpr;
        this.literal = null;
    }

    public ArrayDecl(Identifier id, Expr capacityExpr, LiteralArray literal) {
        this.id = id;
        this.capacityExpr = capacityExpr;
        this.literal = literal;
    }

    public override Value exec(Scope _) {
        Value capacityValue = capacityExpr.eval(_);
        if (capacityValue.kind != Value.Kind.Number)
            throw new Exception("Array capacity must be a number, found " + capacityValue.kind);

        int capacity = (int)capacityValue.num;
        Value result = Value.array(capacity);

        if (literal != null) {
            int count = literal.count();
            for (int i = 0; i < capacity; i++) {
                Value val = literal.evalAt(i % count, _);
                if (val.kind != Value.Kind.Number)
                    throw new Exception("array elements must be numbers");
                result.arr[i] = val.num;
            }
        }

        _.set(id.name, result);
        return result;
    }

    public override string ToString() {
        return "ASSIGN ARRAY("+capacityExpr+") TO " + id;
    }
}