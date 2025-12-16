// Analizzatore sintattico
public static class Syn {

    public static Expr parse(Token[] tokens, int len) {
        return parseExpr(tokens, 0, len);
    }

    private static Expr parseExpr(Token[] tokens, int start, int len) {
        Token firstToken = tokens[start];

        // Valori letterali
        if (len == 1) {
            if (firstToken.kind == Token.Kind.NUMBER)
                return new Literal(firstToken.value);
            else
                throw new Syn.Error("Expected a literal", firstToken.position);
        }

        // cerca operatori binari e unari
        int binOpIndex = -1, binOpPriority = -1, unOpIndex = -1;
        for (int i = start; i < start+len; i++) {
            if (isBinOp(tokens[i])) {
                int newPriority = BinOp.priorityFor(tokens[i]);
                if (newPriority > binOpPriority) {
                    binOpIndex = i;
                    binOpPriority = newPriority;
                }
            }
            if (unOpIndex < 0 && isUnOp(tokens[i])) unOpIndex = i;
        }

        // Operatori binari
        if (binOpIndex >= 0) {
            if (binOpIndex == start || binOpIndex == start+len-1)
                throw new Syn.Error("Binary operator misses an argument", tokens[binOpIndex].position);

            Expr lhs = parseExpr(tokens, start, binOpIndex - start);
            Expr rhs = parseExpr(tokens, binOpIndex + 1, start + len - binOpIndex - 1);
            return new BinOp(BinOp.kindFromToken(tokens[binOpIndex]), lhs, rhs);
        }

        // Operatori unari
        if (unOpIndex >= 0) {
            if (unOpIndex != start)
                throw new Syn.Error("Unary operator expected here", tokens[start].position);
            if (len < 2)
                throw new Syn.Error("Unary operator misses its agument", tokens[unOpIndex].position);

            Expr arg = parseExpr(tokens, start + 1, len - 1);
            return new UnOp(UnOp.kindFromToken(tokens[unOpIndex]), arg);
        }
        
        throw new Syn.Error("Could not parse", firstToken.position);
    }

    private static bool isUnOp(Token t) {
        switch (t.kind) {
            case Token.Kind.LOG:
            case Token.Kind.SIN:
            return true;
        }
        return false;
    }

    private static bool isBinOp(Token t) {
        switch (t.kind) {
            case Token.Kind.PLUS_SIGN:
            case Token.Kind.PLUS:
            case Token.Kind.DASH:
            case Token.Kind.MINUS:
            case Token.Kind.SLASH:
            case Token.Kind.BY:
            case Token.Kind.ASTERISK:
            case Token.Kind.TIMES:
            case Token.Kind.TICK:
            return true;
        }
        return false;
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