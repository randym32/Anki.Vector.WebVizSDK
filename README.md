# Anki.Vector.WebVizSDK
Anki Vector .NET SDK (unofficial) to the developer-unit WebViz API.

*__WARNING__  This API is unlikely to work or be useful to you.*

The Vector SDK gives you direct access to [Anki Vector](https://www.anki.com/en-us/vector)'s internal modules within the developer builds.  This will work *only* with a developer build, and only a handful of such units have been found.

The name is patterned after [Anki.Vector.SDK](https://github.com/codaris/Anki.Vector.SDK/).
Be glad I didn't call it "Toad (The Websocket)".

## Getting started
Credits: this section lifted from the [Anki.Vector.SDK Readme](https://github.com/codaris/Anki.Vector.SDK/blob/master/README.md)

### Download Microsoft development tools

If you working on Windows, download Visual Studio 2019 Community Edition to get started.  This version is free for personal use.

* [Download Visual Studio 2019 Community Edition](https://visualstudio.microsoft.com/thank-you-downloading-visual-studio/?sku=Community)

To get started on Mac and Linux, you can download .NET Core 3.0.  

* [Download .NET Core 3.0](https://dotnet.microsoft.com/download/dotnet-core/3.0)

### Example

```csharp
    using Anki.Vector.WebVizSDK;
    Session session;

...

    session = new Session("192.168.1.179");
    session.Connected += (sender, args) =>
    {
        session.OnModuleChange("AudioEvents", Listener);
        session.OnModuleChange("MicData", Listener);
        session.OnModuleChange("SoundReactions", Listener);
        session.OnModuleChange("SpeechRecognizerSys", Listener);
    };
    // Begin the Connection with the Vector unit
    session.ConnectBegin();

...
    void Listener(object sender, ModuleEventArgs args)
    {
        var s = JsonSerializer.Serialize(args.data);
        Console.WriteLine($"{args.moduleName}-> {s}");
    }
```

## Contributions

1. Fork the project
2. Make your proposed changes
3. Create a pull request

## Authors

* **Randall Maas** 

## License

This project is licensed under the BSD 2-Clause License - see the [LICENSE](LICENSE) file for details.  Nothing particular about it; I just needed a license.

The code can be integrated into other SDKs, such as Wayne Venables's Anki.Vector.SDK, I'll relicense the code for that.  
