public class Block : Stmt {
    public List<Stmt> statements;

    public Block(List<Stmt> statements) {
        this.statements = statements;
    }

    public override Value exec(Scope _) {
        Scope blockScope = new Scope(_);
        Value result = new Value();
        foreach (Stmt s in statements) {
            result = s.exec(blockScope);
            blockScope.clearTempMemory();
        }
        blockScope.clearMemory();
        return result;
    }

    public override string ToString() {
        return "{ " + string.Join("; ", this.statements) + " }";
    }
}