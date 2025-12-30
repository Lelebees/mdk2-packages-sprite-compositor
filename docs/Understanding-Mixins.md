# Understanding Mixins

## What is a Mixin?

In programming, a **mixin** is a reusable piece of code that can be "mixed into" other code to add functionality. Think of it like adding ingredients to a recipe - the mixin becomes part of the final product rather than remaining separate.

For MDK and Space Engineers, we use the term "mixin" because of how the build process works. When you build a programmable block script with MDK, your main script code is combined with any Shared Project references, and MDK packages everything into a single `.cs` file that gets deployed to your local Workshop folder, ready for the game to use.

The key innovation is that Shared Projects allow you to include code from **outside your main project folder**. Since a Shared Project simply extends your project with additional source files, MDK can collect and package code from multiple locations into one final script. This is what makes them "mixins" - instead of copying and pasting code or keeping everything in one project folder, you can reference external Shared Projects and their code seamlessly integrates into your script during the build.

## How Mixins Work in MDK

### Shared Projects

MDK mixins use Visual Studio's **Shared Project** technology. A Shared Project is a special project type that doesn't produce its own DLL. Instead, it acts as a container for source files that can be referenced by other projects.

When you add a Shared Project reference, the Shared Project's `.cs` files are virtually added to your project. They compile together with your code, and the result is a single output assembly containing everything. All files are compiled together as if they were all in the same project from the start.

### The Technical Details

```
Your Project          Shared Project (Mixin)
├── Program.cs        ├── MixinCode.cs
├── Helper.cs         └── MixinHelper.cs
└── References:
    └── MyMixin (Shared Project)

After Compilation:
YourProject.dll contains code from:
- Program.cs
- Helper.cs
- MixinCode.cs (from the mixin)
- MixinHelper.cs (from the mixin)
```

All files are compiled together as if they were all in the same project from the start.

## Why This Approach for Space Engineers?

Space Engineers doesn't support loading external DLL libraries for either programmable blocks or mods. This makes mixins the only practical way to share code between projects.

### Programmable Block Scripts

Programmable block scripts must be delivered as single `.cs` files. MDK collects all your code from your main project and any referenced Shared Projects, then packages everything into one file that gets deployed to your local Workshop folder, ready for the game to use.

### Mods

Mods have more flexibility in their structure - they don't need to be a single file. However, they still cannot reference external DLL libraries. Mixins provide the same code-sharing benefits for mods that they do for scripts.

### Code Sharing Without Copying

Before mixins, sharing code meant copying and pasting between projects, maintaining multiple copies of the same code, and manually updating all copies when bugs were fixed. With mixins, you write the code once, reference it in multiple projects, and updates propagate automatically when you rebuild.

## Understanding Shared Project Limitations

Shared Projects are powerful but have some important characteristics you need to understand:

**Shared Projects Are Not Self-Sufficient** - A Shared Project cannot be built or run on its own. It has no framework target, no language version, no dependencies of its own. It's essentially just a collection of source files waiting to be included in a real project. This means you cannot open a Shared Project by itself and expect IntelliSense to work or the code to compile. You **must** have at least one regular project (like your Demo project) that references the Shared Project for the IDE to understand the context.

**Context Comes From the Referencing Project** - When you reference a Shared Project, it inherits everything from the referencing project: the C# language version, the framework target, the dependencies, and all compiler settings. This is why the Demo project is essential - it provides the context that makes your mixin code "real."

**Multiple References Can Cause Confusion** - If you reference the same Shared Project from multiple regular projects in the same solution, and those projects use different C# versions or frameworks, the IDE's IntelliSense will get confused. It typically picks the "highest" or most recent version it finds, which means you might see IntelliSense suggesting features that won't work in all your referencing projects. This is a known limitation of Shared Projects and something to be aware of when structuring your solution.

**Code Duplication in Output** - Each project that references a Shared Project gets its own copy of the code in the compiled output. If you have multiple scripts using the same mixin, each script's DLL will contain a complete copy of the mixin code. For Space Engineers this doesn't matter since programmable blocks can't share memory anyway, and MDK strips out unused types automatically.

**Rebuild Required for Updates** - When you change code in a Shared Project, every project that references it needs to be rebuilt to pick up the changes. For most SE projects this happens quickly, but it's something to be aware of in larger solutions.

**Namespace Collisions** - Since all code compiles together as if it were in one project, you can have naming conflicts if multiple mixins define the same class names. Always use unique, descriptive namespaces following the convention `YourName.MixinName.*` to avoid this.

## Creating Your Own Mixins

To learn how to create your own mixins, see [Creating a Mixin with Visual Studio](Creating-A-Mixin-VisualStudio.md), [Creating a Mixin with Rider](Creating-A-Mixin-Rider.md), or [Creating a Mixin with VS Code](Creating-A-Mixin-VSCode.md).

## Technical Deep Dive: How Shared Projects Work

### Project Structure

A Shared Project consists of two files:

**`.shproj`** - The project file that Visual Studio/Rider/VS Code recognizes
```xml
<Project ToolsVersion="15.0" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
    <PropertyGroup Label="Globals">
        <ProjectGuid>{GUID}</ProjectGuid>
    </PropertyGroup>
    <Import Project="$(MSBuildExtensionsPath)\...CodeSharing.Common.props"/>
    <Import Project="YourMixin.projitems" Label="Shared"/>
    <Import Project="$(MSBuildExtensionsPath)\...CodeSharing.CSharp.targets"/>
</Project>
```

**`.projitems`** - The actual item list (source files, resources, etc.)
```xml
<Project xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
    <PropertyGroup>
        <HasSharedItems>true</HasSharedItems>
        <SharedGUID>{GUID}</SharedGUID>
    </PropertyGroup>
    <ItemGroup>
        <Compile Include="$(MSBuildThisFileDirectory)MyCode.cs" />
        <Compile Include="$(MSBuildThisFileDirectory)Helper.cs" />
    </ItemGroup>
</Project>
```

### How Referencing Works

When a regular project references a Shared Project, MSBuild reads the `.projitems` file, expands `$(MSBuildThisFileDirectory)` to the Shared Project's folder, adds all the `<Compile>` items to the referencing project's compilation, and the C# compiler sees them as if they were part of the original project.

### Conditional Compilation

You can use preprocessor directives to handle differences:

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

This allows one mixin to work in multiple environments if needed.

## Summary

Mixins are a powerful pattern for code reuse in Space Engineers. They are source code that gets compiled directly into your project, perfect for SE's single-file programmable block scripts. They work using Visual Studio Shared Projects technology and are ideal for utilities, helpers, and reusable algorithms. The mixin approach gives you the benefits of code reuse and library distribution without the downsides of external dependencies or version conflicts, making it the perfect fit for Space Engineers scripting.
