public class Token {

    public enum Kind {
        NUMBER, ID, OPAR, CPAR, OPAR_ANG, CPAR_ANG,
        PLUS_SIGN, DASH, ASTERISK, SLASH, CARET, QUESTION_MARK, COLON,
        PLUS, BY, TIMES, MINUS, LOG, SIN, EQUALS, AND, OR
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