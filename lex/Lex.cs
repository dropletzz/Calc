
public static class Lex {
    public static readonly string[] SYMBOLS = { "+", "plus", "-", "minus", "*", "times", "/", "by", "^", "log", "sin", "(", ")" };

    public static Token[] tokenize(string s, out int tokensLength) {
        int position = 0;
        Token[] tokens = new Token[1024];
        tokensLength = 0;

        position = chompWhitespace(s, position);

        while (position < s.Length) {
            bool found = false;

            // parse a symbol
            foreach (string sym in SYMBOLS) {
                found = true;
                for (int c = 0; c < sym.Length; c++) {
                    if (s[position + c] != sym[c]) {
                        found = false;
                        break;
                    }
                }
                if (found) {
                    string rawToken = s.Substring(position, sym.Length);
                    tokens[tokensLength] = Token.fromString(rawToken, position);
                    tokensLength++;
                    position += sym.Length;
                    break;
                }
            }

            // parse number
            if (!found) {
                int startPosition = position;
                position = chompDigits(s, position);
                position = chompChar('.', s, position);
                position = chompDigits(s, position);

                if (position > startPosition) {
                    string rawToken = s.Substring(startPosition, position - startPosition);
                    tokens[tokensLength] = Token.fromString(rawToken, startPosition);
                    tokensLength++;
                    found = true;
                }
            }

            if (!found) throw new Error("Couldn't parse token", position);

            position = chompWhitespace(s, position);
        }

        return tokens;
    }

    public static bool isDigit(char c) {
        return c >= '0' && c <= '9';
    }

    public static bool isWhitespace(char c) {
        return c == ' '; // the only whitespace allowed is spaces
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

    public class Error : Exception {
        public readonly string message;
        public readonly int position;

        public Error(string message, int position) {
            this.message = message;
            this.position = position;
        }
    }
}