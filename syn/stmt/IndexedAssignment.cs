public class IndexedAssignment : Stmt {
    private IndexedArray arrayIndex;
    private Expr assignee;

    public IndexedAssignment(IndexedArray arrayIndex, Expr assignee) {
        this.arrayIndex = arrayIndex;
        this.assignee = assignee;
    }

    public override Value exec(Scope _) {
        Value result = assignee.eval(_);
        if (result.kind != Value.Kind.Number)
            throw new Exception("Can't assign a " + result.kind + " to an array");

        arrayIndex.set(result.num, _);
        return result;
    }

    public override string ToString() {
        return "ASSIGN " + assignee + " TO " + arrayIndex;
    }
}