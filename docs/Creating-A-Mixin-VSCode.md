# Creating a New Mixin Library - Visual Studio Code

This guide will walk you through creating a new mixin library for the MDK Package Libraries repository using Visual Studio Code and the command line.

## Prerequisites

- Visual Studio Code
- .NET Framework 4.8 SDK
- .NET SDK (for CLI tools)
- Git
- C# extension for VS Code (ms-dotnettools.csharp)

## Step 1: Fork and Clone the Repository

You don't have direct write access to the main repository. Forking creates your own copy where you can make changes, then submit them as a pull request.

1. Go to `https://github.com/malforge/mdk2-packages` in your browser
2. Click the **Fork** button in the top right
3. GitHub will create a fork under your account
4. Open a terminal and run:

```bash
git clone https://github.com/YourUsername/mdk2-packages
cd mdk2-packages
code .
```

This will clone **your fork** and open it in VS Code.

## Step 2: Create the Library Folder Structure

In the terminal (or VS Code's integrated terminal):

```bash
# Replace with your actual library name following the naming convention
LIBNAME="Alice.MdkScriptMixin.DataGrid"

# Create the folder structure
mkdir -p "libraries/$LIBNAME/$LIBNAME"
cd "libraries/$LIBNAME/$LIBNAME"
```

**Naming Convention:**
- `YourName.MdkScriptMixin.LibraryName` - For programmable block scripts
- `YourName.MdkModMixin.LibraryName` - For mods
- `YourName.MdkSharedMixin.LibraryName` - For libraries that work in both

## Step 3: Create the Mixin Project (Shared Project)

This is where you'll write your library code. The MDK Mixin template sets up a [Shared Project](Understanding-Mixins.md) with all the necessary files and structure.

In the terminal:

```bash
# Replace with your actual library name
LIBNAME="Alice.MdkScriptMixin.DataGrid"

# Navigate to your library folder
cd "libraries/$LIBNAME"

# Create the mixin project using the MDK template
dotnet new mdk2mixin -n "$LIBNAME"
```

This creates a Shared Project with all the necessary structure pre-configured.

## Step 4: Create Required Metadata Files

```bash
# _version
echo "1.0.0" > _version

# _authors
cat > _authors << 'EOF'
Your Name
Co-Author Name (if any)
EOF

# _description
echo "A brief description of what your library does." > _description

# _releasenotes
cat > _releasenotes << 'EOF'
- 1.0.0
  Initial release
  Feature 1 added
  Feature 2 added
EOF

# readme.md
cat > readme.md << 'EOF'
# Your Library Name

Brief overview of what your library does.

## Features

- Feature 1
- Feature 2
- Feature 3

## Usage

```csharp
// Example code here
```

## Installation

See the main repository documentation for installation instructions.

## API Reference

Document your public API here.
EOF
```

**Important:** Always include a readme.md to avoid public shaming in the documentation! 😄

## Step 5: Add Your Code Files

Create your C# source files in the mixin project folder:

```bash
# Example: Create a main library file
cat > MyLibrary.cs << 'EOF'
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
EOF
```

## Step 6: Create a Demo Project

The Demo Project serves three purposes:
1. **Testing**: It lets you test your library code as if it were being used in a real Space Engineers script
2. **Documentation**: It shows other developers how to use your library
3. **Packaging**: The build system uses it to package your library as a NuGet package

Think of it as a sample Space Engineers script that uses your library.

In the terminal:

```bash
# Navigate back to your library folder (if needed)
cd "libraries/$LIBNAME"

# Create the demo project using the MDK template
dotnet new mdk2pbscript -n "$LIBNAME.Demo"
```

The template will create a fully configured demo project with all MDK packages already included.

## Step 7: Reference the Mixin in Demo

This connects your Demo project to your library code. When you build the Demo, it will include all the code from your Shared Project, just like a real user's script would.

Edit the Demo project's `.csproj` file to add the shared project reference:

```bash
cd "$LIBNAME.Demo"

# Add the Import line before the closing </Project> tag
# You'll need to edit this manually or use a text editor
# Add: <Import Project="..\$LIBNAME\$LIBNAME.projitems" Label="Shared"/>
```

Open the `.csproj` file in VS Code and add this line before `</Project>`:

```xml
<Import Project="..\YourLibraryName\YourLibraryName.projitems" Label="Shared"/>
```

(Replace `YourLibraryName` with your actual library name)

## Step 8: Create Demo Code

The MDK template already created a `Program.cs` with the basic structure. Open it in VS Code and modify it to demonstrate your library - this helps other developers understand how to use it and serves as a working test.

```csharp
using Sandbox.ModAPI.Ingame;
using System;

namespace IngameScript
{
    partial class Program : MyGridProgram
    {
        public Program()
        {
            // Initialize your library here
            // Example: var myLib = new MyLibrary();
        }

        public void Main(string argument, UpdateType updateSource)
        {
            // Demonstrate your library usage
        }
    }
}
```

## Step 9: Test Building

Building verifies that your code compiles correctly, all references are set up properly, and the NuGet package can be generated.

```bash
# Navigate back to repository root
cd ../../..

# Restore packages
dotnet restore

# Build the solution
dotnet build --configuration Release
```

Verify there are no errors in the output.

## Step 10: Commit and Push

```bash
git add .
git commit -m "Add [YourLibraryName] mixin"
git push origin main
```

This pushes your changes to **your fork**.

## Step 11: Create a Pull Request

A pull request (PR) asks the repository maintainers to review and merge your changes into the main repository.

1. Go to your fork on GitHub: `https://github.com/YourUsername/mdk2-packages`
2. Click the **Pull requests** tab
3. Click **New pull request**
4. Ensure the base repository is `malforge/mdk2-packages` and base branch is `main`
5. Ensure the head repository is your fork and the compare branch has your changes
6. Click **Create pull request**
7. Fill in the title and description:
   - Title: `Add [YourLibraryName] mixin`
   - Description: Briefly describe what your library does
8. Click **Create pull request**

## Step 12: Wait for Review

Once your PR is submitted:
1. The automated GitHub Actions workflows will run to verify your library builds correctly
2. A maintainer will review your code
3. They may request changes or approve immediately
4. Once approved and merged, your library will be automatically:
   - Built and packaged
   - Published to GitHub Packages
   - Documented on the main README

## Naming Convention Summary

- **MdkScriptMixin**: For programmable block scripts only
- **MdkModMixin**: For mods only
- **MdkSharedMixin**: Works in both environments

## Tips

- Always include a `readme.md` file with usage examples
- Keep `_releasenotes` up to date with each version bump
- Test your library in an actual Space Engineers environment
- Follow C# 6.0 syntax for script mixins (Space Engineers limitation)
- Use semantic versioning (Major.Minor.Patch)
- Use VS Code extensions for better C# support

## Useful VS Code Extensions

- **C# for Visual Studio Code** (ms-dotnettools.csharp)
- **NuGet Package Manager** (jmrog.vscode-nuget-package-manager)
- **GitLens** (eamodio.gitlens) - Enhanced Git integration
- **Markdown All in One** (yzhang.markdown-all-in-one) - For editing readme.md

## VS Code Tips

- Press `Ctrl+Shift+P` to open the command palette
- Use `Ctrl+`` to toggle the integrated terminal
- Press `F5` to start debugging (if configured)
- Use `Ctrl+P` for quick file navigation

## Need Help?

Check out existing libraries in the repository for examples, or open an issue on GitHub!

---

*This guide is part of the MDK Package Libraries documentation.*
