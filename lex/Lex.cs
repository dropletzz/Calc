using System.Reflection;

// Lexical analyzer
public static class Lex {

    public static readonly string[] SYMBOLS = typeof(Symbols)
        .GetFields(BindingFlags.Public | BindingFlags.Static)
        .Where(f => f.FieldType == typeof(string))
        .Select(f => (string)f.GetValue(null)!)
        .OrderByDescending(s => s.Length)
        .ToArray();

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

        chompWhitespace(s);

        while (position < s.Length) {
            bool found = false;
            if (tokensLength >= MAX_TOKENS_LENGTH)
                throw new Lex.Error("Too many tokens, max is " + MAX_TOKENS_LENGTH, new Location(line, linePosition()));

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
                    tokens[tokensLength] = Token.symbol(
                        rawToken, new Location(line, linePosition())
                    );
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

            // look for an identifier
            if (!found) {
                int startPosition = position;
                int startLinePosition = linePosition();
                int startLine = line;
                chompIdentifier(s);

                if (position > startPosition) {
                    string rawToken = s.Substring(startPosition, position - startPosition);
                    tokens[tokensLength] = Token.identifier(
                        rawToken, new Location(startLine, startLinePosition)
                    );
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

    private static void checkNewline(string s) {
        char c = s[position];
        int newLineLen = 0;
        if (c == '\n') newLineLen = 1;
        else if (c == '\r') {
            bool crlf = position + 1 < s.Length && s[position + 1] == '\n';
            newLineLen = crlf ? 2 : 1;
        }

        if (newLineLen > 0) {
            line++;
            lastLinePosition = position + newLineLen;
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