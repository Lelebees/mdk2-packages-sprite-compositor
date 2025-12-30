# Creating a New Mixin Library - JetBrains Rider

This guide will walk you through creating a new mixin library for the MDK Package Libraries repository using JetBrains Rider.

## Prerequisites

- JetBrains Rider 2023.2 (or later)
- .NET Framework 4.8 SDK
- **MDK2 installed** (required to use these libraries)
- Git

## Step 1: Fork and Clone the Repository

You don't have direct write access to the main repository. Forking creates your own copy where you can make changes, then submit them as a pull request.

1. Go to `https://github.com/malforge/mdk2-packages` in your browser
2. Click the **Fork** button in the top right
3. GitHub will create a fork under your account
4. Open Rider
5. On the welcome screen, click **Get from VCS**
6. Enter **your fork's URL**: `https://github.com/YourUsername/mdk2-packages`
7. Choose a local path and click **Clone**

## Step 2: Create the Library Folder Structure

1. In File Explorer, navigate to the repository's `libraries` folder
2. Create a new folder with your library name following the naming convention:
   - `YourName.MdkScriptMixin.LibraryName` - For programmable block scripts
   - `YourName.MdkModMixin.LibraryName` - For mods
   - `YourName.MdkSharedMixin.LibraryName` - For libraries that work in both
3. This will be your library's project folder
   - Example: `libraries/Alice.MdkScriptMixin.DataGrid/`

## Step 3: Create the Mixin Project (Shared Project)

This is where you'll write your library code. The MDK Mixin template sets up a [Shared Project](Understanding-Mixins.md) with all the necessary files and structure.

1. In Rider, right-click the solution in the Solution Explorer
2. Select **Add → New Project**
3. Search for **"MDK Mixin"** template
4. Set the project name to match your library name (e.g., `Alice.MdkScriptMixin.DataGrid`)
5. Set the location to the folder you created in Step 2
6. Click **Create**

The template will create a Shared Project with all the necessary structure pre-configured.

## Step 4: Create Required Metadata Files

In the shared project folder, create these text files with exact names (no file extensions except `readme.md`):

### `_version`
```
1.0.0
```

### `_authors`
```
Your Name
Co-Author Name (if any)
```

### `_description`
```
A brief description of what your library does.
```

### `_releasenotes`
```
- 1.0.0
  Initial release
  Feature 1 added
  Feature 2 added
```

### `readme.md`
Create a comprehensive readme with:
- Library overview
- Features
- Usage examples
- API documentation

**Important:** Always include a readme.md to avoid public shaming in the documentation! 😄

## Step 5: Add Your Code Files

1. Right-click the shared project
2. Select **Add → C# File**
3. Name your file and click **OK**
4. Add your library code
5. Remember: Keep it compatible with Space Engineers restrictions if targeting MdkScriptMixin (C# 6.0, limited APIs)

## Step 6: Create a Demo Project

The Demo Project serves three purposes:
1. **Testing**: It lets you test your library code as if it were being used in a real Space Engineers script
2. **Documentation**: It shows other developers how to use your library
3. **Packaging**: The build system uses it to package your library as a NuGet package

Think of it as a sample Space Engineers script that uses your library.

1. Right-click the solution
2. Select **Add → New Project**
3. Search for **"MDK Programmable Block Script"** template
4. Set the project name to `YourLibraryName.Demo`
5. Set the location to the same folder as your mixin (from Step 2)
6. Click **Create**

The template will create a fully configured demo project with all MDK packages already included.

## Step 7: Reference the Mixin in Demo

This connects your Demo project to your library code. When you build the Demo, it will include all the code from your Shared Project, just like a real user's script would.

1. Right-click the Demo project
2. Select **Add → Reference**
3. In the dialog, select the **Projects** tab
4. Check your mixin project (it should show as a Shared Project)
5. Click **OK**

## Step 8: Create Demo Code

The MDK template already created a `Program.cs` with the basic structure. Modify it to demonstrate your library - this helps other developers understand how to use it and serves as a working test.

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
            // Example: var myLib = new MyLibraryClass();
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

1. Click **Build → Build Solution** (or press `Ctrl+Shift+B`)
2. Verify there are no errors in the Build output window
3. The build should:
   - Compile your shared project
   - Compile your demo project
   - Generate a NuGet package in the output folder

## Step 10: Commit and Push

1. Go to **VCS → Commit** (or press `Ctrl+K`)
2. Review and stage all your new files
3. Write a commit message: `Add [YourLibraryName] mixin`
4. Click **Commit**
5. Go to **VCS → Git → Push** (or press `Ctrl+Shift+K`)
6. Click **Push** to upload to **your fork**

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
- Rider has excellent Git integration - use it!

## Rider-Specific Features

- **Code Analysis**: Rider will highlight potential issues in your code
- **Quick Fixes**: Press `Alt+Enter` on warnings/errors for quick fixes
- **Refactoring**: Use `Ctrl+Shift+R` for advanced refactoring options
- **TODO Explorer**: Keep track of pending work with TODO comments

## Need Help?

Check out existing libraries in the repository for examples, or open an issue on GitHub!

---

*This guide is part of the MDK Package Libraries documentation.*
