# Setting Up GitHub Packages as a NuGet Source

One-time setup to access the mixin libraries in this repository.

## Why GitHub Packages?

These mixins are published to GitHub Packages rather than NuGet.org because they're highly specialized for Space Engineers development. Publishing them to the public NuGet feed would add unnecessary clutter, while GitHub Packages provides a more appropriate home for game-specific tooling and keeps them easily accessible to the SE modding community.

Note: GitHub Packages requires authentication even for public packages.

## Prerequisites

- GitHub account (free account works fine - [sign up here](https://github.com/signup))
- Visual Studio, Rider, or .NET CLI installed

## Step 1: Create a Personal Access Token (PAT)

1. Go to https://github.com/settings/tokens
2. Click **Generate new token** → **Generate new token (classic)**
3. Name it (e.g., "NuGet Package Access")
4. Set expiration (recommendation: 90+ days)
5. Check **read:packages** scope
6. Click **Generate token**
7. **Copy the token immediately** - you won't see it again!

## Step 2: Add GitHub Packages as a NuGet Source

Choose your preferred method:

### Visual Studio

1. **Tools** → **Options** → **NuGet Package Manager** → **Package Sources**
2. Click **+** to add a new source
3. **Name:** `GitHub Malforge`
4. **Source:** `https://nuget.pkg.github.com/malforge/index.json`
5. Click **Update**, then **OK**
6. Enter credentials when prompted:
   - **Username:** Your GitHub username
   - **Password:** Your PAT from Step 1

### JetBrains Rider

1. **File** → **Settings** → **Build, Execution, Deployment** → **NuGet** → **Sources**
2. Click **+** to add a new source
3. **Name:** `GitHub Malforge`
4. **URL:** `https://nuget.pkg.github.com/malforge/index.json`
5. Check **Requires authentication**
6. Enter credentials:
   - **Username:** Your GitHub username
   - **Password:** Your PAT from Step 1
7. Click **OK**

### Command Line (VS Code and others)

VS Code doesn't have a built-in NuGet package manager. Use the command line:

```
dotnet nuget add source https://nuget.pkg.github.com/malforge/index.json --name "GitHub Malforge" --username YOUR_GITHUB_USERNAME --password YOUR_PAT --store-password-in-clear-text
```

Replace `YOUR_GITHUB_USERNAME` and `YOUR_PAT` with your actual values.

## Step 3: Verify Setup

Test by installing a package in an MDK project:

```
dotnet add package Mal.MdkScriptMixin.Coroutines
```

If successful, you're all set!

## Troubleshooting

**"Unable to load the service index"**
- Verify URL is exactly: `https://nuget.pkg.github.com/malforge/index.json`

**"401 Unauthorized" errors**
- Check your PAT has `read:packages` scope
- Verify GitHub username is correct (case-sensitive)
- Ensure PAT hasn't expired

**IDE not prompting for credentials**
- Use command line method first, then restart your IDE

## Security Notes

- Never commit your PAT to source control
- Revoke compromised PATs immediately in GitHub settings
- Set expiration dates and create renewal reminders
