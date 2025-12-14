// Operatore unario (con 1 argomento)
public class UnOp : Expr {
    
    public enum Kind {
        LOG, SIN
    }

    private Kind kind;
    private Expr arg;

    public UnOp(Kind kind, Expr arg) {
        this.kind = kind;
        this.arg = arg;
    }

    public double eval() {
        switch (kind) {
            case Kind.LOG: return Math.Log(arg.eval());
            case Kind.SIN: return Math.Sin(arg.eval());
        }
        throw new Exception("UNIMPLEMENTED: eval for UnOp.Kind" + kind);
    }

    public static Kind kindFromSymbol(string s) {
        switch (s) {
            case "log": return Kind.LOG;
            case "sin": return Kind.SIN;
        }
        throw new Exception("UNIMPLEMENTED: UnOp.kindFromSymbol for \"" + s + "\"");
    }

    public override string ToString() {
        return kind + "(" + arg.ToString() + ")";
    }
}