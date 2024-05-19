# XeroToGRT

This tool is intended for processing FIT files produced by the Garmin Xero C1 Pro chronometer. 
The output format is a CSV file which is supported by Gordon's Reloading Tool.

The solution has been created with Visual Studio 2022 Community edition. It creates a .NET 8 executable.
Naturally it's also possible to build it without VS 2022 with `dotnet build`.

To create a self-contained executable for the desired platform, you can use `dotnet publish`. 
An example for Windows:

```
dotnet publish -r win-x64 --self-contained -p:PublishSingleFile=true .\XeroToGRT\XeroToGRT.csproj
```