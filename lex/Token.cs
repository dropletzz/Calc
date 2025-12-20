public class Token {

    public enum Kind {
        NUMBER, OPAR, CPAR,
        PLUS_SIGN, DASH, ASTERISK, SLASH, CARET,
        PLUS, BY, TIMES, MINUS, LOG, SIN
    }

    public readonly Kind kind;
    public readonly int position; // where the token begins in the input string
    public readonly string raw;
    public readonly double value; // value is only assigned for Kind.NUMBER

    public Token(Kind kind, int position, string raw) {
        this.kind = kind;
        this.position = position;
        this.raw = raw;
    }

    public Token(int position, string raw, double value) {
        this.kind = Kind.NUMBER;
        this.position = position;
        this.raw = raw;
        this.value = value;
    }

    public override string ToString() {
        if (kind == Kind.NUMBER) return kind + "(" + value + ")";
        return kind.ToString();
    }
}