public class Block : Stmt {
    public List<Stmt> statements;

    public Block(List<Stmt> statements) {
        this.statements = statements;
    }

    public override double exec(Scope _) {
        Scope blockScope = new Scope(_);
        double result = 0;
        foreach (Stmt s in statements) {
            result = s.exec(blockScope);
        }
        return result;
    }

    public override string ToString() {
        string res = "{ ";
        foreach (Stmt s in statements) {
            res += s.ToString() + "; ";
        }
        return res + "}";
    }
}