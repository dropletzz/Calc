public class Assignment : Stmt {
    protected Assignable assigned;
    protected Expr assignee;

    public Assignment(Assignable assigned, Expr assignee) {
        this.assigned = assigned;
        this.assignee = assignee;
        this.assignee.setIsAssigned(true);
    }

    public override Value exec(Scope _) {
        Value result = assignee.eval(_);
        if (this.assignee is Identifier && !result.isCopy())
            result = result.clone();
        assigned.set(result, _);
        return result;
    }

    public override string ToString() {
        return "ASSIGN " + assignee + " TO " + assigned;
    }
}