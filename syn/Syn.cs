// Syntactic analyzer
public static class Syn {

    public static Expr parse(Token[] tokens, int len) {
        if (len <= 0) throw new Syn.Error("Can't parse empty token list", 0);
        return parseExpr(tokens, 0, len);
    }

    private static Expr parseExpr(Token[] tokens, int start, int len) {
        Token firstToken = tokens[start];
        Token lastToken = tokens[start + len - 1];

        // find candidate UnOp, BinOp and TernOp to parse
        // (based on parentheses and BinOp priority)
        int ternOpFirstIndex = -1;
        int ternOpSecondIndex = -1;
        int ternOpLevel = 0;
        int binOpIndex = -1;
        int binOpPriority = Int32.MaxValue;
        int unOpIndex = -1;

        int firstOparIndex = -1;
        int parLevel = 0;
        for (int i = start; i < start+len; i++) {
            Token t = tokens[i];

            if (t.kind == Token.Kind.OPAR && parLevel == 0) firstOparIndex = i;

            if (t.kind == Token.Kind.OPAR) parLevel++;
            else if (t.kind == Token.Kind.CPAR) parLevel--;
            else if (parLevel == 0 && isTernOpFirst(t)) {
                ternOpLevel++;
                if (ternOpFirstIndex < 0) ternOpFirstIndex = i;
            }
            else if (
                parLevel == 0 && isTernOpSecond(t) 
                && ternOpSecondIndex < 0 && ternOpFirstIndex >= 0
            ) {
                ternOpLevel--;
                if (ternOpLevel == 0) ternOpSecondIndex = i;
            }
            else if (parLevel == 0 && isBinOp(t)) {
                // find BinOp at parentheses level 0 and lowest priority
                int newPriority = BinOp.priorityFor(t);
                if (newPriority <= binOpPriority) {
                    binOpIndex = i;
                    binOpPriority = newPriority;
                }
            }
            else if (parLevel == 0 && unOpIndex < 0 && isUnOp(t)) {
                // find first UnOp at parentheses level 0
                unOpIndex = i;
            }

            if (parLevel < 0) throw new Syn.Error(
                "Parenthesis never opened", t.position
            );
        }

        if (parLevel > 0) throw new Syn.Error(
            "Parenthesis never closed", tokens[firstOparIndex].position
        );

        if (ternOpFirstIndex >= 0 && ternOpSecondIndex >= 0) {
            return parseTernOp(ternOpFirstIndex, ternOpSecondIndex, tokens, start, len);
        }

        if (binOpIndex >= 0) 
            return parseBinOp(binOpIndex, tokens, start, len);
        if (unOpIndex >= 0)
            return parseUnOp(unOpIndex, tokens, start, len);

        // parentheses removal
        if (firstToken.kind == Token.Kind.OPAR &&
            lastToken.kind  == Token.Kind.CPAR
        ) return parseExpr(tokens, start + 1, len - 2);
        
        // parse Literal
        if (len == 1) {
            if (firstToken.kind == Token.Kind.NUMBER)
                return new Literal(firstToken.value);
            else if (firstToken.kind == Token.Kind.ID)
                return new Identifier(firstToken.raw);
            else
                throw new Syn.Error("Expected a literal", firstToken.position);
        }

        throw new Syn.Error("Could not parse", firstToken.position);
    }

    private static UnOp parseUnOp(int unOpIndex, Token[] tokens, int start, int len) {
        Token unOpToken = tokens[unOpIndex];
        if (len < 2)
            throw new Syn.Error("Unary operator misses its agument", unOpToken.position);
        if (unOpIndex != start)
            if (tokens[start].kind == Token.Kind.NUMBER)
                throw new Syn.Error("Unexpected token", tokens[start+1].position);
            else
                throw new Syn.Error("Unexpected token", tokens[start].position);

        Expr arg = parseExpr(tokens, start + 1, len - 1);
        return new UnOp(UnOp.kindFromToken(unOpToken), arg);
    }

    private static BinOp parseBinOp(int binOpIndex, Token[] tokens, int start, int len) {
        Token binOpToken = tokens[binOpIndex];
        if (binOpIndex == start || binOpIndex == start+len-1)
            throw new Syn.Error(
                "Binary operator misses an argument", binOpToken.position
            );

        int lhsLen = binOpIndex - start;
        int rhsLen = len - (lhsLen + 1);
        Expr lhs = parseExpr(tokens, start, lhsLen);
        Expr rhs = parseExpr(tokens, binOpIndex + 1, rhsLen);
        return new BinOp(BinOp.kindFromToken(binOpToken), lhs, rhs);
    }

    private static TernOp parseTernOp(int firstIndex, int secondIndex, Token[] tokens, int start, int len) {
        int arg0Len = firstIndex - start;
        int arg1Len = secondIndex - (firstIndex + 1);
        int arg2Len = len - (arg0Len + arg1Len + 2);
        Expr arg0 = parseExpr(tokens, start, arg0Len);
        Expr arg1 = parseExpr(tokens, firstIndex + 1, arg1Len);
        Expr arg2 = parseExpr(tokens, secondIndex + 1, arg2Len);
        return new TernOp(TernOp.Kind.CONDITIONAL, arg0, arg1, arg2);
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
            case Token.Kind.CARET:
            case Token.Kind.EQUALS:
            case Token.Kind.OPAR_ANG:
            case Token.Kind.CPAR_ANG:
            case Token.Kind.AND:
            case Token.Kind.OR:
            return true;
        }
        return false;
    }

    private static bool isTernOpFirst(Token t) {
        return t.kind == Token.Kind.QUESTION_MARK;
    }

    private static bool isTernOpSecond(Token t) {
        return t.kind == Token.Kind.COLON;
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