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

        Token lastToken = tokens[start+len-1];
        if (firstToken.kind == Token.Kind.OPAR
            && lastToken.kind == Token.Kind.CPAR
        ) return parseExpr(tokens, start + 1, len - 2);

        int binOpIndex = -1, binOpPriority = -1, binOpParLevel = 1000;
        int unOpIndex = -1, unOpParLevel = 1000;
        int parLevel = 0, firstOparIndex = -1;
        for (int i = start; i < start+len; i++) {
            if (tokens[i].kind == Token.Kind.OPAR && firstOparIndex < 0) firstOparIndex = i;

            if (tokens[i].kind == Token.Kind.OPAR) parLevel++;
            else if (tokens[i].kind == Token.Kind.CPAR) parLevel--;
            else if (isUnOp(tokens[i]) && parLevel < unOpParLevel) {
                // find unop at lowest parLevel
                unOpIndex = i;
                unOpParLevel = parLevel;
            }
            else if (isBinOp(tokens[i])) {
                // find binop at lowest parLevel
                // if parLevel is the same find at lowest proprity
                int newPriority = BinOp.priorityFor(tokens[i]);

                if (parLevel < binOpParLevel || 
                    (parLevel == binOpParLevel &&
                    newPriority < binOpPriority)
                ) {
                    binOpIndex = i;
                    binOpParLevel = parLevel;
                    binOpPriority = newPriority;
                }
            }

            if (parLevel < 0) throw new Syn.Error("Parenthesis never opened", tokens[i].position);
        }
        if (parLevel > 0) throw new Syn.Error("Parenthesis never closed", tokens[firstOparIndex].position);

        // Operatori binari
        if (binOpIndex >= 0 && !(unOpIndex >= 0 && unOpParLevel < binOpParLevel)) {
            if (binOpIndex == start || binOpIndex == start+len-1)
                throw new Syn.Error("Binary operator misses an argument", tokens[binOpIndex].position);

            Expr lhs = parseExpr(tokens, start, binOpIndex - start);
            Expr rhs = parseExpr(tokens, binOpIndex + 1, start + len - binOpIndex - 1);
            return new BinOp(BinOp.kindFromToken(tokens[binOpIndex]), lhs, rhs);
        }

        // Operatori unari
        if (unOpIndex >= 0) {
            if (unOpIndex != start)
                throw new Syn.Error("Unary operator expected here", firstToken.position);
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