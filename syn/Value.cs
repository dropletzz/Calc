public readonly struct Value {
    public enum Kind {
        Number,
        Array,
        Nil
    }

    public readonly Kind kind;
    public readonly double num;
    public readonly List<double>? arr;

    private Value(Kind kind, double num, List<double>? arr) {
        this.kind = kind;
        this.num = num;
        this.arr = arr;
    }

    public static Value nil() {
        return new Value(Kind.Nil, 0, null);
    }

    public static Value number(double num) {
        return new Value(Kind.Number, num, null);
    }

    public static Value array(List<double> arr) {
        return new Value(Kind.Array, 0, arr);
    }

    public override string ToString() {
        if (kind == Kind.Number) return num.ToString();
        if (arr != null && kind == Kind.Array) return "[" + string.Join(", ", arr) + "]"; 
        return "Nil";
    }
}