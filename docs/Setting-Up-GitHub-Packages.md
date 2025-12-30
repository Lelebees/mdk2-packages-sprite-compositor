# Setting Up GitHub Packages as a NuGet Source

Before you can use the mixins in this repository, you need to configure your development environment to access GitHub Packages. This is a one-time setup process.

## Why This is Required

The mixins in this repository are published to GitHub Packages, not the public NuGet.org feed. GitHub Packages requires authentication to download packages, even for public repositories.

We've chosen not to publish these packages to NuGet.org because they are highly specialized for Space Engineers development and would add unnecessary clutter to the public NuGet feed. GitHub Packages provides a more appropriate home for game-specific tooling while keeping them easily accessible to the SE modding community.

## Prerequisites

- A GitHub account (free tier is fine)
- Visual Studio, Rider, or the .NET CLI installed

## Step 1: Create a Personal Access Token (PAT)

1. Log in to GitHub and navigate to **Settings** → **Developer settings** → **Personal access tokens** → **Tokens (classic)**
   - Direct link: https://github.com/settings/tokens
2. Click **Generate new token** → **Generate new token (classic)**
3. Give your token a descriptive name (e.g., "NuGet Package Access")
4. Set expiration as desired (recommendation: 90 days or more)
5. Select the following scope:
   - ☑️ **read:packages** - Download packages from GitHub Package Registry
6. Click **Generate token**
7. **Important:** Copy the token immediately - you won't be able to see it again!

## Step 2: Add GitHub Packages as a NuGet Source

### Option A: Visual Studio

1. Go to **Tools** → **Options** → **NuGet Package Manager** → **Package Sources**
2. Click the **+** button to add a new source
3. Set **Name:** `GitHub Malforge`
4. Set **Source:** `https://nuget.pkg.github.com/malforge/index.json`
5. Click **Update**, then **OK**
6. When prompted for credentials:
   - **Username:** Your GitHub username
   - **Password:** The PAT you created in Step 1

### Option B: JetBrains Rider

1. Go to **File** → **Settings** → **Build, Execution, Deployment** → **NuGet** → **Sources**
2. Click the **+** button to add a new source
3. Set **Name:** `GitHub Malforge`
4. Set **URL:** `https://nuget.pkg.github.com/malforge/index.json`
5. Check **Requires authentication**
6. Enter credentials:
   - **Username:** Your GitHub username
   - **Password:** The PAT you created in Step 1
7. Click **OK**

### Option C: Command Line (All IDEs)

1. Add the package source:
   ```bash
   dotnet nuget add source https://nuget.pkg.github.com/malforge/index.json --name "GitHub Malforge" --username YOUR_GITHUB_USERNAME --password YOUR_PAT --store-password-in-clear-text
   ```
   
   Replace `YOUR_GITHUB_USERNAME` and `YOUR_PAT` with your actual GitHub username and the token from Step 1.

2. **Alternative (More Secure):** Store credentials in NuGet config manually:
   - Windows: `%AppData%\NuGet\NuGet.Config`
   - Mac/Linux: `~/.nuget/NuGet/NuGet.Config`
   
   Add this to your config file:
   ```xml
   <?xml version="1.0" encoding="utf-8"?>
   <configuration>
     <packageSources>
       <add key="GitHub Malforge" value="https://nuget.pkg.github.com/malforge/index.json" protocolVersion="3" />
     </packageSources>
     <packageSourceCredentials>
       <GitHub_x0020_Malforge>
         <add key="Username" value="YOUR_GITHUB_USERNAME" />
         <add key="ClearTextPassword" value="YOUR_PAT" />
       </GitHub_x0020_Malforge>
     </packageSourceCredentials>
   </configuration>
   ```

## Step 3: Verify Setup

1. Create or open an MDK script project
2. Try installing a package:
   ```bash
   dotnet add package Mal.MdkScriptMixin.Coroutines
   ```
3. If successful, you're all set! If you get authentication errors, double-check your PAT and credentials.

## Troubleshooting

**Problem:** "Unable to load the service index for source"
- **Solution:** Verify the URL is exactly `https://nuget.pkg.github.com/malforge/index.json`

**Problem:** "401 Unauthorized" errors
- **Solution:** 
  - Check that your PAT has the `read:packages` scope
  - Verify your GitHub username is correct (case-sensitive)
  - Make sure the PAT hasn't expired

**Problem:** Visual Studio/Rider not prompting for credentials
- **Solution:** Use the command line method to store credentials first, then restart your IDE

## Security Notes

- Never commit your PAT to source control
- If your PAT is compromised, revoke it immediately in GitHub settings and generate a new one
- Consider setting an expiration date on your PAT and creating calendar reminders to renew it
