// Lexical analyzer
public static class Lex {

    public static readonly string[] SYMBOLS = {
        "+", "plus", "-", "minus", "*", "times", "/", "by", "?", "%", ":=", ":", ";",
        "^", "log", "sin", "neg", "not", "(", ")", "==", "=", "<", ">", "and", "or", "print",
        "while", "{", "}", "[", "]", "if", "else", "len", "getc", "putc", ","
    };

    public static readonly int MAX_TOKENS_LENGTH = 1024 * 1024;

    private static int position = 0;
    private static int line = 0;
    private static int lastLinePosition = 0;

    public static Token[] tokenize(string s, out int tokensLength) {
        position = 0;
        line = 0;
        lastLinePosition = 0;

        Token[] tokens = new Token[MAX_TOKENS_LENGTH];
        tokensLength = 0;

        while (position < s.Length) {
            chompWhitespace(s);

            bool found = false;
            if (tokensLength >= MAX_TOKENS_LENGTH)
                throw new Lex.Error("Too many tokens, max is " + MAX_TOKENS_LENGTH, new Location(line, linePosition()));

            // look for an identifier
            {
                int startPosition = position;
                int startLinePosition = linePosition();
                int startLine = line;
                chompIdentifier(s);

                if (position > startPosition) {
                    string rawToken = s.Substring(startPosition, position - startPosition);
                    if (SYMBOLS.Contains(rawToken)) {
                        position = startPosition;
                        line = startLine;
                        found = false;
                    } else {
                        tokens[tokensLength] = parseIdentifier(rawToken, new Location(startLine, startLinePosition));
                        tokensLength++;
                        found = true;
                        continue;
                    }
                }
            }

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
                    tokens[tokensLength] = parseSymbol(rawToken, new Location(line, linePosition()));
                    tokensLength++;
                    position += sym.Length;
                    break;
                }
            }

            // look for a number
            if (!found) {
                int startPosition = position;
                int startLinePosition = linePosition();
                int startLine = line;
                chompDigits(s);
                chompChar('.', s);
                chompDigits(s);

                if (position > startPosition) {
                    string rawToken = s.Substring(startPosition, position - startPosition);
                    tokens[tokensLength] = parseNumber(rawToken, new Location(startLine, startLinePosition));
                    tokensLength++;
                    found = true;
                }
            }

            if (!found) throw new Lex.Error("Couldn't parse token", new Location(line, linePosition()));
            chompWhitespace(s);
        }

        return tokens;
    }

    private static Token parseNumber(string rawToken, Location loc) {
        double value;
        bool isNumber = Double.TryParse(rawToken, out value);
        if (!isNumber) throw new Lex.Error("Couldn't parse number", loc);
        return Token.number(rawToken, value, loc);
    }

    private static Token parseIdentifier(string rawToken, Location loc) {
        return Token.identifier(rawToken, loc);
    }

    private static Token parseSymbol(string rawToken, Location loc) {
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
            case "==":    kind = Token.Kind.DOUBLE_EQUALS; break;
            case "%":     kind = Token.Kind.PERCENT; break;
            case "<":     kind = Token.Kind.OPAR_ANG; break;
            case ">":     kind = Token.Kind.CPAR_ANG; break;
            case "?":     kind = Token.Kind.QUESTION_MARK; break;
            case ":":     kind = Token.Kind.COLON; break;
            case ";":     kind = Token.Kind.SEMICOLON; break;
            case "and":   kind = Token.Kind.AND; break;
            case "or":    kind = Token.Kind.OR; break;
            case "neg":   kind = Token.Kind.NEG; break;
            case "not":   kind = Token.Kind.NOT; break;
            case "{":    kind = Token.Kind.OPAR_CURLY; break;
            case "}":    kind = Token.Kind.CPAR_CURLY; break;
            case "[":    kind = Token.Kind.OPAR_SQUARE; break;
            case "]":    kind = Token.Kind.CPAR_SQUARE; break;
            case "if":    kind = Token.Kind.IF; break;
            case "else":    kind = Token.Kind.ELSE; break;
            case "len":    kind = Token.Kind.LEN; break;
            case "print":    kind = Token.Kind.PRINT; break;
            case "while":    kind = Token.Kind.WHILE; break;
            case "getc":    kind = Token.Kind.GETC; break;
            case "putc":    kind = Token.Kind.PUTC; break;
            case ",":    kind = Token.Kind.COMMA; break;
            default: throw new Lex.Error("parseSymbol undefined for '" + rawToken + "'", loc);
        }

        return Token.symbol(kind, rawToken, loc);
    }


    private static bool isDigit(char c) {
        return c >= '0' && c <= '9';
    }

    private static bool isAlpha(char c) {
        return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
    }

    private static bool isWhitespace(char c) {
        return c == ' ' || c == '\t' || c == '\n' || c == '\r';
    }

    private static bool isAlphanum(char c) {
        return isDigit(c) || isAlpha(c);
    }

    // this function feels like a bad hack
    private static void checkNewline(string s) {
        bool crlf = false;
        char c = s[position];
        int newLineLen = 0;
        if (c == '\n') newLineLen = 1;
        else if (c == '\r') {
            crlf = position + 1 < s.Length && s[position + 1] == '\n';
            newLineLen = crlf ? 2 : 1;
        }

        if (newLineLen > 0) {
            line++;
            lastLinePosition = position + newLineLen;
            if (crlf) position++;
        }
    }

    private static void chompChar(char c, string s) {
        if (position < s.Length && s[position] == c) {
            checkNewline(s);
            position++;
        }
    }

    private static void chompWhitespace(string s) {
        while (
            position < s.Length
            && isWhitespace(s[position])
        ) {
            checkNewline(s);
            position++;
        }
    }

    private static void chompDigits(string s) {
        while (
            position < s.Length
            && isDigit(s[position])
        ) {
            checkNewline(s);
            position++;
        }
    }

    private static void chompIdentifier(string s) {
        if (!isAlpha(s[position])) return;
        while (
            position < s.Length
            && isAlphanum(s[position])
        ) {
            checkNewline(s);
            position++;
        }
    }

    private static int linePosition() {
        return position - lastLinePosition;
    }

    public class Error : Exception {
        public readonly string message;
        public readonly Location loc;

        public Error(string message, Location loc) {
            this.message = message;
            this.loc = loc;
        }
    }
}