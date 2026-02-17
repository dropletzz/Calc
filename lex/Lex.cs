// Lexical analyzer
public static class Lex {

    public static readonly string[] SYMBOLS = {
        "+", "plus", "-", "minus", "*", "times", "/", "by", "?", ":",
        "^", "log", "sin", "(", ")", "=", "<", ">", "and", "or"
    };

    public static readonly int MAX_TOKENS_LENGTH = 1024;

    public static Token[] tokenize(string s, out int tokensLength) {
        int position = 0;
        Token[] tokens = new Token[MAX_TOKENS_LENGTH];
        tokensLength = 0;

        position = chompWhitespace(s, position);

        while (position < s.Length) {
            bool found = false;
            if (tokensLength >= MAX_TOKENS_LENGTH)
                throw new Lex.Error("Too many tokens, max is " + MAX_TOKENS_LENGTH, position);

            // look for a symbol
            foreach (string sym in SYMBOLS) {
                if (sym.Length > s.Length - position) continue;
                found = true;
                for (int c = 0; c < sym.Length; c++) {
                    if (s[position + c] != sym[c]) {
                        found = false;
                        break;
                    }
                }
                if (found) {
                    string rawToken = s.Substring(position, sym.Length);
                    tokens[tokensLength] = parseSymbol(rawToken, position);
                    tokensLength++;
                    position += sym.Length;
                    break;
                }
            }

            // look for a number
            if (!found) {
                int startPosition = position;
                position = chompDigits(s, position);
                position = chompChar('.', s, position);
                position = chompDigits(s, position);

                if (position > startPosition) {
                    string rawToken = s.Substring(startPosition, position - startPosition);
                    tokens[tokensLength] = parseNumber(rawToken, startPosition);
                    tokensLength++;
                    found = true;
                }
            }

            // look for an identifier
            if (!found) {
                int startPosition = position;
                position = chompIdentifier(s, position);

                if (position > startPosition) {
                    string rawToken = s.Substring(startPosition, position - startPosition);
                    tokens[tokensLength] = parseIdentifier(rawToken, startPosition);
                    tokensLength++;
                    found = true;
                }
            }

            if (!found) throw new Error("Couldn't parse token", position);

            position = chompWhitespace(s, position);
        }

        return tokens;
    }

    public static Token parseNumber(string rawToken, int position) {
        double value;
        bool isNumber = Double.TryParse(rawToken, out value);
        if (!isNumber) throw new Lex.Error("Couldn't parse number", position);
        return new Token(position, rawToken, value);
    }

    public static Token parseIdentifier(string rawToken, int position) {
        return new Token(Token.Kind.ID, position, rawToken);
    }

    public static Token parseSymbol(string rawToken, int position) {
        Token.Kind kind;
        switch (rawToken) {
            case "+":     kind = Token.Kind.PLUS_SIGN; break;
            case "plus":  kind = Token.Kind.PLUS; break;
            case "-":     kind = Token.Kind.DASH; break;
            case "minus": kind = Token.Kind.MINUS; break;
            case "*":     kind = Token.Kind.ASTERISK; break;
            case "times": kind = Token.Kind.TIMES; break;
            case "/":     kind = Token.Kind.SLASH; break;
            case "by":    kind = Token.Kind.BY; break;
            case "^":     kind = Token.Kind.CARET; break;
            case "log":   kind = Token.Kind.LOG; break;
            case "sin":   kind = Token.Kind.SIN; break;
            case "(":     kind = Token.Kind.OPAR; break;
            case ")":     kind = Token.Kind.CPAR; break;
            case "=":     kind = Token.Kind.EQUALS; break;
            case "<":     kind = Token.Kind.OPAR_ANG; break;
            case ">":     kind = Token.Kind.CPAR_ANG; break;
            case "?":     kind = Token.Kind.QUESTION_MARK; break;
            case ":":     kind = Token.Kind.COLON; break;
            case "and":   kind = Token.Kind.AND; break;
            case "or":    kind = Token.Kind.OR; break;
            default: throw new Lex.Error("parseSymbol undefined for '" + rawToken + "'", position);
        }

        return new Token(kind, position, rawToken);
    }


    public static bool isDigit(char c) {
        return c >= '0' && c <= '9';
    }

    public static bool isAlpha(char c) {
        return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
    }

    public static bool isWhitespace(char c) {
        // only spaces and tabs are considered whitespace
        return c == ' ' || c == '\t';
    }

    public static bool isAlphanum(char c) {
        return isDigit(c) || isAlpha(c);
    }

    public static int chompChar(char c, string s, int from) {
        if (from < s.Length && s[from] == c) from++;
        return from;
    }

    public static int chompWhitespace(string s, int from) {
        while (
            from < s.Length
            && isWhitespace(s[from])
        ) from++;
        return from;
    }

    public static int chompDigits(string s, int from) {
        while (
            from < s.Length
            && isDigit(s[from])
        ) from++;
        return from;
    }

    public static int chompIdentifier(string s, int from) {
        if (!isAlpha(s[from])) return from;
        while (
            from < s.Length
            && isAlphanum(s[from])
        ) from++;
        return from;
    }

    public class Error : Exception {
        public readonly string message;
        public readonly int position;

        public Error(string message, int position) {
            this.message = message;
            this.position = position;
        }
    }
}