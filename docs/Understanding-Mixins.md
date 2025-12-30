# Understanding Mixins

## What is a Mixin?

A **mixin** is reusable code that gets "mixed into" other code to add functionality - like adding ingredients to a recipe, it becomes part of the final product.

In MDK for Space Engineers, mixins work by combining your code with Shared Project references during the build. The key advantage: you can include code from **outside your project folder**. Instead of copying and pasting, you reference external Shared Projects and their source code seamlessly integrates.

**For programmable block scripts:** MDK packages everything into a single `.cs` file ready for the game.

**For mods:** Source files from the mixin are included with your mod's files (mods retain multiple separate `.cs` files).

## How Mixins Work in MDK

### Shared Projects

MDK mixins use **Shared Projects** - a special project type that doesn't produce its own output. Instead, it's a container for source files that can be referenced by other projects.

When you add a Shared Project reference, its `.cs` files are compiled together with your code as if they were all in the same project from the start.

### Visual Example

```
Your Project          Shared Project (Mixin)
├── Program.cs        ├── MixinCode.cs
├── Helper.cs         └── MixinHelper.cs
└── References:
    └── MyMixin (Shared Project)

After Compilation:
Your project's output contains code from all files above
```

## Why This Approach for Space Engineers?

Space Engineers doesn't support loading external DLL libraries. Both programmable blocks and mods work with source code, making mixins the practical way to share code between projects.

**Scripts** must be delivered as single `.cs` files. MDK collects all your code from your project and any referenced Shared Projects, then packages everything into one file ready for the game.

**Mods** are also based on source code files. When you reference a Shared Project in a mod, the mixin's source files are included with your mod's files. Unlike scripts, mods retain multiple separate `.cs` files rather than being merged into one.

Before mixins, you'd copy-paste code between projects, maintain multiple copies, and manually update everywhere when fixing bugs. With mixins: write once, reference in multiple projects, and updates propagate automatically on rebuild.

## Important Shared Project Characteristics

**Not Self-Sufficient** - A Shared Project can't be built or run on its own. It has no framework target, language version, or dependencies. You **must** have at least one regular project (like a Demo project) that references it for IntelliSense to work.

**Context From Referencing Project** - The Shared Project inherits everything from the referencing project: C# version, framework target, dependencies, and compiler settings.

**Multiple References Can Confuse IntelliSense** - If you reference the same Shared Project from multiple projects with different C# versions or frameworks, IntelliSense may suggest features that won't work everywhere. Known limitation.

**Code Duplication in Output** - Each project gets its own copy of the mixin code in compiled output. For Space Engineers this doesn't matter since programmable blocks can't share memory anyway.

**Rebuild Required for Updates** - When you change mixin code, every project that references it must rebuild to pick up changes.

**Namespace Collisions** - All code compiles together, so use unique namespaces following the convention `YourName.MixinName.*` to avoid conflicts.

## Creating Your Own Mixins

See the IDE-specific guides:
- [Creating a Mixin with Visual Studio](Creating-A-Mixin-VisualStudio.md)
- [Creating a Mixin with Rider](Creating-A-Mixin-Rider.md)
- [Creating a Mixin with VS Code](Creating-A-Mixin-VSCode.md)

## Technical Deep Dive: How Shared Projects Work

### Project Structure

A Shared Project has two files:

**`.shproj`** - The project file IDEs recognize
**`.projitems`** - The actual source file list

### How Referencing Works

When you reference a Shared Project, MSBuild:
1. Reads the `.projitems` file
2. Expands file paths to the Shared Project's folder
3. Adds all source files to the referencing project's compilation
4. The C# compiler sees them as if they were in the original project

### Conditional Compilation

Use preprocessor directives for environment-specific code:

```csharp
public class MyMixin
{
    public void DoSomething()
    {
#if SPACE_ENGINEERS
        // SE-specific code
#else
        // Generic code
#endif
    }
}
```

## Summary

Mixins provide powerful code reuse for Space Engineers development. They work at the source code level - perfect for SE's single-file scripts and multi-file mods that both work with source code. Using Shared Project technology, mixins give you the benefits of code reuse and library distribution without external dependencies or version conflicts.
