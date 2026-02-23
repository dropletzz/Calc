// Syntactic analyzer
public static class Syn {

    public static Stmt parse(Token[] tokens, int len) {
        if (len <= 0) throw new Syn.Error("Can't parse empty token list", 0);
        return parseStmtList(tokens, 0, len);
    }

    // Statements have to be separated with ';' with the exception of the last one (in each Block)
    private static StmtList parseStmtList(Token[] tokens, int start, int len) {
        List<Stmt> statements = new List<Stmt>();

        int cursor = start;
        int stmtStart = start;
        int blockLevel = 0;
        int blockOparIndex = -1;
        while (cursor < start + len) {
            Token curToken = tokens[cursor];

            if (curToken.kind == Token.Kind.OPAR_CURLY) blockLevel++;
            else if (curToken.kind == Token.Kind.CPAR_CURLY) blockLevel--;
            else if (curToken.kind == Token.Kind.SEMICOLON && blockLevel == 0) {
                int stmtLen = cursor - stmtStart;
                if (stmtLen > 0) {
                    Stmt s = parseStmt(tokens, stmtStart, stmtLen);
                    statements.Add(s);
                }
                stmtStart = cursor + 1;
            }

            if (curToken.kind == Token.Kind.OPAR_CURLY
                && blockOparIndex < 0) blockOparIndex = cursor;
            if (blockLevel < 0) throw new Syn.Error("Block never opened", curToken.position);

            cursor++;
        }

        if (blockLevel > 0) throw new Syn.Error("Block never closed", tokens[blockOparIndex].position);

        int unparsedLen = start + len - stmtStart;
        if (unparsedLen > 0) {
            Stmt s = parseStmt(tokens, stmtStart, unparsedLen);
            statements.Add(s);
        }

        return new StmtList(statements);
    }

    // Block is different from StmtList because it creates a child Scope during execution
    private static Block parseBlock(Token[] tokens, int start, int len) {
        StmtList statements = parseStmtList(tokens, start, len);
        return new Block(statements);
    }

    private static Stmt parseStmt(Token[] tokens, int start, int len) {
        Token firstToken = tokens[start];

        // Assignment
        if (len > 2 && firstToken.kind == Token.Kind.ID
        &&  tokens[start + 1].kind == Token.Kind.ASSIGN) {
            Identifier id = new Identifier(firstToken.raw);
            Expr assignee = parseExpr(tokens, start + 2, len - 2);
            return new Assignment(id, assignee);
        }

        // IndexedAssignment
        if (len > 1 && firstToken.kind == Token.Kind.OPAR_SQUARE
        ) {
            // TODO this is very much duplicated (see parseExpr IndexedArray)
            int cparIndex = start;
            while (cparIndex < start + len
                && tokens[cparIndex].kind != Token.Kind.CPAR_SQUARE
            ) cparIndex++;

            if (cparIndex >= start + len)
                throw new Syn.Error("missing ']'", tokens[start + len - 1].position);
            if (cparIndex + 1 < start + len
                && tokens[cparIndex + 1].kind != Token.Kind.ID
            ) throw new Syn.Error("Should be an identifier", tokens[cparIndex + 1].position);

            int indExprLen = cparIndex - start - 1;
            IndexedArray idx = new IndexedArray(
                parseExpr(tokens, start + 1, indExprLen), // index
                new Identifier(tokens[cparIndex + 1].raw)
            );

            if (cparIndex + 2 < start+len
                && tokens[cparIndex + 2].kind == Token.Kind.ASSIGN
            ) {
                int assStart = cparIndex + 3;
                int assLength = start + len - assStart;
                Expr assignee = parseExpr(tokens, assStart, assLength);
                return new IndexedAssignment(idx, assignee);
            }
        }

        // Print
        if (len > 1 && firstToken.kind == Token.Kind.PRINT) {
            Expr expr = parseExpr(tokens, start + 1, len - 1);
            return new Print(expr);
        }

        // ArrayDecl
        if (len == 4
            && tokens[start].kind == Token.Kind.ID
            && tokens[start+1].kind == Token.Kind.OPAR_SQUARE
            && tokens[start+2].kind == Token.Kind.NUMBER
            && tokens[start+3].kind == Token.Kind.CPAR_SQUARE
        ) {
            Identifier id = new Identifier(tokens[start].raw);
            int capacity = (int)tokens[start+2].value;
            return new ArrayDecl(capacity, id);
        }

        // While
        if (len > 0 && firstToken.kind == Token.Kind.WHILE) {
            int cursor = start + 1;

            while (
                cursor < start + len
                && tokens[cursor].kind != Token.Kind.OPAR_CURLY
            ) cursor++;

            if (cursor == start)
                throw new Syn.Error("Missing condition after while", tokens[cursor].position);
            if (cursor == start + len) 
                throw new Syn.Error("Missing '{' after condition", tokens[cursor - 1].position);

            int blockStart = cursor;
            Expr cond = parseExpr(tokens, start + 1, cursor - start - 1);

            if (tokens[start + len - 1].kind != Token.Kind.CPAR_CURLY)
                throw new Syn.Error("Open block never closed", tokens[blockStart].position);

            int blockLen = start + len - blockStart - 2;
            Block body = parseBlock(tokens, blockStart + 1, blockLen);

            return new While(cond, body);
        }

        // Block
        if (
            len > 1 && tokens[start].kind == Token.Kind.OPAR_CURLY
            && tokens[start + len - 1].kind == Token.Kind.CPAR_CURLY
        ) return parseBlock(tokens, start + 1, len - 2);

        // ExprStmt
        Expr exprStmt = parseExpr(tokens, start, len);
        return new ExprStmt(exprStmt);
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
                return new Literal(Value.number(firstToken.value));
            else if (firstToken.kind == Token.Kind.ID)
                return new Identifier(firstToken.raw);
            else
                throw new Syn.Error("Expected a literal", firstToken.position);
        }

        throw new Syn.Error("Could not parse", firstToken.position);
    }

    private static Expr parseUnOp(int unOpIndex, Token[] tokens, int start, int len) {
        Token unOpToken = tokens[unOpIndex];
        if (len < 2)
            throw new Syn.Error("Unary operator misses its agument", unOpToken.position);
        if (unOpIndex != start)
            if (tokens[start].kind == Token.Kind.NUMBER)
                throw new Syn.Error("Unexpected token", tokens[start+1].position);
            else
                throw new Syn.Error("Unexpected token", tokens[start].position);

        // IndexedArray
        if (unOpToken.kind == Token.Kind.OPAR_SQUARE) {
            int cparIndex = start;
            while (cparIndex < start + len
                && tokens[cparIndex].kind != Token.Kind.CPAR_SQUARE
            ) cparIndex++;

            if (cparIndex >= start + len)
                throw new Syn.Error("missing ']'", tokens[start + len - 1].position);
            if (cparIndex + 1 < start + len
                && tokens[cparIndex + 1].kind != Token.Kind.ID
            ) throw new Syn.Error("Should be an identifier", tokens[cparIndex + 1].position);
            if (cparIndex + 2 < start + len)
                throw new Syn.Error("Unexpected token", tokens[cparIndex + 2].position);

            int indExprLen = cparIndex - start - 1;
            Expr indExpr = parseExpr(tokens, start + 1, indExprLen);
            Identifier id = new Identifier(tokens[cparIndex + 1].raw);
            return new IndexedArray(indExpr, id);
        }

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
            case Token.Kind.NEG:
            case Token.Kind.OPAR_SQUARE:
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