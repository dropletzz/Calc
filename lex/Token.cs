public readonly struct Token {

    public enum Kind {
        NUMBER, ID, WHILE,
        OPAR, CPAR, OPAR_ANG, CPAR_ANG, OPAR_CURLY, CPAR_CURLY, OPAR_SQUARE, CPAR_SQUARE,
        PLUS_SIGN, DASH, ASTERISK, SLASH, CARET, QUESTION_MARK, COLON, SEMICOLON,
        PLUS, BY, TIMES, MINUS, LOG, SIN, NEG, EQUALS, AND, OR, NOT, PRINT,
        IF, ELSE, LEN, GETC, PUTC, DOUBLE_EQUALS, PERCENT, COMMA
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

    public static Token symbol(Token.Kind kind, string raw, Location loc) {
        return new Token(kind, raw, 0, loc);
    }

    public static Token identifier(string raw, Location loc) {
        return new Token(Token.Kind.ID, raw, 0, loc);
    }

    public static Token number(string raw, double value, Location loc) {
        return new Token(Token.Kind.NUMBER, raw, value, loc);
    }

    public override string ToString() {
        if (kind == Kind.NUMBER) return kind + "(" + value + ")";
        return kind.ToString();
    }
}