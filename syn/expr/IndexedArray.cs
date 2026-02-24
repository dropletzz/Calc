public class IndexedArray : Expr {
    
    public readonly Expr indExpr;
    public readonly Identifier id;

    public IndexedArray(Expr indExpr, Identifier id) {
        this.indExpr = indExpr;
        this.id = id;
    }

    public Value eval(Scope _) {
        Value index = indExpr.eval(_);
        if (index.kind != Value.Kind.Number)
            throw new Exception("Can't use " + index.kind + " as an index");

        Value array;
        if (_.get(id.name, out array)) {
            if (array.kind == Value.Kind.Array) {
                int i = (int)index.num;
                if (i < 0 || i >= array.capacity)
                    throw new Exception("Index '" + i + "' is out of bounds");

                double result = array.arr[i];
                return Value.number(result);
            }
            else throw new Exception("Can't index value of type " + array.kind);
        }
        throw new Exception("'"+id.name+"' is undefined in the current scope");
    }

    public override string ToString() {
        return  "[" + indExpr + "]" + id;
    }
}