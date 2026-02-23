
public class StmtList : Stmt {
    private List<Stmt> statements;

    public StmtList(List<Stmt> statements) {
        this.statements = statements;
    }

    public override Value exec(Scope _) {
        Value result = new Value();
        foreach (Stmt s in statements) {
            result = s.exec(_);
        }
        return result;
    }

    public override string ToString() {
        return string.Join("; ", this.statements);
    }
}