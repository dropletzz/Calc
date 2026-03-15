public class Assignment : Stmt {
    protected Identifier id;
    protected Expr assignee;

    public Assignment(Identifier id, Expr assignee) {
        this.id = id;
        this.assignee = assignee;
        this.assignee.setIsAssigned(true);
    }

    public override Value exec(Scope _) {
        Value result = assignee.eval(_);
        if (this.assignee is Identifier && !result.isCopy())
            result = result.clone();
        _.set(id.name, result);
        return result;
    }

    public override string ToString() {
        return "ASSIGN " + assignee + " TO " + id;
    }
}