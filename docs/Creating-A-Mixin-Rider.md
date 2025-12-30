# Creating a New Mixin Library - JetBrains Rider

This guide will walk you through creating a new mixin library for the MDK Package Libraries repository using JetBrains Rider.

## A Quick Word About Sharing Code

Hey, thanks for wanting to contribute! Sharing code with the Space Engineers community is awesome, and I really appreciate you taking the time to do it.

That said, I need to be upfront about something: **By contributing here, you're responsible for your own code and licensing.** I recommend using the MIT License (it's simple and everyone understands it), but you can choose whatever open-source license you want. Just make sure you actually have the right to share what you're contributing, and understand that if there are any copyright issues, that's on you to handle.

I'm not here to play lawyer or referee licensing disputes - I'm just providing a platform for the community. If you're cool with that, let's build something great together!

## Prerequisites

- JetBrains Rider 2023.2+
- .NET Framework 4.8 SDK
- MDK2 installed
- Git

## Step 1: Fork and Clone the Repository

1. Go to `https://github.com/malforge/mdk2-packages` and click **Fork**
2. In Rider, click **Get from VCS** on the welcome screen
3. Enter your fork's URL: `https://github.com/YourUsername/mdk2-packages`
4. Choose a local path and click **Clone**

## Step 2: Create a New Solution for Your Library

1. In Rider, go to **File → New → Solution**
2. Search for **"MDK Mixin"** template
3. Name your solution using the format:
   - `YourName.MdkScriptMixin.LibraryName` - For programmable block scripts only
   - `YourName.MdkModMixin.LibraryName` - For mods only
   - `YourName.MdkSharedMixin.LibraryName` - For both scripts and mods
4. Set location to the `libraries` folder in your cloned repository
5. **Check "Create directory for solution"** (this creates the proper folder structure)
6. Click **Create**

## Step 3: Create Required Metadata Files

In your shared project folder (the one inside the solution folder), create these files:

1. **`_version`** - Version number: `1.0.0`
2. **`_authors`** - Your name (one per line)
3. **`_description`** - One-line description of your library
4. **`_releasenotes`** - Version history (format: `- 1.0.0` then indented features)
5. **`readme.md`** - Usage examples and API docs

> **Tip:** Right-click the project → **Add → New Item → Text File** for metadata files (they have no extension)

## Step 4: Add Your Code Files

Now you'll write the actual library code that you want to share with others.

1. Right-click the shared project → **Add → C# File**
2. Name your file (e.g., `MyLibrary.cs`)
3. Write your library code - classes, methods, utilities, etc.
4. Add as many files as needed to organize your code

**Important:**
- **For script mixins**: Strongly recommended to use the `IngameScript` namespace to avoid conflicts (programmable blocks don't use namespaces, making namespace mismatches easy to cause trouble). Mixins for mods do not need to follow this convention.

**Tips:**
- Keep code well-organized and commented
- Consider edge cases and error handling
- Make your API intuitive for other developers

## Step 5: Create a Demo Project

The demo shows how to use your library and is required for packaging.

1. Right-click the solution → **Add → New Project**
2. Search for **"MDK Programmable Block Script"**
3. Name it `YourLibraryName.Demo`
4. Set location to your library's solution folder (same as the mixin project)
5. Click **Create**

## Step 6: Reference the Mixin in Demo

1. Right-click the Demo project → **Add → Reference**
2. Select **Projects** tab
3. Check your mixin project (shows as Shared Project)
4. Click **OK**

## Step 7: Write Demo Code

Edit `Program.cs` in the Demo project to show how to use your library:

```csharp
partial class Program : MyGridProgram
{
    public Program()
    {
        // Initialize your library
        var myLib = new MyLibraryClass();
    }

    public void Main(string argument, UpdateType updateSource)
    {
        // Show library usage
    }
}
```

## Step 8: Test Building

1. Press `Ctrl+Shift+B` or click **Build → Build Solution**
2. Check for errors in the Build output
3. Success means your library compiles and a NuGet package was generated

## Step 9: Commit and Push

1. Press `Ctrl+K` to open commit dialog
2. Stage all new files
3. Commit message: `Add [YourLibraryName] mixin`
4. Click **Commit**
5. Press `Ctrl+Shift+K` and click **Push**

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

## Copyright & Licensing

**Important:** By contributing to this repository, you confirm that:
- You own the copyright to the code you're contributing, OR
- You have permission to share it under your chosen license

**We strongly recommend using the MIT License** for maximum compatibility and ease of use. However, you may choose any open-source license. You are responsible for:
- Ensuring your code doesn't infringe on others' copyrights
- Maintaining and enforcing your chosen license
- Handling any licensing disputes

**The repository maintainer does not police or enforce individual library licenses and takes no responsibility for copyright or licensing issues.**

## Quick Tips

- Always include `readme.md` with usage examples
- Use semantic versioning (1.0.0 = Major.Minor.Patch)
- Test in Space Engineers before submitting
- Press `Alt+Enter` on errors for quick fixes
- Use `Ctrl+Shift+R` for refactoring options

## Need Help?

Check out existing libraries in the repository for examples, or open an issue on GitHub!

---

*This guide is part of the MDK Package Libraries documentation.*
