# XOOP

XOOP (XML Object-Oriented Programming) is a small source-to-source language
that uses XML as its surface syntax and compiles to idiomatic C#.

This repository contains a minimal XOOP compiler that:

- Parses `.xoop` XML files into an AST
- Emits readable, well-formatted C# source files
- Provides a CLI to compile `.xoop` → `.cs`

Features
--------

- Classes, interfaces and enums
- Fields, properties (auto, field-backed, custom accessors)
- Constructors with `base(...)` / `this(...)` chaining
- Methods: `static`, `virtual`, `override`, `abstract`, `async`
- Parameter modifiers: `params`, `ref`, `out`, `in`, and default values
- Bodies written as plain C# (wrap in CDATA in `.xoop` files)

Quickstart
----------

Build the compiler:

```powershell
dotnet build
```

Compile an example `.xoop` file into C#:

```powershell
dotnet run -- examples/hello.xoop            # outputs examples/hello.cs
dotnet run -- examples/animals.xoop out.cs   # specify output path
```

The CLI prints a brief success line and writes the generated C# to disk.

Examples
--------

- `examples/hello.xoop` — simple hello/greet demo
- `examples/animals.xoop` — full OOP demo with interfaces, enums,
	inheritance and constructor chaining

Notes
-----

- Method and constructor bodies are emitted verbatim (after whitespace
	normalisation). Use CDATA sections in `.xoop` files to avoid XML escaping.
- The emitted C# has no XOOP runtime dependency — compile the generated
	`.cs` file with the .NET SDK as usual.

Contributing
------------

Contributions welcome. Open an issue or a pull request with a description of
what you'd like to add (examples, language features, emitter improvements).

License
-------

This project is licenced under MPL

