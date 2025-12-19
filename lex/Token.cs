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

    public static Token fromString(string raw, int position) {
        Kind kind = Kind.NUMBER;
        // should convert this switch to a static map
        // to avoid duplication with Lex.SYMBOLS
        switch (raw) {
            case "+":     kind = Kind.PLUS_SIGN; break;
            case "plus":  kind = Kind.PLUS; break;
            case "-":     kind = Kind.DASH; break;
            case "minus": kind = Kind.MINUS; break;
            case "*":     kind = Kind.ASTERISK; break;
            case "times": kind = Kind.TIMES; break;
            case "/":     kind = Kind.SLASH; break;
            case "by":    kind = Kind.BY; break;
            case "^":     kind = Kind.CARET; break;
            case "log":   kind = Kind.LOG; break;
            case "sin":   kind = Kind.SIN; break;
            case "(":     kind = Kind.OPAR; break;
            case ")":     kind = Kind.CPAR; break;
        }
        if (kind != Kind.NUMBER) return new Token(kind, position, raw);

        double value;
        bool isNumber = Double.TryParse(raw, out value);
        if (isNumber) return new Token(position, raw, value);

        throw new Lex.Error("Token.fromString is unimplemented for '" + raw + "'", position);
    }

    public override string ToString() {
        if (kind == Kind.NUMBER) return kind + "(" + value + ")";
        return "" + kind;
    }
}