public class IndexedAssignment : Stmt {
    private IndexedArray arrayIndex;
    private Expr assignee;

    public IndexedAssignment(IndexedArray arrayIndex, Expr assignee) {
        this.arrayIndex = arrayIndex;
        this.assignee = assignee;
    }

    public override Value exec(Scope _) {
        Value result = assignee.eval(_);
        Value index = arrayIndex.indExpr.eval(_);
        Value array;
        if (_.get(arrayIndex.id.name, out array)) {
            if (array.kind == Value.Kind.Array) {
                int i = (int)index.num;
                array.arr[i] = result.num;
                return result;
            }
            else throw new Exception("Can't index value of type " + array.kind);
        }
        throw new Exception("'"+arrayIndex.id.name+"' is undefined in the current scope");
    }

    public override string ToString() {
        return "ASSIGN " + assignee + " TO " + arrayIndex;
    }
}