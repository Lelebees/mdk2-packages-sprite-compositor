# Installing Mal.MdkScriptMixin.Coroutines

**Quick Links:** [Visual Studio](#visual-studio) | [JetBrains Rider](#jetbrains-rider) | [Command Line](#command-line--manual)

---

## Visual Studio

1. Right-click on your project in Solution Explorer
2. Select **Manage NuGet Packages**
3. Click on **Settings** (gear icon)
4. Add a new package source:
   - Name: `Malforge`
   - Source: `https://nuget.pkg.github.com/malforge/index.json`
5. Click **OK** to save
6. Search for `Mal.MdkScriptMixin.Coroutines`
7. Click **Install**

---

## JetBrains Rider

1. Go to **File → Settings → NuGet → Sources**
2. Click **+** to add a new source
   - Name: `Malforge`
   - URL: `https://nuget.pkg.github.com/malforge/index.json`
3. Click **OK** to save
4. Open the **NuGet** tool window
5. Search for `Mal.MdkScriptMixin.Coroutines`
6. Click **Install**

---

## Command Line / Manual

### Add Package Source (one time only)

```bash
dotnet nuget add source https://nuget.pkg.github.com/malforge/index.json --name Malforge
```

### Install the Package

```bash
dotnet add package Mal.MdkScriptMixin.Coroutines
```

### Or Edit .csproj Directly

Add this to your `.csproj` file:

```xml
<PackageReference Include="Mal.MdkScriptMixin.Coroutines" Version="1.0.6" />
```

---

*Installation instructions auto-generated. Last updated: 2025-12-30*
