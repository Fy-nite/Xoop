using Xoop.Compiler;

// ─── CLI ─────────────────────────────────────────────────────────────────────

if (args.Length == 0 || args[0] is "-h" or "--help")
{
    PrintHelp();
    return 0;
}

if (args[0] is "-v" or "--version")
{
    Console.WriteLine("XOOP Compiler v1.0.0");
    return 0;
}

string inputFile  = args[0];
string outputFile = args.Length > 1 ? args[1] : Path.ChangeExtension(inputFile, ".cs");

if (!File.Exists(inputFile))
{
    WriteError($"File not found: '{inputFile}'");
    return 1;
}

try
{
    var watch  = System.Diagnostics.Stopwatch.StartNew();
    string xml = File.ReadAllText(inputFile);
    string cs  = new XoopCompiler().Compile(xml);
    File.WriteAllText(outputFile, cs);
    watch.Stop();

    Console.ForegroundColor = ConsoleColor.Green;
    Console.Write("✓ ");
    Console.ResetColor();
    Console.WriteLine($"{Path.GetFileName(inputFile)}  →  {outputFile}  ({watch.ElapsedMilliseconds} ms)");
    return 0;
}
catch (XoopCompileException ex)
{
    WriteError($"Compile error: {ex.Message}");
    return 1;
}
catch (Exception ex)
{
    WriteError($"Fatal error: {ex.Message}");
    return 1;
}

// ─── Helpers ─────────────────────────────────────────────────────────────────

static void WriteError(string msg)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.Error.WriteLine($"[xoop] {msg}");
    Console.ResetColor();
}

static void PrintHelp() => Console.WriteLine("""
    ╔══════════════════════════════════════════════════════════╗
    ║  XOOP — XML Object-Oriented Programming Language v1.0.0 ║
    ║  Compiles .xoop files to C# source code.                ║
    ╚══════════════════════════════════════════════════════════╝

    Usage:
      xoop <input.xoop> [output.cs]
      xoop --help | --version

    Examples:
      xoop examples/hello.xoop
      xoop examples/animals.xoop out/Animals.cs

    ┌─ Language reference ────────────────────────────────────────────────┐
    │                                                                      │
    │  <XoopProgram>                          Root element                 │
    │  <Using namespace="System"/>            Import namespace             │
    │  <Namespace name="MyApp">               Declare namespace            │
    │                                                                      │
    │  <Class name="Foo"                      Class declaration            │
    │         access="public"                   public / private / …       │
    │         extends="Bar"                     base class                 │
    │         implements="IFoo,IBar"            interfaces (comma-sep.)    │
    │         abstract="true"                   mark abstract              │
    │         sealed="true|static="true">       other modifiers            │
    │                                                                      │
    │  <Interface name="IFoo">                Interface declaration        │
    │  <Enum name="Color">                    Enum declaration             │
    │    <Member name="Red" value="0"/>         enum member                │
    │                                                                      │
    │  <Field name="_x" type="int"            Field                        │
    │         access="private"                                             │
    │         readonly="true" const="true"                                 │
    │         default="42"/>                    initial value              │
    │                                                                      │
    │  <Property name="X" type="int"          Property                     │
    │            access="public"                                           │
    │            fieldRef="_x"                  backed by field            │
    │            get="true" set="false"          auto accessor control     │
    │            setAccess="private"            narrower setter access     │
    │            default="0"/>                  auto-property default      │
    │                                                                      │
    │  <Constructor access="public">          Constructor                  │
    │    <Parameter name="x" type="int"/>       parameter                  │
    │    <Base><Arg>x</Arg></Base>              base(...) call             │
    │    <Body><![CDATA[ … ]]></Body>           body (CDATA recommended)  │
    │                                                                      │
    │  <Method name="Foo"                     Method                       │
    │          returnType="void"                                           │
    │          access="public"                                             │
    │          static="false" virtual="true"                               │
    │          override="false" abstract="false"                           │
    │          async="false" sealed="false">                               │
    │    <Parameter name="n" type="int"                                    │
    │               default="0" ref="false"/>                              │
    │    <Body><![CDATA[ … ]]></Body>                                      │
    │  </Method>                                                           │
    └──────────────────────────────────────────────────────────────────────┘
    """);
