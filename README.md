# MDK2 Packages

**MDK2 Packages** is a collection of **mixins**—reusable source code packages  
designed to enhance **Space Engineers** scripts and mods. These packages  
provide useful functionality without enforcing a rigid framework.

## Using the Packages

MDK2 Packages are distributed as **NuGet packages** hosted on GitHub.  
To use them, you need to add this repository as a **NuGet source**  
in your preferred IDE.

### Adding the GitHub Package Source

#### **Visual Studio**
1. Open **Tools > NuGet Package Manager > Package Manager Settings**.
2. Select **Package Sources** and click **Add**.
3. Set the source URL to:

   ~~~
   https://nuget.pkg.github.com/malforge/index.json
   ~~~

4. Save and close the settings.

#### **VS Code (with .NET CLI)**
Run the following command:

~~~
dotnet nuget add source "https://nuget.pkg.github.com/malforge/index.json" \
  --name "mdk2-packages"
~~~

#### **JetBrains Rider**
1. Open **File > Settings > Build, Execution, Deployment > NuGet**.
2. Click **Add** and enter the source URL:

   ~~~
   https://nuget.pkg.github.com/malforge/index.json
   ~~~

3. Save and close the settings.

Once the source is added, you can install packages from Malforge  
directly through your IDE’s NuGet Package Manager.

## Contributing

Want to submit a mixin? See our [contribution guide](CONTRIBUTING.md)  
for details on how to package and publish your work.

## Licensing

Malforge **does not own** any of the mixins in this repository. Each package  
is owned by its respective contributor and is licensed accordingly.  
Licenses may vary, but all packages are **freely usable** in Space Engineers  
scripts and mods.

## Disclaimer

Malforge provides a **repository** for community-created libraries  
but does not guarantee their functionality, safety, or compatibility.  
Contributors are responsible for their own work.

---  

🚀 **Ready to get started? Add the NuGet source and explore the available packages!**  
