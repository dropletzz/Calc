public class Block : Stmt {
    public StmtList statements;

    public Block(List<Stmt> statements) {
        this.statements = new StmtList(statements);
    }

    public Block(StmtList statements) {
        this.statements = statements;
    }

    public override Value exec(Scope _) {
        Scope blockScope = new Scope(_);
        Value result = this.statements.exec(blockScope);
        blockScope.exit();
        return result;
    }

    public override string ToString() {
        return "{ " + this.statements + " }";
    }
}