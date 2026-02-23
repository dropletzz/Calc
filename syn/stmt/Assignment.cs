
public class Assignment : Stmt {
    private Identifier id;
    private Expr assignee;

    public Assignment(Identifier id, Expr assignee) {
        this.id = id;
        this.assignee = assignee;
    }

    public override double exec(Scope _) {
        double result = assignee.eval(_);
        _.set(id.name, result);
        return result;
    }

    public override string ToString() {
        return "ASSIGN " + assignee + " TO " + id;
    }
}