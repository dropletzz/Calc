public class Block : Stmt {
    public List<Stmt> statements;

    public Block(List<Stmt> statements, Scope _) : base(_) {
        this.statements = statements;
    }

    public override double exec() {
        double result = 0;
        foreach (Stmt s in statements) {
            result = s.exec();
        }
        return result;
    }

    public override string ToString() {
        string res = "";
        foreach (Stmt s in statements) {
            res += s.ToString() + '\n';
        }
        return res;
    }
}