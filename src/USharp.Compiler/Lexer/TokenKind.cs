namespace USharp.Compiler.Lexer;

public enum TokenKind
{
    // Special
    EndOfFile,
    NewLine,
    Indent,
    Dedent,

    // Literals
    Identifier,
    IntLiteral,
    FloatLiteral,
    StringLiteral,
    BoolLiteral,

    // Keywords
    Module,
    Class,
    Fn,
    If,
    Else,
    Each,
    For,
    While,
    In,
    Return,
    Print,
    Send,
    Filter,
    Map,
    Sort,
    And,
    Or,
    Not,
    Empty,
    Retain,
    Var,

    // Symbols
    Dot,
    Comma,
    Colon,
    Semicolon,
    LParen,
    RParen,
    LBrace,
    RBrace,
    LBracket,
    RBracket,
    DotDot,

    // Operators
    Plus,
    Minus,
    Star,
    Slash,
    Percent,
    Equals,
    EqualsEquals,
    BangEquals,
    Lt,
    LtEquals,
    Gt,
    GtEquals,
    Bang,
    Arrow,

    // Comment (kept for completeness, not emitted)
    Comment,
}
