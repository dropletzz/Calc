// Syntactic analyzer
public static class Syn {

    public static Stmt parse(Token[] tokens, int len) {
        if (len <= 0) throw new Syn.Error("Can't parse empty token list", 0);
        return parseStmtList(tokens, 0, len);
    }

    // Statements have to be separated with ';' except for the last one of each block
    private static StmtList parseStmtList(Token[] tokens, int start, int len) {
        List<Stmt> statements = new List<Stmt>();

        int cursor = start;
        while (cursor < start + len) {
            if (tokens[cursor].kind == Token.Kind.SEMICOLON) {
                cursor++;
                continue;
            }

            int stmtStart = cursor;

            while (cursor < start + len && (
                tokens[cursor].kind != Token.Kind.SEMICOLON &&
                tokens[cursor].kind != Token.Kind.OPAR_CURLY
            )) cursor++;

            if (cursor < start + len
                && tokens[cursor].kind == Token.Kind.OPAR_CURLY
            ) {
                int remainingLen = start + len - cursor;
                cursor = 1 + nextMatching(
                    Token.Kind.OPAR_CURLY, Token.Kind.CPAR_CURLY,
                    tokens, cursor, remainingLen
                );
                remainingLen = start + len - cursor;
                cursor = next(Token.Kind.SEMICOLON, tokens, cursor, remainingLen);
            }

            int stmtLen = cursor - stmtStart;
            Stmt s = parseStmt(tokens, stmtStart, stmtLen);
            statements.Add(s);

            cursor++;
        }

        return new StmtList(statements);
    }

    private static Stmt parseStmt(Token[] tokens, int start, int len) {
        Token firstToken = tokens[start];

        if (firstToken.kind == Token.Kind.PUTC) {
            Expr expr = parseExpr(tokens, start + 1, len - 1);
            return new Putc(expr);
        }

        if (firstToken.kind == Token.Kind.GETC) {
            if (len < 2) throw new Syn.Error("missing argument", tokens[start].position);

            if (tokens[start + 1].kind == Token.Kind.ID) {
                string name = tokens[start + 1].raw;
                return new Getc(new Identifier(name));
            }

            IndexedArray targetArr = parseIndexedArray(tokens, start + 1, len - 1);
            return new Getc(targetArr);
        }

        // Assignment
        if (len > 2 && firstToken.kind == Token.Kind.ID
        &&  tokens[start + 1].kind == Token.Kind.EQUALS) {
            Identifier id = new Identifier(firstToken.raw);
            Expr assignee = parseExpr(tokens, start + 2, len - 2);
            return new Assignment(id, assignee);
        }

        // IndexedAssignment
        if (len > 1 && firstToken.kind == Token.Kind.OPAR_SQUARE
        ) {
            int assignIndex = start;
            while (assignIndex < start+len && tokens[assignIndex].kind != Token.Kind.EQUALS) assignIndex++;
            if (assignIndex < start + len) {
                if (assignIndex == start+len-1) throw new Syn.Error("Missing assignee", tokens[len-1].position);

                int idxArrayLen = assignIndex - start;
                IndexedArray idx = parseIndexedArray(tokens, start, idxArrayLen);

                int assStart = assignIndex + 1;
                int assLen = start + len - assStart;
                Expr assignee = parseExpr(tokens, assStart, assLen);

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
            int cursor = next(Token.Kind.OPAR_CURLY, tokens, start, len);

            if (cursor >= start + len) 
                throw new Syn.Error("Missing block after condition", tokens[cursor - 1].position);
            if (tokens[start + len - 1].kind != Token.Kind.CPAR_CURLY)
                throw new Syn.Error("Open block never closed", tokens[cursor].position);

            Expr cond = parseExpr(tokens, start + 1, cursor - start - 1);

            int blockStart = cursor + 1;
            int blockLen = start + len - blockStart - 1;
            StmtList statements = parseStmtList(tokens, blockStart, blockLen);
            Block body = new Block(statements);
            return new While(cond, body);
        }

        // Block
        if (
            len > 1 && tokens[start].kind == Token.Kind.OPAR_CURLY
            && tokens[start + len - 1].kind == Token.Kind.CPAR_CURLY
        ) {
            StmtList statements = parseStmtList(tokens, start + 1, len - 2);
            return new Block(statements);
        }

        // IfElse
        if (len > 0 && firstToken.kind == Token.Kind.IF)
            return parseIfElse(tokens, start, len);


        // ExprStmt
        Expr exprStmt = parseExpr(tokens, start, len);
        return new ExprStmt(exprStmt);
    }

    private static IfElse parseIfElse(Token[] tokens, int start, int len) {
        int blockOpar = next(Token.Kind.OPAR_CURLY, tokens, start, len);
        if (blockOpar >= start + len)
            throw new Syn.Error("Missing block after condition", tokens[blockOpar - 1].position);

        Expr cond = parseExpr(tokens, start + 1, blockOpar - start - 1);

        int blockCpar = nextMatching(
            Token.Kind.OPAR_CURLY, Token.Kind.CPAR_CURLY,
            tokens, blockOpar, start + len - blockOpar
        );
        int stmtListStart = blockOpar + 1;
        int stmtListLen = blockCpar - stmtListStart;
        Block ifTrue = new Block(
            parseStmtList(tokens, stmtListStart, stmtListLen)
        );
        if (blockCpar + 1 == start + len)
            return new IfElse(cond, ifTrue, null);

        if (tokens[blockCpar + 1].kind != Token.Kind.ELSE)
            throw new Syn.Error("expected else or ;", tokens[blockCpar + 1].position);
        if (blockCpar + 2 >= start + len)
            throw new Syn.Error("else is incomplete", tokens[blockCpar + 1].position);
        if (tokens[blockCpar + 2].kind != Token.Kind.OPAR_CURLY
            && tokens[blockCpar + 2].kind != Token.Kind.IF
        ) throw new Syn.Error("expected if or {", tokens[blockCpar + 2].position);

        if (tokens[blockCpar + 2].kind == Token.Kind.OPAR_CURLY) {
            blockOpar = blockCpar + 2;
            blockCpar = nextMatching(
                Token.Kind.OPAR_CURLY, Token.Kind.CPAR_CURLY,
                tokens, blockOpar, start + len - blockOpar
            );
            stmtListStart = blockOpar + 1;
            stmtListLen = blockCpar - stmtListStart;
            Block ifFalse = new Block(
                parseStmtList(tokens, stmtListStart, stmtListLen)
            );
            return new IfElse(cond, ifTrue, ifFalse);
        }
        else if (tokens[blockCpar + 2].kind == Token.Kind.IF) {
            int elseIfStart = blockCpar + 2;
            int elseIfLen = start + len - elseIfStart;
            IfElse elseIf = parseIfElse(tokens, elseIfStart, elseIfLen);
            return new IfElse(cond, ifTrue, elseIf);
        }
        throw new Syn.Error("unexpected token", tokens[blockCpar + 2].position);
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

            if (t.kind == Token.Kind.OPAR_SQUARE) {
                i = nextMatching(
                    Token.Kind.OPAR_SQUARE, Token.Kind.CPAR_SQUARE,
                    tokens, start, len
                );
                i++;
            }

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

        if (binOpIndex >= 0) {
            return parseBinOp(binOpIndex, tokens, start, len);
        }

        if (unOpIndex >= 0) {
            return parseUnOp(unOpIndex, tokens, start, len);
        }

        if (firstToken.kind == Token.Kind.OPAR_SQUARE) {
            return parseIndexedArray(tokens, start, len);
        }

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

    private static IndexedArray parseIndexedArray(Token[] tokens, int start, int len) {
        if (tokens[start].kind != Token.Kind.OPAR_SQUARE)
            throw new Syn.Error("IndexedArray must start with '['", tokens[start].position);

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
            case Token.Kind.NEG:
            case Token.Kind.LEN:
            case Token.Kind.NOT:
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
            case Token.Kind.PERCENT:
            case Token.Kind.DOUBLE_EQUALS:
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

    private static int next(Token.Kind kind, Token[] tokens, int start, int len) {
        int cursor = start;
        while (
            cursor < start + len
            && tokens[cursor].kind != kind
        ) cursor++;
        return cursor;
    }

    private static int nextMatching(Token.Kind begin, Token.Kind end, Token[] tokens, int start, int len) {
        int parLevel = 0;
        int cursor = start;
        while (cursor < start + len) {
            if (tokens[cursor].kind == begin) parLevel++;
            else if (tokens[cursor].kind == end) parLevel--;

            if (parLevel == 0 && tokens[cursor].kind == end) break;
            else if (parLevel < 0) 
                throw new Syn.Error("never opened", tokens[cursor].position);
            cursor++;
        }
        if (parLevel > 0)
            throw new Syn.Error("never closed", tokens[start].position);

        return cursor;
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