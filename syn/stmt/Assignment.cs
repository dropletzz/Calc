
public class Assignment : Stmt {
    private string name;
    private Expr assignee;

    public Assignment(string name, Expr assignee, Scope _) : base(_) {
        this.name = name;
        this.assignee = assignee;
    }

    public override double exec() {
        double result = assignee.eval(_);
        _.set(name, result);
        return result;
    }

    public override string ToString() {
        return "ASSIGN " + assignee + " TO " + name;
    }
}