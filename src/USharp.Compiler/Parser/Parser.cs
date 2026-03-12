namespace USharp.Compiler.Parser;

using USharp.Compiler.Ast;
using USharp.Compiler.Lexer;
using USharp.Compiler.Diagnostics;

public sealed class Parser
{
    private readonly List<Token> _tokens;
    private int _pos;
    private readonly List<Diagnostic> _diagnostics = new();

    public IReadOnlyList<Diagnostic> Diagnostics => _diagnostics;

    public Parser(List<Token> tokens)
    {
        _tokens = tokens;
        _pos = 0;
    }

    private Token Current => _pos < _tokens.Count ? _tokens[_pos] : _tokens[^1];
    private Token Peek(int offset = 1) => (_pos + offset) < _tokens.Count ? _tokens[_pos + offset] : _tokens[^1];

    private Token Consume()
    {
        var t = Current;
        _pos++;
        return t;
    }

    private Token Expect(TokenKind kind)
    {
        if (Current.Kind == kind)
            return Consume();
        _diagnostics.Add(Diagnostic.Error(
            DiagnosticCode.USP1005,
            $"Expected {kind} but got {Current.Kind} ('{Current.Text}')",
            Current.Line, Current.Column));
        return new Token(kind, "", Current.Line, Current.Column);
    }

    private void SkipNewLines()
    {
        while (Current.Kind == TokenKind.NewLine)
            Consume();
    }

    public ProgramNode Parse()
    {
        var loc = Current.ToLocation();
        var decls = new List<AstNode>();
        SkipNewLines();
        while (Current.Kind != TokenKind.EndOfFile)
        {
            var decl = ParseTopLevel();
            if (decl != null) decls.Add(decl);
            SkipNewLines();
        }
        return new ProgramNode(decls, loc);
    }

    private AstNode? ParseTopLevel()
    {
        switch (Current.Kind)
        {
            case TokenKind.Module:
                return ParseModule();
            case TokenKind.Class:
                return ParseClass();
            case TokenKind.Fn:
                return ParseFn();
            default:
                return ParseStatement();
        }
    }

    private ModuleDecl ParseModule()
    {
        var loc = Current.ToLocation();
        Consume(); // module
        var nameParts = new System.Text.StringBuilder();
        nameParts.Append(Expect(TokenKind.Identifier).Text);
        while (Current.Kind == TokenKind.Dot)
        {
            Consume(); // consume '.'
            nameParts.Append('.');
            nameParts.Append(Expect(TokenKind.Identifier).Text);
        }
        SkipNewLines();
        return new ModuleDecl(nameParts.ToString(), loc);
    }

    private ClassDecl ParseClass()
    {
        var loc = Current.ToLocation();
        Consume(); // class
        var name = Expect(TokenKind.Identifier).Text;
        SkipNewLines();
        var members = new List<AstNode>();
        if (Current.Kind == TokenKind.Indent)
        {
            Consume(); // indent
            SkipNewLines();
            while (Current.Kind != TokenKind.Dedent && Current.Kind != TokenKind.EndOfFile)
            {
                var member = ParseClassMember();
                if (member != null) members.Add(member);
                SkipNewLines();
            }
            if (Current.Kind == TokenKind.Dedent) Consume();
        }
        return new ClassDecl(name, members, loc);
    }

    private AstNode? ParseClassMember()
    {
        var loc = Current.ToLocation();
        if (Current.Kind == TokenKind.Fn)
        {
            return ParseFn();
        }
        if (Current.Kind == TokenKind.Identifier)
        {
            var name = Consume().Text;
            bool isReadonly = false;
            TypeExprNode? type = null;
            ExpressionNode? init = null;

            if (Current.Kind == TokenKind.Retain)
            {
                isReadonly = true;
                Consume();
                type = ParseType();
            }
            else if (Current.Kind == TokenKind.Empty)
            {
                Consume();
                type = ParseType();
            }
            else if (Current.Kind == TokenKind.Colon)
            {
                Consume();
                type = ParseType();
            }
            else
            {
                SkipToEndOfLine();
                return null;
            }

            if (Current.Kind == TokenKind.Equals)
            {
                Consume();
                init = ParseExpression();
            }

            SkipNewLines();
            return new FieldDecl(name, isReadonly, type, init, loc);
        }
        else
        {
            SkipToEndOfLine();
            return null;
        }
    }

    private void SkipToEndOfLine()
    {
        while (Current.Kind != TokenKind.NewLine && Current.Kind != TokenKind.EndOfFile &&
               Current.Kind != TokenKind.Dedent)
        {
            Consume();
        }
        if (Current.Kind == TokenKind.NewLine) Consume();
    }

    private FnDecl ParseFn()
    {
        var loc = Current.ToLocation();
        Consume(); // fn
        var name = Expect(TokenKind.Identifier).Text;
        var parameters = new List<ParameterNode>();
        TypeExprNode returnType = new TypeExprNode("void", [], loc);

        if (Current.Kind == TokenKind.LParen)
        {
            Consume(); // (
            while (Current.Kind != TokenKind.RParen && Current.Kind != TokenKind.EndOfFile)
            {
                var pLoc = Current.ToLocation();
                var pName = Expect(TokenKind.Identifier).Text;
                Expect(TokenKind.Colon);
                var pType = ParseType();
                parameters.Add(new ParameterNode(pName, pType, pLoc));
                if (Current.Kind == TokenKind.Comma) Consume();
            }
            Expect(TokenKind.RParen);
        }

        if (Current.Kind == TokenKind.Colon)
        {
            Consume();
            returnType = ParseType();
        }

        SkipNewLines();
        var body = ParseBlock();
        return new FnDecl(name, parameters, returnType, body, loc);
    }

    private List<StatementNode> ParseBlock()
    {
        var stmts = new List<StatementNode>();
        if (Current.Kind != TokenKind.Indent)
            return stmts;
        Consume(); // indent
        SkipNewLines();
        while (Current.Kind != TokenKind.Dedent && Current.Kind != TokenKind.EndOfFile)
        {
            var stmt = ParseStatement();
            if (stmt != null) stmts.Add(stmt);
            SkipNewLines();
        }
        if (Current.Kind == TokenKind.Dedent) Consume();
        return stmts;
    }

    private StatementNode? ParseStatement()
    {
        switch (Current.Kind)
        {
            case TokenKind.Return:
                return ParseReturn();
            case TokenKind.Print:
                return ParsePrint();
            case TokenKind.Send:
                return ParseSend();
            case TokenKind.If:
                return ParseIf();
            case TokenKind.Each:
                return ParseEach();
            case TokenKind.For:
                return ParseFor();
            case TokenKind.While:
                return ParseWhile();
            case TokenKind.NewLine:
                Consume();
                return null;
            case TokenKind.Identifier:
                return ParseIdentifierStatement();
            case TokenKind.EndOfFile:
            case TokenKind.Dedent:
                return null;
            default:
                return ParseExpressionStatement();
        }
    }

    private ReturnStatement ParseReturn()
    {
        var loc = Current.ToLocation();
        Consume(); // return
        ExpressionNode? value = null;
        if (Current.Kind != TokenKind.NewLine && Current.Kind != TokenKind.EndOfFile &&
            Current.Kind != TokenKind.Dedent)
        {
            value = ParseExpression();
        }
        SkipNewLines();
        return new ReturnStatement(value, loc);
    }

    private PrintStatement ParsePrint()
    {
        var loc = Current.ToLocation();
        Consume(); // print
        var value = ParseExpression();
        SkipNewLines();
        return new PrintStatement(value, loc);
    }

    private SendStatement ParseSend()
    {
        var loc = Current.ToLocation();
        Consume(); // send
        var target = Expect(TokenKind.Identifier).Text;
        SkipNewLines();
        return new SendStatement(target, loc);
    }

    private IfStatement ParseIf()
    {
        var loc = Current.ToLocation();
        Consume(); // if
        var condition = ParseExpression();
        SkipNewLines();
        var thenBlock = new BlockStatement(ParseBlock(), loc);
        SkipNewLines();
        BlockStatement? elseBlock = null;
        if (Current.Kind == TokenKind.Else)
        {
            Consume(); // else
            SkipNewLines();
            elseBlock = new BlockStatement(ParseBlock(), Current.ToLocation());
        }
        return new IfStatement(condition, thenBlock, elseBlock, loc);
    }

    private EachStatement ParseEach()
    {
        var loc = Current.ToLocation();
        Consume(); // each
        var variable = Expect(TokenKind.Identifier).Text;
        Expect(TokenKind.In);
        var collection = ParseExpression();
        SkipNewLines();
        var body = new BlockStatement(ParseBlock(), loc);
        return new EachStatement(variable, collection, body, loc);
    }

    private ForStatement ParseFor()
    {
        var loc = Current.ToLocation();
        Consume(); // for
        var variable = Expect(TokenKind.Identifier).Text;
        Expect(TokenKind.In);
        // Parse start (not full expression to avoid consuming ..)
        var start = ParseAdditive();
        Expect(TokenKind.DotDot);
        var end = ParseAdditive();
        SkipNewLines();
        var body = new BlockStatement(ParseBlock(), loc);
        return new ForStatement(variable, start, end, body, loc);
    }

    private WhileStatement ParseWhile()
    {
        var loc = Current.ToLocation();
        Consume(); // while
        var condition = ParseExpression();
        SkipNewLines();
        var body = new BlockStatement(ParseBlock(), loc);
        return new WhileStatement(condition, body, loc);
    }

    // Known directive names for USP1001 suggestions
    private static readonly string[] KnownDirectives =
        ["print", "send", "return", "if", "else", "while", "for", "each", "fn", "class", "module", "filter", "map", "sort"];

    private static string? SuggestDirective(string name)
    {
        // Simple edit-distance suggestion (Levenshtein ≤ 2)
        string? best = null;
        int bestDist = int.MaxValue;
        foreach (var directive in KnownDirectives)
        {
            int dist = EditDistance(name, directive);
            if (dist < bestDist)
            {
                bestDist = dist;
                best = directive;
            }
        }
        return bestDist <= 2 ? best : null;
    }

    private static int EditDistance(string a, string b)
    {
        int m = a.Length, n = b.Length;
        int[,] dp = new int[m + 1, n + 1];
        for (int i = 0; i <= m; i++) dp[i, 0] = i;
        for (int j = 0; j <= n; j++) dp[0, j] = j;
        for (int i = 1; i <= m; i++)
            for (int j = 1; j <= n; j++)
                dp[i, j] = a[i - 1] == b[j - 1]
                    ? dp[i - 1, j - 1]
                    : 1 + Math.Min(dp[i - 1, j - 1], Math.Min(dp[i - 1, j], dp[i, j - 1]));
        return dp[m, n];
    }

    private static bool IsDirectiveLikeToken(TokenKind kind) =>
        kind == TokenKind.StringLiteral || kind == TokenKind.IntLiteral ||
        kind == TokenKind.FloatLiteral || kind == TokenKind.BoolLiteral ||
        kind == TokenKind.Identifier;

    private StatementNode? ParseIdentifierStatement()
    {
        var loc = Current.ToLocation();
        int saved = _pos;
        var identName = Current.Text;
        _pos++; // skip identifier

        if (Current.Kind == TokenKind.Empty || Current.Kind == TokenKind.Retain)
        {
            _pos = saved;
            return ParseVarDecl();
        }
        else if (Current.Kind == TokenKind.Colon)
        {
            _pos = saved;
            return ParseVarDecl();
        }
        else if (Current.Kind == TokenKind.Equals && Peek().Kind != TokenKind.Equals)
        {
            _pos = saved;
            return ParseVarDecl();
        }
        else
        {
            _pos = saved;

            // Detect unknown directive: bare identifier followed immediately by a
            // non-operator token on the same line (e.g. `pront "hello"`).
            bool unknownDirectiveDetected = false;
            if (IsDirectiveLikeToken(Peek().Kind) &&
                !KnownDirectives.Contains(identName, StringComparer.Ordinal))
            {
                var suggestion = SuggestDirective(identName);
                var msg = $"Unknown directive '{identName}'.";
                if (suggestion != null) msg += $" Did you mean '{suggestion}'?";
                _diagnostics.Add(Diagnostic.Error(
                    DiagnosticCode.USP1001, msg, loc.Line, loc.Column,
                    suggestion != null ? suggestion : null));
                unknownDirectiveDetected = true;
            }

            var expr = ParseExpression();
            // If unknown directive was detected, consume the argument expression too
            _ = unknownDirectiveDetected;

            // Check for pipeline: NewLine + Indent + filter/map/sort
            int savedPos = _pos;
            SkipNewLines();
            if (Current.Kind == TokenKind.Indent)
            {
                int indentPos = _pos;
                Consume(); // indent
                SkipNewLines();
                if (Current.Kind == TokenKind.Filter || Current.Kind == TokenKind.Map ||
                    Current.Kind == TokenKind.Sort)
                {
                    var stages = new List<PipelineStage>();
                    while (Current.Kind != TokenKind.Dedent && Current.Kind != TokenKind.EndOfFile)
                    {
                        if (Current.Kind == TokenKind.NewLine) { Consume(); continue; }
                        if (Current.Kind != TokenKind.Filter && Current.Kind != TokenKind.Map &&
                            Current.Kind != TokenKind.Sort)
                            break;

                        var stageLoc = Current.ToLocation();
                        var op = Consume().Text;
                        ExpressionNode? arg = null;
                        if (Current.Kind != TokenKind.NewLine && Current.Kind != TokenKind.Dedent &&
                            Current.Kind != TokenKind.EndOfFile)
                        {
                            arg = ParseExpression();
                        }
                        stages.Add(new PipelineStage(op, arg, stageLoc));
                        SkipNewLines();
                    }
                    if (Current.Kind == TokenKind.Dedent) Consume();
                    return new PipelineStatement(expr, stages, loc);
                }
                else
                {
                    _pos = savedPos;
                }
            }
            else
            {
                _pos = savedPos;
            }

            SkipNewLines();
            return new ExpressionStatement(expr, loc);
        }
    }

    private VarDeclStatement ParseVarDecl()
    {
        var loc = Current.ToLocation();
        var name = Expect(TokenKind.Identifier).Text;
        bool isReadonly = false;
        TypeExprNode? type = null;
        ExpressionNode? init = null;

        if (Current.Kind == TokenKind.Empty)
        {
            Consume();
            type = ParseType();
        }
        else if (Current.Kind == TokenKind.Retain)
        {
            isReadonly = true;
            Consume();
            type = ParseType();
            if (Current.Kind == TokenKind.Equals)
            {
                Consume();
                init = ParseExpression();
            }
        }
        else if (Current.Kind == TokenKind.Colon)
        {
            Consume();
            type = ParseType();
            if (Current.Kind == TokenKind.Equals)
            {
                Consume();
                init = ParseExpression();
            }
        }
        else if (Current.Kind == TokenKind.Equals)
        {
            Consume();
            init = ParseExpression();
        }

        SkipNewLines();
        return new VarDeclStatement(name, isReadonly, type, init, loc);
    }

    private ExpressionStatement ParseExpressionStatement()
    {
        var loc = Current.ToLocation();
        var expr = ParseExpression();
        SkipNewLines();
        return new ExpressionStatement(expr, loc);
    }

    private TypeExprNode ParseType()
    {
        var loc = Current.ToLocation();
        string name;
        if (Current.Kind == TokenKind.Identifier || IsTypeToken(Current.Kind))
        {
            name = Consume().Text;
        }
        else
        {
            name = "object";
        }

        var typeArgs = new List<TypeExprNode>();
        if (Current.Kind == TokenKind.Lt)
        {
            Consume(); // <
            typeArgs.Add(ParseType());
            while (Current.Kind == TokenKind.Comma)
            {
                Consume();
                typeArgs.Add(ParseType());
            }
            if (Current.Kind == TokenKind.Gt) Consume();
        }

        return new TypeExprNode(name, typeArgs, loc);
    }

    private static bool IsTypeToken(TokenKind kind) => kind == TokenKind.Identifier;

    // Expression parsing with precedence
    private ExpressionNode ParseExpression() => ParseOr();

    private ExpressionNode ParseOr()
    {
        var left = ParseAnd();
        while (Current.Kind == TokenKind.Or)
        {
            var loc = Current.ToLocation();
            Consume();
            var right = ParseAnd();
            left = new BinaryExpr(left, "||", right, loc);
        }
        return left;
    }

    private ExpressionNode ParseAnd()
    {
        var left = ParseComparison();
        while (Current.Kind == TokenKind.And)
        {
            var loc = Current.ToLocation();
            Consume();
            var right = ParseComparison();
            left = new BinaryExpr(left, "&&", right, loc);
        }
        return left;
    }

    private ExpressionNode ParseComparison()
    {
        var left = ParseAdditive();
        while (Current.Kind is TokenKind.EqualsEquals or TokenKind.BangEquals or
               TokenKind.Lt or TokenKind.LtEquals or TokenKind.Gt or TokenKind.GtEquals)
        {
            var loc = Current.ToLocation();
            var op = Current.Text;
            Consume();
            var right = ParseAdditive();
            left = new BinaryExpr(left, op, right, loc);
        }
        return left;
    }

    private ExpressionNode ParseAdditive()
    {
        var left = ParseMultiplicative();
        while (Current.Kind is TokenKind.Plus or TokenKind.Minus)
        {
            var loc = Current.ToLocation();
            var op = Current.Text;
            Consume();
            var right = ParseMultiplicative();
            left = new BinaryExpr(left, op, right, loc);
        }
        return left;
    }

    private ExpressionNode ParseMultiplicative()
    {
        var left = ParseUnary();
        while (Current.Kind is TokenKind.Star or TokenKind.Slash or TokenKind.Percent)
        {
            var loc = Current.ToLocation();
            var op = Current.Text;
            Consume();
            var right = ParseUnary();
            left = new BinaryExpr(left, op, right, loc);
        }
        return left;
    }

    private ExpressionNode ParseUnary()
    {
        if (Current.Kind == TokenKind.Not || Current.Kind == TokenKind.Bang)
        {
            var loc = Current.ToLocation();
            Consume();
            return new UnaryExpr("!", ParseUnary(), loc);
        }
        if (Current.Kind == TokenKind.Minus)
        {
            var loc = Current.ToLocation();
            Consume();
            return new UnaryExpr("-", ParseUnary(), loc);
        }
        return ParsePostfix();
    }

    private ExpressionNode ParsePostfix()
    {
        var expr = ParsePrimary();
        while (true)
        {
            if (Current.Kind == TokenKind.Dot)
            {
                var loc = Current.ToLocation();
                Consume();
                var member = Expect(TokenKind.Identifier).Text;
                expr = new MemberAccessExpr(expr, member, loc);
            }
            else if (Current.Kind == TokenKind.LParen)
            {
                var loc = Current.ToLocation();
                Consume();
                var args = new List<ExpressionNode>();
                while (Current.Kind != TokenKind.RParen && Current.Kind != TokenKind.EndOfFile)
                {
                    args.Add(ParseExpression());
                    if (Current.Kind == TokenKind.Comma) Consume();
                }
                Expect(TokenKind.RParen);
                expr = new CallExpr(expr, args, loc);
            }
            else
            {
                break;
            }
        }
        return expr;
    }

    private ExpressionNode ParsePrimary()
    {
        var loc = Current.ToLocation();
        switch (Current.Kind)
        {
            case TokenKind.IntLiteral:
            {
                var t = Consume();
                return new LiteralExpr(int.Parse(t.Text), t.Text, loc);
            }
            case TokenKind.FloatLiteral:
            {
                var t = Consume();
                return new LiteralExpr(double.Parse(t.Text, System.Globalization.CultureInfo.InvariantCulture),
                    t.Text, loc);
            }
            case TokenKind.StringLiteral:
            {
                var t = Consume();
                return new LiteralExpr(t.Text, t.Text, loc);
            }
            case TokenKind.BoolLiteral:
            {
                var t = Consume();
                return new LiteralExpr(t.Text == "true", t.Text, loc);
            }
            case TokenKind.Identifier:
            {
                var t = Consume();
                return new IdentifierExpr(t.Text, loc);
            }
            case TokenKind.LParen:
            {
                Consume();
                var inner = ParseExpression();
                Expect(TokenKind.RParen);
                return inner;
            }
            default:
            {
                var t = Consume();
                return new IdentifierExpr(t.Text, loc);
            }
        }
    }
}

internal static class TokenExtensions
{
    public static SourceLocation ToLocation(this Token t) => new(t.Line, t.Column);
}
