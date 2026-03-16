using System.Reflection;

public static class Lexeme {
    // UnOp
    public static readonly string LOG = "log";
    public static readonly string SIN = "sin";
    public static readonly string NEG = "neg";
    public static readonly string NOT = "not";
    public static readonly string LEN = "len";

    // BinOp
    public static readonly string PLUS_SIGN = "+";
    public static readonly string PLUS = "plus";
    public static readonly string DASH = "-";
    public static readonly string MINUS = "minus";
    public static readonly string ASTERISK = "*";
    public static readonly string TIMES = "times";
    public static readonly string SLASH = "/";
    public static readonly string BY = "by";
    public static readonly string CARET = "^";
    public static readonly string DOUBLE_EQUALS = "==";
    public static readonly string AND = "and";
    public static readonly string OR = "or";
    public static readonly string PERCENT = "%";
    public static readonly string OPAR_ANG = "<";
    public static readonly string CPAR_ANG = ">";

    // TernOp
    public static readonly string QUESTION_MARK = "?";
    public static readonly string COLON = ":";

    // Stmt
    public static readonly string EQUALS = "=";
    public static readonly string IF = "if";
    public static readonly string ELSE = "else";
    public static readonly string PRINT = "print";
    public static readonly string WHILE = "while";
    public static readonly string GETC = "getc";
    public static readonly string PUTC = "putc";

    // Other
    public static readonly string OPAR = "(";
    public static readonly string CPAR = ")";
    public static readonly string OPAR_SQUARE = "[";
    public static readonly string CPAR_SQUARE = "]";
    public static readonly string OPAR_CURLY = "{";
    public static readonly string CPAR_CURLY = "}";
    public static readonly string COMMA = ",";
    public static readonly string SEMICOLON = ";";

    public static readonly string[] allLexemes = typeof(Lexeme)
        .GetFields(BindingFlags.Public | BindingFlags.Static)
        .Where(f => f.FieldType == typeof(string))
        .Select(f => (string)f.GetValue(null)!)
        .ToArray();

    // If you strictly want a method instead of a field:
    public static string[] all() => allLexemes;
}