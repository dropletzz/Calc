public class IndexedArray : Expr, Assignable {
    
    public readonly Expr indExpr;
    public readonly Identifier id;

    public IndexedArray(Identifier id, Expr indExpr) {
        this.id = id;
        this.indExpr = indExpr;
    }

    public override Value eval(Scope _) {
        var (i, val) = ival(_);
        return Value.number(val.arr[i]);
    }

    public void set(double num, Scope _) {
        var (i, val) = ival(_);
        val.arr[i] = num;
    }

    private (int i, Value val) ival(Scope _) {
        Value index = indExpr.eval(_);
        if (index.kind != Value.Kind.Number)
            throw new Exception("Can't use " + index.kind + " as an index");
        int i = (int)index.num;

        if (_.tryGet(id.name, out Value val)) {
            if (val.kind != Value.Kind.Array) 
                throw new Exception("Can't index value of type " + val.kind);
            if (i < 0 || i >= val.capacity)
                throw new Exception("Index '" + i + "' is out of bounds");
            return (i, val);
        }

        throw new Exception("'"+id.name+"' is undefined in the current scope");
    }

    public override string ToString() {
        return  "[" + indExpr + "]" + id;
    }
}