using Xoop.AST;
using Xoop.Emitter;
using Xoop.Parser;

namespace Xoop.Compiler;

// ─── Exception ───────────────────────────────────────────────────────────────

public class XoopCompileException(string message) : Exception(message);

// ─── Compiler ────────────────────────────────────────────────────────────────

/// <summary>
/// Ties together the XML parser and the C# emitter.
/// Call <see cref="Compile"/> with the contents of a .xoop file to get C# source.
/// </summary>
public class XoopCompiler
{
    private readonly XoopParser    _parser  = new();
    private readonly CSharpEmitter _emitter = new();

    public string Compile(string xoopXml)
    {
        ProgramNode ast = _parser.Parse(xoopXml);
        return _emitter.Emit(ast);
    }
}
