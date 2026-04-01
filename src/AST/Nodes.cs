namespace Xoop.AST;

// ─── Root ────────────────────────────────────────────────────────────────────

public abstract class XoopNode { }

public class ProgramNode : XoopNode
{
    public List<string>        Usings              { get; set; } = [];
    public List<NamespaceNode> Namespaces          { get; set; } = [];
    public List<ClassNode>     TopLevelClasses     { get; set; } = [];
    public List<InterfaceNode> TopLevelInterfaces  { get; set; } = [];
    public List<EnumNode>      TopLevelEnums       { get; set; } = [];
}

// ─── Namespace ───────────────────────────────────────────────────────────────

public class NamespaceNode : XoopNode
{
    public string              Name       { get; set; } = "";
    public List<ClassNode>     Classes    { get; set; } = [];
    public List<EnumNode>      Enums      { get; set; } = [];
    public List<InterfaceNode> Interfaces { get; set; } = [];
}

// ─── Class ───────────────────────────────────────────────────────────────────

public class ClassNode : XoopNode
{
    public string              Name          { get; set; } = "";
    public string              Access        { get; set; } = "public";
    public string?             BaseClass     { get; set; }
    public List<string>        Interfaces    { get; set; } = [];
    public List<string>        GenericParams { get; set; } = [];
    public bool                IsStatic      { get; set; }
    public bool                IsAbstract    { get; set; }
    public bool                IsSealed      { get; set; }
    public bool                IsPartial     { get; set; }
    public List<FieldNode>       Fields        { get; set; } = [];
    public List<PropertyNode>    Properties    { get; set; } = [];
    public List<ConstructorNode> Constructors  { get; set; } = [];
    public List<MethodNode>      Methods       { get; set; } = [];
    public List<ClassNode>       NestedClasses { get; set; } = [];
    public List<EnumNode>        NestedEnums   { get; set; } = [];
}

// ─── Interface ───────────────────────────────────────────────────────────────

public class InterfaceNode : XoopNode
{
    public string                     Name       { get; set; } = "";
    public string                     Access     { get; set; } = "public";
    public List<string>               Extends    { get; set; } = [];
    public List<MethodSignatureNode>  Methods    { get; set; } = [];
    public List<PropertySignatureNode> Properties { get; set; } = [];
}

public class MethodSignatureNode : XoopNode
{
    public string             Name       { get; set; } = "";
    public string             ReturnType { get; set; } = "void";
    public List<ParameterNode> Parameters { get; set; } = [];
}

public class PropertySignatureNode : XoopNode
{
    public string Name   { get; set; } = "";
    public string Type   { get; set; } = "";
    public bool   HasGet { get; set; } = true;
    public bool   HasSet { get; set; }
}

// ─── Enum ────────────────────────────────────────────────────────────────────

public class EnumNode : XoopNode
{
    public string              Name           { get; set; } = "";
    public string              Access         { get; set; } = "public";
    public string?             UnderlyingType { get; set; }
    public List<EnumMemberNode> Members       { get; set; } = [];
}

public class EnumMemberNode : XoopNode
{
    public string  Name  { get; set; } = "";
    public string? Value { get; set; }
}

// ─── Field ───────────────────────────────────────────────────────────────────

public class FieldNode : XoopNode
{
    public string  Name         { get; set; } = "";
    public string  Type         { get; set; } = "";
    public string  Access       { get; set; } = "private";
    public bool    IsStatic     { get; set; }
    public bool    IsReadOnly   { get; set; }
    public bool    IsConst      { get; set; }
    public string? DefaultValue { get; set; }
}

// ─── Property ────────────────────────────────────────────────────────────────

public class PropertyNode : XoopNode
{
    public string  Name         { get; set; } = "";
    public string  Type         { get; set; } = "";
    public string  Access       { get; set; } = "public";
    public string  SetAccess    { get; set; } = "";
    public bool    IsStatic     { get; set; }
    public bool    IsVirtual    { get; set; }
    public bool    IsOverride   { get; set; }
    public string? FieldRef     { get; set; }
    public string? GetBody      { get; set; }
    public string? SetBody      { get; set; }
    public bool    HasGet       { get; set; } = true;
    public bool    HasSet       { get; set; } = true;
    public string? DefaultValue { get; set; }
}

// ─── Constructor ─────────────────────────────────────────────────────────────

public class ConstructorNode : XoopNode
{
    public string             Access       { get; set; } = "public";
    public List<ParameterNode> Parameters  { get; set; } = [];
    public string?            BaseCallArgs { get; set; }
    public bool               IsThisCall   { get; set; }
    public string             Body         { get; set; } = "";
}

// ─── Method ──────────────────────────────────────────────────────────────────

public class MethodNode : XoopNode
{
    public string             Name          { get; set; } = "";
    public string             ReturnType    { get; set; } = "void";
    public string             Access        { get; set; } = "public";
    public List<string>       GenericParams { get; set; } = [];
    public List<ParameterNode> Parameters   { get; set; } = [];
    public bool               IsStatic      { get; set; }
    public bool               IsVirtual     { get; set; }
    public bool               IsOverride    { get; set; }
    public bool               IsAbstract    { get; set; }
    public bool               IsSealed      { get; set; }
    public bool               IsNew         { get; set; }
    public bool               IsAsync       { get; set; }
    public string             Body          { get; set; } = "";
}

// ─── Parameter ───────────────────────────────────────────────────────────────

public class ParameterNode : XoopNode
{
    public string  Name         { get; set; } = "";
    public string  Type         { get; set; } = "";
    public string? DefaultValue { get; set; }
    public bool    IsParams     { get; set; }
    public bool    IsRef        { get; set; }
    public bool    IsOut        { get; set; }
    public bool    IsIn         { get; set; }
}
