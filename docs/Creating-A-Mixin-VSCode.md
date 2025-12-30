# Creating a New Mixin Library - Visual Studio Code

This guide will walk you through creating a new mixin library for the MDK Package Libraries repository using Visual Studio Code and the command line.

## Prerequisites

- Visual Studio Code
- .NET Framework 4.8 SDK
- .NET SDK (for CLI tools)
- MDK2 installed
- Git
- C# extension (ms-dotnettools.csharp)

## Step 1: Fork and Clone the Repository

1. Go to `https://github.com/malforge/mdk2-packages` and click **Fork**
2. Open a terminal and run:

```bash
git clone https://github.com/YourUsername/mdk2-packages
cd mdk2-packages
code .
```

## Step 2: Create the Library Structure

Choose a library name following the format:
- `YourName.MdkScriptMixin.LibraryName` - For programmable block scripts only
- `YourName.MdkModMixin.LibraryName` - For mods only
- `YourName.MdkSharedMixin.LibraryName` - For both scripts and mods

1. In VS Code's Explorer, right-click the `libraries` folder → **New Folder**
2. Name it (e.g., `Alice.MdkScriptMixin.DataGrid`)
3. Open the integrated terminal (`` Ctrl+` ``)
4. Navigate to your new folder and create the mixin project:
   ```
   cd libraries\Alice.MdkScriptMixin.DataGrid
   dotnet new mdk2mixin -n Alice.MdkScriptMixin.DataGrid
   ```

## Step 3: Create Required Metadata Files

In VS Code's Explorer, navigate to your mixin project folder and create these files:

1. Right-click the mixin project folder → **New File**
2. Create `_version` and enter: `1.0.0`
3. Create `_authors` and enter your name
4. Create `_description` and enter a brief description
5. Create `_releasenotes` with format:
   ```
   - 1.0.0
     Initial release
   ```
6. Create `readme.md` with usage examples and documentation

## Step 4: Add Your Code Files

Now you'll write the actual library code that you want to share with others.

1. Right-click your mixin project folder → **New File**
2. Name it (e.g., `MyLibrary.cs`)
3. Write your library code
4. Add as many files as needed to organize your code

Example code structure:

```csharp
using System;

namespace IngameScript
{
    public class MyLibrary
    {
        public void DoSomething()
        {
            // Your code here
        }
    }
}
```

**Important:**
- **For script mixins**: Strongly recommended to use the `IngameScript` namespace to avoid conflicts (programmable blocks don't use namespaces, making namespace mismatches easy to cause trouble). Mixins for mods do not need to follow this convention.

**Tips:**
- Add as many files as needed to organize your code
- Keep code well-organized and commented
- Consider edge cases and error handling
- Make your API intuitive for other developers

## Step 5: Create a Demo Project

The demo shows how to use your library and is required for packaging.

1. Open the integrated terminal (`` Ctrl+` ``)
2. Run: `dotnet new mdk2pbscript -n Alice.MdkScriptMixin.DataGrid.Demo`
   (replace with your library name + `.Demo`)

## Step 6: Reference the Mixin in Demo

1. In Explorer, open the Demo project's `.csproj` file
2. Find the `</Project>` closing tag at the bottom
3. Add this line just before it:
   ```xml
   <Import Project="..\Alice.MdkScriptMixin.DataGrid\Alice.MdkScriptMixin.DataGrid.projitems" Label="Shared"/>
   ```
   (Replace with your actual library name)
4. Save the file

## Step 7: Write Demo Code

1. In Explorer, open `Program.cs` in the Demo project
2. Modify it to demonstrate your library usage:

```csharp
partial class Program : MyGridProgram
{
    public Program()
    {
        // Initialize your library
        var myLib = new MyLibrary();
    }

    public void Main(string argument, UpdateType updateSource)
    {
        // Show library usage
    }
}
```

## Step 8: Test Building

1. Open the integrated terminal (`` Ctrl+` ``)
2. Navigate to repository root: `cd ..\..`
3. Build: `dotnet build --configuration Release`
4. Check the output for any errors

## Step 9: Commit and Push

1. Open the Source Control view (`` Ctrl+Shift+G ``)
2. Stage all changes (click **+** next to Changes)
3. Enter commit message: `Add [YourLibraryName] mixin`
4. Click **✓ Commit**
5. Click **⋯** → **Push**

## Step 10: Create a Pull Request

1. Go to your fork: `https://github.com/YourUsername/mdk2-packages`
2. Click **Pull requests** → **New pull request**
3. Verify base is `malforge/mdk2-packages:main`
4. Title: `Add [YourLibraryName] mixin`
5. Describe what your library does
6. Click **Create pull request**

## Step 11: Wait for Review

1. GitHub Actions will automatically test your build
2. A maintainer will review your code
3. Once approved and merged, your library is automatically published to GitHub Packages

## Quick Tips

- Always include `readme.md` with usage examples
- Use semantic versioning (1.0.0 = Major.Minor.Patch)
- Test in Space Engineers before submitting
- Press `Ctrl+Shift+P` for command palette
- Use `Ctrl+`` to toggle terminal

## Need Help?

Check out existing libraries in the repository for examples, or open an issue on GitHub!

---

*This guide is part of the MDK Package Libraries documentation.*
