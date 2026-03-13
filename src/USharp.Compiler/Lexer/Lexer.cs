namespace USharp.Compiler.Lexer;

public sealed class Lexer
{
    private readonly string _source;
    private int _pos;
    private int _line;
    private int _column;
    private readonly Stack<int> _indentStack = new([0]);

    private static readonly Dictionary<string, TokenKind> Keywords = new(StringComparer.Ordinal)
    {
        ["module"] = TokenKind.Module,
        ["class"] = TokenKind.Class,
        ["fn"] = TokenKind.Fn,
        ["if"] = TokenKind.If,
        ["else"] = TokenKind.Else,
        ["each"] = TokenKind.Each,
        ["for"] = TokenKind.For,
        ["while"] = TokenKind.While,
        ["in"] = TokenKind.In,
        ["return"] = TokenKind.Return,
        ["print"] = TokenKind.Print,
        ["send"] = TokenKind.Send,
        ["filter"] = TokenKind.Filter,
        ["map"] = TokenKind.Map,
        ["sort"] = TokenKind.Sort,
        ["and"] = TokenKind.And,
        ["or"] = TokenKind.Or,
        ["not"] = TokenKind.Not,
        ["empty"] = TokenKind.Empty,
        ["retain"] = TokenKind.Retain,
        ["true"] = TokenKind.BoolLiteral,
        ["false"] = TokenKind.BoolLiteral,
        ["var"] = TokenKind.Var,
    };

    /// <summary>
    /// The set of all known U-Sharp keyword strings. Used by the parser for
    /// directive typo detection (USP1001) so that the list is defined in one place.
    /// </summary>
    public static IReadOnlySet<string> KeywordNames { get; } =
        new HashSet<string>(Keywords.Keys, StringComparer.Ordinal);

    public Lexer(string source)
    {
        _source = source;
        _pos = 0;
        _line = 1;
        _column = 1;
    }

    public List<Token> Tokenize()
    {
        var tokens = new List<Token>();
        bool atLineStart = true;
        int currentIndent = 0;

        while (_pos < _source.Length)
        {
            if (atLineStart)
            {
                currentIndent = 0;
                while (_pos < _source.Length && (_source[_pos] == ' ' || _source[_pos] == '\t'))
                {
                    currentIndent += _source[_pos] == '\t' ? 4 : 1;
                    _pos++;
                    _column++;
                }

                // Skip blank lines
                if (_pos < _source.Length && (_source[_pos] == '\n' || _source[_pos] == '\r'))
                {
                    ConsumeNewLine(tokens);
                    atLineStart = true;
                    continue;
                }
                // Skip comment lines
                if (_pos < _source.Length && (_source[_pos] == '#' ||
                    (_pos + 1 < _source.Length && _source[_pos] == '-' && _source[_pos + 1] == '-')))
                {
                    while (_pos < _source.Length && _source[_pos] != '\n' && _source[_pos] != '\r')
                    {
                        _pos++; _column++;
                    }
                    if (_pos < _source.Length) ConsumeNewLine(tokens);
                    atLineStart = true;
                    continue;
                }
                if (_pos >= _source.Length) break;

                int topIndent = _indentStack.Peek();
                if (currentIndent > topIndent)
                {
                    _indentStack.Push(currentIndent);
                    tokens.Add(new Token(TokenKind.Indent, "", _line, 1));
                }
                else if (currentIndent < topIndent)
                {
                    while (_indentStack.Peek() > currentIndent)
                    {
                        _indentStack.Pop();
                        tokens.Add(new Token(TokenKind.Dedent, "", _line, 1));
                    }
                }

                atLineStart = false;
            }

            if (_pos >= _source.Length) break;

            char c = _source[_pos];

            if (c == '\n' || c == '\r')
            {
                ConsumeNewLine(tokens);
                atLineStart = true;
                continue;
            }

            if (c == ' ' || c == '\t')
            {
                _pos++; _column++;
                continue;
            }

            // Inline comments
            if (c == '#')
            {
                while (_pos < _source.Length && _source[_pos] != '\n' && _source[_pos] != '\r')
                {
                    _pos++; _column++;
                }
                continue;
            }
            if (c == '-' && _pos + 1 < _source.Length && _source[_pos + 1] == '-')
            {
                while (_pos < _source.Length && _source[_pos] != '\n' && _source[_pos] != '\r')
                {
                    _pos++; _column++;
                }
                continue;
            }

            int tokLine = _line;
            int tokCol = _column;

            if (c == '"')
            {
                tokens.Add(ReadString(tokLine, tokCol));
                continue;
            }

            if (char.IsDigit(c))
            {
                tokens.Add(ReadNumber(tokLine, tokCol));
                continue;
            }

            if (char.IsLetter(c) || c == '_')
            {
                tokens.Add(ReadIdentifierOrKeyword(tokLine, tokCol));
                continue;
            }

            switch (c)
            {
                case '.':
                    if (_pos + 1 < _source.Length && _source[_pos + 1] == '.')
                    {
                        tokens.Add(new Token(TokenKind.DotDot, "..", tokLine, tokCol));
                        _pos += 2; _column += 2;
                    }
                    else
                    {
                        tokens.Add(new Token(TokenKind.Dot, ".", tokLine, tokCol));
                        _pos++; _column++;
                    }
                    break;
                case ',':
                    tokens.Add(new Token(TokenKind.Comma, ",", tokLine, tokCol));
                    _pos++; _column++;
                    break;
                case ':':
                    tokens.Add(new Token(TokenKind.Colon, ":", tokLine, tokCol));
                    _pos++; _column++;
                    break;
                case ';':
                    tokens.Add(new Token(TokenKind.Semicolon, ";", tokLine, tokCol));
                    _pos++; _column++;
                    break;
                case '(':
                    tokens.Add(new Token(TokenKind.LParen, "(", tokLine, tokCol));
                    _pos++; _column++;
                    break;
                case ')':
                    tokens.Add(new Token(TokenKind.RParen, ")", tokLine, tokCol));
                    _pos++; _column++;
                    break;
                case '{':
                    tokens.Add(new Token(TokenKind.LBrace, "{", tokLine, tokCol));
                    _pos++; _column++;
                    break;
                case '}':
                    tokens.Add(new Token(TokenKind.RBrace, "}", tokLine, tokCol));
                    _pos++; _column++;
                    break;
                case '[':
                    tokens.Add(new Token(TokenKind.LBracket, "[", tokLine, tokCol));
                    _pos++; _column++;
                    break;
                case ']':
                    tokens.Add(new Token(TokenKind.RBracket, "]", tokLine, tokCol));
                    _pos++; _column++;
                    break;
                case '+':
                    tokens.Add(new Token(TokenKind.Plus, "+", tokLine, tokCol));
                    _pos++; _column++;
                    break;
                case '-':
                    if (_pos + 1 < _source.Length && _source[_pos + 1] == '>')
                    {
                        tokens.Add(new Token(TokenKind.Arrow, "->", tokLine, tokCol));
                        _pos += 2; _column += 2;
                    }
                    else
                    {
                        tokens.Add(new Token(TokenKind.Minus, "-", tokLine, tokCol));
                        _pos++; _column++;
                    }
                    break;
                case '*':
                    tokens.Add(new Token(TokenKind.Star, "*", tokLine, tokCol));
                    _pos++; _column++;
                    break;
                case '/':
                    tokens.Add(new Token(TokenKind.Slash, "/", tokLine, tokCol));
                    _pos++; _column++;
                    break;
                case '%':
                    tokens.Add(new Token(TokenKind.Percent, "%", tokLine, tokCol));
                    _pos++; _column++;
                    break;
                case '=':
                    if (_pos + 1 < _source.Length && _source[_pos + 1] == '=')
                    {
                        tokens.Add(new Token(TokenKind.EqualsEquals, "==", tokLine, tokCol));
                        _pos += 2; _column += 2;
                    }
                    else
                    {
                        tokens.Add(new Token(TokenKind.Equals, "=", tokLine, tokCol));
                        _pos++; _column++;
                    }
                    break;
                case '!':
                    if (_pos + 1 < _source.Length && _source[_pos + 1] == '=')
                    {
                        tokens.Add(new Token(TokenKind.BangEquals, "!=", tokLine, tokCol));
                        _pos += 2; _column += 2;
                    }
                    else
                    {
                        tokens.Add(new Token(TokenKind.Bang, "!", tokLine, tokCol));
                        _pos++; _column++;
                    }
                    break;
                case '<':
                    if (_pos + 1 < _source.Length && _source[_pos + 1] == '=')
                    {
                        tokens.Add(new Token(TokenKind.LtEquals, "<=", tokLine, tokCol));
                        _pos += 2; _column += 2;
                    }
                    else
                    {
                        tokens.Add(new Token(TokenKind.Lt, "<", tokLine, tokCol));
                        _pos++; _column++;
                    }
                    break;
                case '>':
                    if (_pos + 1 < _source.Length && _source[_pos + 1] == '=')
                    {
                        tokens.Add(new Token(TokenKind.GtEquals, ">=", tokLine, tokCol));
                        _pos += 2; _column += 2;
                    }
                    else
                    {
                        tokens.Add(new Token(TokenKind.Gt, ">", tokLine, tokCol));
                        _pos++; _column++;
                    }
                    break;
                default:
                    _pos++; _column++;
                    break;
            }
        }

        // Emit remaining DEDENTs
        while (_indentStack.Peek() > 0)
        {
            _indentStack.Pop();
            tokens.Add(new Token(TokenKind.Dedent, "", _line, _column));
        }

        tokens.Add(new Token(TokenKind.EndOfFile, "", _line, _column));
        return tokens;
    }

    private void ConsumeNewLine(List<Token> tokens)
    {
        if (_pos < _source.Length && _source[_pos] == '\r') { _pos++; _column++; }
        if (_pos < _source.Length && _source[_pos] == '\n') { _pos++; }
        tokens.Add(new Token(TokenKind.NewLine, "", _line, _column));
        _line++;
        _column = 1;
    }

    private Token ReadString(int line, int col)
    {
        _pos++; _column++; // skip opening "
        var sb = new System.Text.StringBuilder();
        sb.Append('"');
        while (_pos < _source.Length && _source[_pos] != '"')
        {
            if (_source[_pos] == '\\' && _pos + 1 < _source.Length)
            {
                sb.Append(_source[_pos]);
                sb.Append(_source[_pos + 1]);
                _pos += 2; _column += 2;
            }
            else
            {
                sb.Append(_source[_pos]);
                _pos++; _column++;
            }
        }
        if (_pos < _source.Length) { sb.Append('"'); _pos++; _column++; }
        return new Token(TokenKind.StringLiteral, sb.ToString(), line, col);
    }

    private Token ReadNumber(int line, int col)
    {
        var sb = new System.Text.StringBuilder();
        bool isFloat = false;
        while (_pos < _source.Length && char.IsDigit(_source[_pos]))
        {
            sb.Append(_source[_pos]); _pos++; _column++;
        }
        if (_pos < _source.Length && _source[_pos] == '.' &&
            (_pos + 1 >= _source.Length || _source[_pos + 1] != '.'))
        {
            isFloat = true;
            sb.Append('.'); _pos++; _column++;
            while (_pos < _source.Length && char.IsDigit(_source[_pos]))
            {
                sb.Append(_source[_pos]); _pos++; _column++;
            }
        }
        return new Token(isFloat ? TokenKind.FloatLiteral : TokenKind.IntLiteral, sb.ToString(), line, col);
    }

    private Token ReadIdentifierOrKeyword(int line, int col)
    {
        var sb = new System.Text.StringBuilder();
        while (_pos < _source.Length && (char.IsLetterOrDigit(_source[_pos]) || _source[_pos] == '_'))
        {
            sb.Append(_source[_pos]); _pos++; _column++;
        }
        var text = sb.ToString();
        var kind = Keywords.TryGetValue(text, out var kw) ? kw : TokenKind.Identifier;
        return new Token(kind, text, line, col);
    }
}
