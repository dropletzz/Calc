
public class Token {
    public enum Kind {
        NUMBER, PLUS_SIGN, PLUS, DASH, MINUS, SLASH, BY, ASTERISK, TIMES, TICK, LOG, SIN
    }
    public readonly Kind kind;
    public readonly int position; // where the token begins in the input string
    public readonly string raw;
    public readonly double value;

    public Token(Kind kind, int position, string raw) {
        this.kind = kind;
        this.position = position;
        this.raw = raw;
    }

    public Token(Kind kind, int position, string raw, double value) {
        this.kind = kind;
        this.position = position;
        this.raw = raw;
        this.value = value;
    }

    public static Token fromString(string raw, int position) {
        Kind kind = Kind.NUMBER;
        switch (raw) {
            case "+":     kind = Kind.PLUS_SIGN; break;
            case "plus":  kind = Kind.PLUS; break;
            case "-":     kind = Kind.DASH; break;
            case "minus": kind = Kind.MINUS; break;
            case "*":     kind = Kind.ASTERISK; break;
            case "times": kind = Kind.TIMES; break;
            case "/":     kind = Kind.SLASH; break;
            case "by":    kind = Kind.BY; break;
            case "^":     kind = Kind.TICK; break;
            case "log":   kind = Kind.LOG; break;
            case "sin":   kind = Kind.SIN; break;
        }
        if (kind != Kind.NUMBER) return new Token(kind, position, raw);

        double value;
        bool isNumber = Double.TryParse(raw, out value);
        if (isNumber) return new Token(Kind.NUMBER, position, raw, value);

        throw new Lex.Error("UNIMPLEMENTED: Token.fromString for '" + raw + "'", position);
    }

    public override string ToString() {
        if (kind == Kind.NUMBER) return kind + "(" + value + ")";
        return "" + kind;
    }
}