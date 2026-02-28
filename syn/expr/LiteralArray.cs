public class LiteralArray : Expr {
    private List<Expr> expressions;

    public LiteralArray(List<Expr> expressions) {
        this.expressions = expressions;
    }

    public Value eval(Scope _) {
        Value array = Value.array(expressions.Count);
        for (int i = 0; i < expressions.Count; i++) {
            Value element = expressions[i].eval(_);
            if (element.kind != Value.Kind.Number)
                throw new Exception("array elements must be numbers");
            array.arr[i] = element.num;
        }
        return array;
    }

    public override string ToString() {
        return "LITERAL_ARRAY["+expressions.Count+"]";
    }
}
