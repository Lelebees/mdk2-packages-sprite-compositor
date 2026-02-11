# MDK Package Libraries

Welcome to the Malforge MDK Package Libraries! This repository contains reusable code libraries (mixins) for Space Engineers, designed to work with [Malware's Development Kit (MDK)](https://github.com/malware-dev/mdk2).

## Getting Started

**Important:** Before you can use these libraries, you need to configure GitHub Packages as a NuGet source. This is a one-time setup that takes just a few minutes.

👉 **[Follow the setup guide here](./docs/Setting-Up-GitHub-Packages.md)**

Once configured, you can install any library from this repository using your IDE's NuGet package manager.

## What Are Mixins?

Mixins are reusable code libraries that get compiled directly into your Space Engineers projects. Since SE doesn't support external DLL references, mixins provide the only practical way to share code between scripts and mods. [Learn more about how mixins work](./docs/Understanding-Mixins.md).

## Library Naming Convention

All libraries follow the MDK naming convention to clearly indicate where they can be used:

- **MdkScriptMixin** - For programmable block scripts only
- **MdkModMixin** - For mods only
- **MdkSharedMixin** - For libraries that work in both environments

This naming helps you quickly identify which libraries are suitable for your project type.

## Available Libraries

| Package | Version | Author(s) | Description |
|---------|---------|-----------|-------------|
| [Mal.MdkScriptMixin.Coroutines](./docs/libraries/Mal.MdkScriptMixin.Coroutines.md) | 1.0.9 | Malware | A lightweight coroutine scheduler for Space Engineers programmable blocks, enabling async-style programming patterns. |
| [Mal.MdkScriptMixin.Graphics](./docs/libraries/Mal.MdkScriptMixin.Graphics.md) | 1.0.0 | Malware | A graphics library for Space Engineers providing high-level drawing abstractions, aspect-aware viewports, and an advanced paint system for creating responsive UIs. |
| [Mal.MdkScriptMixin.Stopwatch](./docs/libraries/Mal.MdkScriptMixin.Stopwatch.md) | 1.0.0 | Malware | A lightweight timing utility for measuring elapsed game time across script runs. |

**Click on any library name above** to see its documentation, including detailed installation instructions for Visual Studio, Rider, and VS Code.

## Contributing Your Own Library

Want to share your own Space Engineers code libraries? We'd love to have your contributions! 

See our guides for creating mixins:
- [Creating a Mixin with Visual Studio](./docs/Creating-A-Mixin-VisualStudio.md)
- [Creating a Mixin with Rider](./docs/Creating-A-Mixin-Rider.md)
- [Creating a Mixin with VS Code](./docs/Creating-A-Mixin-VSCode.md)

Once your library is ready, submit a pull request to this repository. All contributions are reviewed to ensure quality and consistency.

## Resources

- [Understanding Mixins](./docs/Understanding-Mixins.md) - Learn how mixins work and why they're perfect for SE
- [MDK Documentation](https://malforge.github.io/spaceengineers/mdk2/) - Full MDK documentation
- [MDK GitHub](https://github.com/malware-dev/mdk2) - MDK source code

## Legal Stuff (Unfortunately Necessary)

Look, I hate having to add this section. I'd rather just share cool code with fellow Space Engineers players without worrying about legal nonsense. But the reality of the internet is that you have to protect yourself, even when you're just trying to help people out for free.

**The bottom line:** This repository is a community platform. I'm not responsible for what other people contribute here. Each library comes from its respective author(s), and they're responsible for their own code and licensing.

**Disclaimer:** This repository is a community platform for sharing Space Engineers code libraries. The repository maintainer is not responsible for the content, licensing, or copyright status of contributed libraries. Each library is provided by its respective author(s) as indicated in the package metadata.

**Use at your own risk:** All libraries are provided "as-is" without warranty of any kind. Users should review each library's license and code before use.

---

*This README was auto-generated from library metadata. Last updated: 2026-02-11*

