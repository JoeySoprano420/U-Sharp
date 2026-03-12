namespace USharp.Compiler.Semantics;

public enum SymbolKind { Variable, Function, Class, Parameter, Field }

public class Symbol
{
    public string Name { get; }
    public SymbolKind Kind { get; }
    public string TypeName { get; }
    public bool IsReadonly { get; }

    public Symbol(string name, SymbolKind kind, string typeName, bool isReadonly = false)
    {
        Name = name;
        Kind = kind;
        TypeName = typeName;
        IsReadonly = isReadonly;
    }
}

public class SymbolTable
{
    private readonly SymbolTable? _parent;
    private readonly Dictionary<string, Symbol> _symbols = new(StringComparer.Ordinal);

    public SymbolTable(SymbolTable? parent = null) { _parent = parent; }

    public bool TryDefine(Symbol symbol)
    {
        if (_symbols.ContainsKey(symbol.Name)) return false;
        _symbols[symbol.Name] = symbol;
        return true;
    }

    public void Define(Symbol symbol) => _symbols[symbol.Name] = symbol;

    public Symbol? Lookup(string name)
    {
        if (_symbols.TryGetValue(name, out var sym)) return sym;
        return _parent?.Lookup(name);
    }

    public SymbolTable CreateChild() => new(this);
}
