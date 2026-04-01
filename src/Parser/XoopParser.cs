using System.Xml.Linq;
using Xoop.AST;
using Xoop.Compiler;

namespace Xoop.Parser;

public class XoopParser
{
    // ── Entry ─────────────────────────────────────────────────────────────────

    public ProgramNode Parse(string xml)
    {
        XDocument doc;
        try { doc = XDocument.Parse(xml); }
        catch (Exception ex) { throw new XoopCompileException($"Invalid XML: {ex.Message}"); }

        var root = doc.Root ?? throw new XoopCompileException("Empty XML document.");
        if (root.Name.LocalName != "XoopProgram")
            throw new XoopCompileException(
                $"Root element must be <XoopProgram>, got <{root.Name.LocalName}>.");

        var program = new ProgramNode();

        foreach (var u in root.Elements("Using"))
        {
            var ns = u.Attribute("namespace")?.Value ?? u.Attribute("ns")?.Value;
            if (ns != null) program.Usings.Add(ns);
        }

        foreach (var ns  in root.Elements("Namespace")) program.Namespaces         .Add(ParseNamespace(ns));
        foreach (var cls in root.Elements("Class"))     program.TopLevelClasses    .Add(ParseClass(cls));
        foreach (var ifc in root.Elements("Interface")) program.TopLevelInterfaces .Add(ParseInterface(ifc));
        foreach (var en  in root.Elements("Enum"))      program.TopLevelEnums      .Add(ParseEnum(en));

        return program;
    }

    // ── Namespace ─────────────────────────────────────────────────────────────

    private NamespaceNode ParseNamespace(XElement el)
    {
        var ns = new NamespaceNode
        {
            Name = el.Attribute("name")?.Value
                   ?? throw new XoopCompileException("<Namespace> requires a 'name' attribute."),
        };

        foreach (var cls in el.Elements("Class"))     ns.Classes    .Add(ParseClass(cls));
        foreach (var en  in el.Elements("Enum"))      ns.Enums      .Add(ParseEnum(en));
        foreach (var ifc in el.Elements("Interface")) ns.Interfaces .Add(ParseInterface(ifc));

        return ns;
    }

    // ── Class ─────────────────────────────────────────────────────────────────

    private ClassNode ParseClass(XElement el)
    {
        var cls = new ClassNode
        {
            Name       = el.Attribute("name")?.Value
                         ?? throw new XoopCompileException("<Class> requires a 'name' attribute."),
            Access     = el.Attribute("access")?.Value ?? "public",
            BaseClass  = el.Attribute("extends")?.Value,
            IsStatic   = Bool(el, "static"),
            IsAbstract = Bool(el, "abstract"),
            IsSealed   = Bool(el, "sealed"),
            IsPartial  = Bool(el, "partial"),
        };

        SplitInto(el.Attribute("implements")?.Value, cls.Interfaces);
        SplitInto(el.Attribute("generics")?.Value,   cls.GenericParams);

        foreach (var f  in el.Elements("Field"))       cls.Fields        .Add(ParseField(f));
        foreach (var p  in el.Elements("Property"))    cls.Properties    .Add(ParseProperty(p));
        foreach (var c  in el.Elements("Constructor")) cls.Constructors  .Add(ParseConstructor(c));
        foreach (var m  in el.Elements("Method"))      cls.Methods       .Add(ParseMethod(m));
        foreach (var nc in el.Elements("Class"))       cls.NestedClasses .Add(ParseClass(nc));
        foreach (var ne in el.Elements("Enum"))        cls.NestedEnums   .Add(ParseEnum(ne));

        return cls;
    }

    // ── Interface ─────────────────────────────────────────────────────────────

    private InterfaceNode ParseInterface(XElement el)
    {
        var iface = new InterfaceNode
        {
            Name   = el.Attribute("name")?.Value
                     ?? throw new XoopCompileException("<Interface> requires a 'name' attribute."),
            Access = el.Attribute("access")?.Value ?? "public",
        };

        SplitInto(el.Attribute("extends")?.Value, iface.Extends);

        foreach (var m in el.Elements("Method"))
        {
            var sig = new MethodSignatureNode
            {
                Name       = m.Attribute("name")?.Value
                             ?? throw new XoopCompileException("<Method> in <Interface> requires 'name'."),
                ReturnType = m.Attribute("returnType")?.Value ?? "void",
            };
            foreach (var p in m.Elements("Parameter")) sig.Parameters.Add(ParseParameter(p));
            iface.Methods.Add(sig);
        }

        foreach (var p in el.Elements("Property"))
        {
            iface.Properties.Add(new PropertySignatureNode
            {
                Name   = p.Attribute("name")?.Value
                         ?? throw new XoopCompileException("<Property> in <Interface> requires 'name'."),
                Type   = p.Attribute("type")?.Value
                         ?? throw new XoopCompileException("<Property> in <Interface> requires 'type'."),
                HasGet = Bool(p, "get", defaultVal: true),
                HasSet = Bool(p, "set"),
            });
        }

        return iface;
    }

    // ── Enum ──────────────────────────────────────────────────────────────────

    private static EnumNode ParseEnum(XElement el)
    {
        var en = new EnumNode
        {
            Name           = el.Attribute("name")?.Value
                             ?? throw new XoopCompileException("<Enum> requires a 'name' attribute."),
            Access         = el.Attribute("access")?.Value ?? "public",
            UnderlyingType = el.Attribute("type")?.Value,
        };

        foreach (var m in el.Elements("Member"))
        {
            en.Members.Add(new EnumMemberNode
            {
                Name  = m.Attribute("name")?.Value
                        ?? throw new XoopCompileException("<Member> requires a 'name' attribute."),
                Value = m.Attribute("value")?.Value,
            });
        }

        return en;
    }

    // ── Field ─────────────────────────────────────────────────────────────────

    private static FieldNode ParseField(XElement el) => new()
    {
        Name         = el.Attribute("name")?.Value
                       ?? throw new XoopCompileException("<Field> requires 'name'."),
        Type         = el.Attribute("type")?.Value
                       ?? throw new XoopCompileException("<Field> requires 'type'."),
        Access       = el.Attribute("access")?.Value ?? "private",
        IsStatic     = Bool(el, "static"),
        IsReadOnly   = Bool(el, "readonly"),
        IsConst      = Bool(el, "const"),
        DefaultValue = el.Attribute("default")?.Value ?? el.Value.Trimmed(),
    };

    // ── Property ──────────────────────────────────────────────────────────────

    private static PropertyNode ParseProperty(XElement el)
    {
        var prop = new PropertyNode
        {
            Name         = el.Attribute("name")?.Value
                           ?? throw new XoopCompileException("<Property> requires 'name'."),
            Type         = el.Attribute("type")?.Value
                           ?? throw new XoopCompileException("<Property> requires 'type'."),
            Access       = el.Attribute("access")?.Value ?? "public",
            SetAccess    = el.Attribute("setAccess")?.Value ?? "",
            IsStatic     = Bool(el, "static"),
            IsVirtual    = Bool(el, "virtual"),
            IsOverride   = Bool(el, "override"),
            FieldRef     = el.Attribute("fieldRef")?.Value,
            DefaultValue = el.Attribute("default")?.Value,
        };

        var getEl = el.Element("Get");
        var setEl = el.Element("Set");

        if (getEl != null || setEl != null)
        {
            // explicit <Get> / <Set> child elements with custom bodies
            prop.HasGet  = getEl != null;
            prop.HasSet  = setEl != null;
            prop.GetBody = getEl?.Value.Trimmed();
            prop.SetBody = setEl?.Value.Trimmed();
        }
        else
        {
            // attribute-based or pure auto-property
            prop.HasGet = Bool(el, "get", defaultVal: true);
            prop.HasSet = Bool(el, "set", defaultVal: true);
        }

        return prop;
    }

    // ── Constructor ───────────────────────────────────────────────────────────

    private static ConstructorNode ParseConstructor(XElement el)
    {
        var ctor = new ConstructorNode
        {
            Access = el.Attribute("access")?.Value ?? "public",
        };

        foreach (var p in el.Elements("Parameter")) ctor.Parameters.Add(ParseParameter(p));

        var baseEl = el.Element("Base");
        var thisEl = el.Element("This");

        if (baseEl != null)      { ctor.BaseCallArgs = CallArgs(baseEl); ctor.IsThisCall = false; }
        else if (thisEl != null) { ctor.BaseCallArgs = CallArgs(thisEl); ctor.IsThisCall = true;  }

        ctor.Body = el.Element("Body")?.Value ?? "";
        return ctor;
    }

    // ── Method ────────────────────────────────────────────────────────────────

    private static MethodNode ParseMethod(XElement el)
    {
        var m = new MethodNode
        {
            Name       = el.Attribute("name")?.Value
                         ?? throw new XoopCompileException("<Method> requires 'name'."),
            ReturnType = el.Attribute("returnType")?.Value ?? "void",
            Access     = el.Attribute("access")?.Value ?? "public",
            IsStatic   = Bool(el, "static"),
            IsVirtual  = Bool(el, "virtual"),
            IsOverride = Bool(el, "override"),
            IsAbstract = Bool(el, "abstract"),
            IsSealed   = Bool(el, "sealed"),
            IsNew      = Bool(el, "new"),
            IsAsync    = Bool(el, "async"),
        };

        SplitInto(el.Attribute("generics")?.Value, m.GenericParams);
        foreach (var p in el.Elements("Parameter")) m.Parameters.Add(ParseParameter(p));
        m.Body = el.Element("Body")?.Value ?? "";
        return m;
    }

    // ── Parameter ─────────────────────────────────────────────────────────────

    private static ParameterNode ParseParameter(XElement el) => new()
    {
        Name         = el.Attribute("name")?.Value
                       ?? throw new XoopCompileException("<Parameter> requires 'name'."),
        Type         = el.Attribute("type")?.Value
                       ?? throw new XoopCompileException("<Parameter> requires 'type'."),
        DefaultValue = el.Attribute("default")?.Value,
        IsParams     = Bool(el, "params"),
        IsRef        = Bool(el, "ref"),
        IsOut        = Bool(el, "out"),
        IsIn         = Bool(el, "in"),
    };

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static string CallArgs(XElement el) =>
        string.Join(", ", el.Elements("Arg").Select(a => a.Value.Trim()));

    private static bool Bool(XElement el, string attr, bool defaultVal = false) =>
        el.Attribute(attr)?.Value?.ToLowerInvariant() is { } v
            ? v is "true" or "yes" or "1"
            : defaultVal;

    private static void SplitInto(string? csv, List<string> target)
    {
        if (csv != null)
            target.AddRange(csv.Split(',').Select(s => s.Trim()).Where(s => s.Length > 0));
    }
}

internal static class StringExtensions
{
    /// <summary>Returns null when the string is null or whitespace-only.</summary>
    public static string? Trimmed(this string? s) =>
        string.IsNullOrWhiteSpace(s) ? null : s.Trim();
}
