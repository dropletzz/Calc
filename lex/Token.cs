public readonly struct Token {

    public enum Kind {
        NUMBER, ID, SYMBOL
    }

    public readonly Kind kind;
    public readonly Location loc; // location of the token in the source code
    public readonly string raw;
    public readonly double value; // value is only assigned for Kind.NUMBER

    private Token(Kind kind, string raw, double value, Location loc) {
        this.kind = kind;
        this.loc = loc;
        this.raw = raw;
        this.value = value;
    }

    public static Token symbol(string raw, Location loc) {
        return new Token(Kind.SYMBOL, raw, 0, loc);
    }

    public static Token identifier(string raw, Location loc) {
        return new Token(Kind.ID, raw, 0, loc);
    }

    public static Token number(string raw, double value, Location loc) {
        return new Token(Kind.NUMBER, raw, value, loc);
    }

    public override string ToString() {
        if (kind == Kind.NUMBER) return kind + "(" + value + ")";
        return kind.ToString();
    }
}