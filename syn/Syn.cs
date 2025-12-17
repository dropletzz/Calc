// Syntactic analyzer
public static class Syn {

    public static Expr parse(Token[] tokens, int len) {
        return parseExpr(tokens, 0, len);
    }

    private static Expr parseExpr(Token[] tokens, int start, int len) {
        Token firstToken = tokens[start];

        // parse Literal
        if (len == 1) {
            if (firstToken.kind == Token.Kind.NUMBER)
                return new Literal(firstToken.value);
            else
                throw new Syn.Error("Expected a literal", firstToken.position);
        }

        // FIRST PASS
        // check that parentheses are balanced
        Token lastToken = tokens[start+len-1];
        int parLevel = 0;
        int firstOparIndex = -1;
        if (firstToken.kind == Token.Kind.OPAR) {
            for (int i = start; i < start+len; i++) {
                Token t = tokens[i];
                if (t.kind == Token.Kind.OPAR && parLevel == 0)
                    firstOparIndex = i;

                if (t.kind == Token.Kind.OPAR) parLevel++;
                else if (t.kind == Token.Kind.CPAR) parLevel--;

                if (parLevel < 0) throw new Syn.Error(
                    "Parenthesis never opened", t.position
                );
            }
            if (parLevel > 0) throw new Syn.Error(
                "Parenthesis never closed", tokens[firstOparIndex].position
            );

            // parentheses removal
            if (lastToken.kind == Token.Kind.CPAR)
                return parseExpr(tokens, start + 1, len - 2);
        }

        // SECOND PASS
        // find candidate UnOp and BinOp to parse
        // based on parentheses and BinOp priority
        int binOpIndex = -1, binOpPriority = -1, binOpParLevel = Int32.MaxValue;
        int unOpIndex = -1, unOpParLevel = Int32.MaxValue;
        parLevel = 0;
        for (int i = start; i < start+len; i++) {
            Token t = tokens[i];
            if (t.kind == Token.Kind.OPAR) parLevel++;
            else if (t.kind == Token.Kind.CPAR) parLevel--;
            else if (isUnOp(t) && parLevel < unOpParLevel) {
                // find UnOp at lowest parentheses level
                unOpIndex = i;
                unOpParLevel = parLevel;
            }
            else if (isBinOp(t) && parLevel <= binOpParLevel) {
                // find BinOp at lowest parentheses level
                // if parentheses level is the same, find at lowest proprity
                int newPriority = BinOp.priorityFor(t);

                if (parLevel < binOpParLevel || newPriority < binOpPriority) {
                    binOpIndex = i;
                    binOpParLevel = parLevel;
                    binOpPriority = newPriority;
                }
            }
        }

        // parse either BinOp or UnOp
        bool mustParseUnOp = unOpIndex >= 0 && unOpParLevel < binOpParLevel;
        if (binOpIndex >= 0 && !mustParseUnOp) 
            return parseBinOp(binOpIndex, tokens, start, len);
        if (unOpIndex >= 0)
            return parseUnOp(unOpIndex, tokens, start, len);
        
        throw new Syn.Error("Could not parse", firstToken.position);
    }

    private static UnOp parseUnOp(int unOpIndex, Token[] tokens, int start, int len) {
        Token unOpToken = tokens[unOpIndex];
        if (unOpIndex != start)
            throw new Syn.Error("Unary operator expected here", tokens[start].position);
        if (len < 2)
            throw new Syn.Error("Unary operator misses its agument", unOpToken.position);

        Expr arg = parseExpr(tokens, start + 1, len - 1);
        return new UnOp(UnOp.kindFromToken(unOpToken), arg);
    }

    private static BinOp parseBinOp(int binOpIndex, Token[] tokens, int start, int len) {
        Token binOpToken = tokens[binOpIndex];
        if (binOpIndex == start || binOpIndex == start+len-1)
            throw new Syn.Error(
                "Binary operator misses an argument", binOpToken.position
            );

        Expr lhs = parseExpr(tokens, start, binOpIndex - start);
        Expr rhs = parseExpr(tokens, binOpIndex + 1, start + len - binOpIndex - 1);
        return new BinOp(BinOp.kindFromToken(binOpToken), lhs, rhs);
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