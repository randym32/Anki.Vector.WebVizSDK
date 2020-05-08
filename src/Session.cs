//  Copyright © 2020 Randall Maas. All rights reserved.
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using WebSocket4Net;

namespace Anki.Vector.WebVizSDK
{
/// This handles the session of multiple Vetor event streams on the websocket.
/// </summary>
/// <remarks>
/// This could bear a going over an rewrite ("refactor") to better work with
/// Wayne Venable's C# Vector SDK
/// </remarks>
public partial class Session: IDisposable
{
    /// <summary>
    /// The shared JSON deserialization options
    /// </summary>
    static readonly JsonSerializerOptions JSONOptions;

    /// <summary>
    /// Maps the module name to the port to use
    /// </summary>
    static readonly Dictionary<string, int> module2Port;

    /// <summary>
    /// The websocket handler for port 8888
    /// </summary>
    WebSocket client8888;

    /// <summary>
    /// The websocket handler for port 8889
    /// </summary>
    WebSocket  client8889;

    /// <summary>
    /// This is used to set up the shared resources
    /// </summary>
    static Session()
    {
        // Set up the JSON options
        JSONOptions = new JsonSerializerOptions
        {
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true,
        };

        // Read the configuration JSON
        // Get the text file
        var temp = new global::System.Resources.ResourceManager("Anki.Vector.WebVizSDK.Properties.Resources", typeof(Session).Assembly);
        var text = temp.GetString("webViz.json");
        // Get the dictionary
        module2Port = JsonSerializer.Deserialize<Dictionary<string, int>>(text, JSONOptions);
    }


    /// <summary>
    /// The IP address for the Vector
    /// </summary>
    readonly string serverIP ="192.168.0.1";

    /// <summary>
    /// Set up to use websocket to the given address
    /// </summary>
    /// <param name="serverIP">The address to connect with</param>
    public Session(string serverIP)
    {
        this.serverIP = serverIP;

        // Creat event handlers for each of the modules
        foreach (var moduleName in module2Port.Keys)
            moduleEventHandler[moduleName.ToLower()] = new ModuleEventElement();
    }


    // Public implementation of Dispose pattern callable by consumers.
    public void Dispose()
    {
        // Dispose of unmanaged resources.
        Dispose(true);
        // Suppress finalization.
        GC.SuppressFinalize(this);
    }

    // Flag: Has Dispose already been called?
    bool disposed = false;

    // Protected implementation of Dispose pattern.
    protected virtual void Dispose(bool disposing)
    {
        if (disposed)
            return;

        if (disposing)
        {
            // Close the sockets
            // Note: it isn't clear if we should (let alone need) to post
            // unsubscribe messages to vector
            if (null != client8888) client8888.Dispose();
            client8888 = null;
            if (null != client8889) client8889.Dispose();
            client8889 = null;
        }

        disposed = true;
    }

    /// <summary>
    /// This is used to fetch a list of module names that are supported
    /// </summary>
    /// <returns></returns>
    public static IReadOnlyCollection<string> Modules()
    {
        return new List<string>(module2Port.Keys);
    }


    /// <summary>
    /// Returns the websocket interface to the given port.
    /// If one does not already exist, it is created
    /// </summary>
    /// <param name="port">The port to connect to</param>
    /// <returns>null on error, otherwise a client to the websocket</returns>
    WebSocket SocketForPort(int port)
    {
        if (8888 == port)
        {
            // reuse the websocket if it has already been created
            if (null != client8888)
                return client8888;
        }
        else if (8889 == port)
        {
            // reuse the websocket if it has already been created
            if (null != client8889)
                return client8889;
        }
        else
            return null;


        // Create a web socket and
        // Set up the socket's listeners
        var client = new WebSocket($"ws://{serverIP}:{port}/socket");

        // Hook in the listeners
        client.Opened += OnConnect;
        client.Closed += OnDisconnect;
        client.Error += Error;
        client.DataReceived += DataReceived;
        client.MessageReceived += MessageReceived;

        // Keep the connection
        if (8888 == port)
        {
            client8888 = client;
        }
        else
        {
            client8889 = client;
        }

        // Connect, if there isn't a problem
        client.Open();
        return client;
    }


    /// <summary>
    /// You should call this after creating the object in order to set up an
    /// initial state of affairs.
    /// </summary>
    /// <remarks>
    /// The connection is not guaranteed to be set up when this returns. You
    /// should set a listener for the TBD Connected event, and use that to know
    /// that connection has been established.
    /// </remarks>
    public void ConnectBegin()
    {
        // Contact and get the initial state, in the background
        _ = Task.Run(async () =>
          {
              await InitialVarList();
          });

        // Connect the websockets
        SocketForPort(8888);
        SocketForPort(8889);
    }


    /// <summary>
    /// Returns the websocket interface to the given port.
    /// If one does not already exist, it is created
    /// </summary>
    /// <param name="moduleName">The module that we wish to find a connection for</param>
    /// <returns>null on error, otherwise a client to the websocket</returns>
    WebSocket SocketForModule(string moduleName)
    {
        // Look up the port for the module
        if (module2Port.TryGetValue(moduleName, out var port))
        {
            // Look up the connection
            return SocketForPort(port);
        }

        // Wasn't able to find find a socket for that module
        return null;
    }


    /// <summary>
    /// This is used to subscribe/subscribe/post to the given module
    /// </summary>
    /// <param name="cmd">The command, e.g. subscribe, unsubscribe, data</param>
    /// <param name="moduleName">The module that we wish to message</param>
    /// <param name="payload">Optional data to send</param>
    /// <returns>false on error, otherwise submitted to be sent</returns>
    public bool Cmd(string cmd, string moduleName, object payload=null)
    {
        // Get the web socket
        var client = SocketForModule(moduleName);
        if (null == client)
            return false;

        // Create the frame to subscribe
        var frame = new Dictionary<string, object>();
        frame["type"] = cmd;
        frame["module"] = moduleName;
        if (null != payload)
            frame["data"] = payload;

        // Covnert that to bytes and send it
        var s = JsonSerializer.Serialize(frame, JSONOptions);
        client.Send(s);
        return true;
    }


    /// <summary>
    /// A helper to keep track of which modules have alrady been subscribed to
    /// </summary>
    readonly Dictionary<string, object> subscribedModules = new Dictionary<string, object>();


    /// <summary>
    /// This is used to subscribe to events from the given module
    /// </summary>
    /// <param name="moduleName">The module that we wish to subscribe to events from</param>
    /// <returns>false on error, otherwise success</returns>
    public bool Subscribe(string moduleName)
    {
        // Check to see if it is already subscribed
        if (subscribedModules.ContainsKey(moduleName))
            return true;
        if (Cmd("subscribe", moduleName))
        {
            // Make a note that we have subscribed to the module
            subscribedModules[moduleName] = moduleName;
            return true;
        }

        // The subscribe failed
        return false;
    }


    /// <summary>
    /// This is used to unsubscribe to events from the given module
    /// </summary>
    /// <param name="moduleName">The module that we wish to unsubscribe to events from</param>
    /// <returns>false on error, otherwise success</returns>
    public bool Unsubscribe(string moduleName)
    {
        if (Cmd("unsubscribe", moduleName))
        {
            // Make a note that we have unsubscribed to the module
            subscribedModules.Remove(moduleName);
            return true;
        }

        // The unsubscribe failed
        return false;
    }


    /// <summary>
    /// This is used to post a command to a module
    /// </summary>
    /// <param name="moduleName">The module that we wish to command</param>
    /// <param name="payload">The command parameters</param>
    /// <returns>false on error, otherwise success</returns>
    public bool Post(string moduleName, object payload)
    {
        return Cmd("data", moduleName, payload);
    }

    /// <summary>
    /// This a helper to allow listening for events from the modules
    /// </summary>
    readonly Dictionary<string, ModuleEventElement> moduleEventHandler = new Dictionary<string, ModuleEventElement>();

    /// <summary>
    /// This lets a C# program  add event handlers to module events, and
    /// automatically subscribes to the event
    /// </summary>
    /// <param name="moduleName">The name of a module to added a handler for</param>
    /// <param name="handler">The event handler</param>
    /// <returns>null if the module name isn't recognized, otherwise the delegate hook</returns>
    public ModuleEventElement OnModuleChange(string moduleName, ModuleEventElement.EventDelegate handler)
    {
        moduleName = moduleName.ToLower();
        // Look up the handlers for the module
        if (moduleEventHandler.TryGetValue(moduleName, out var handlers))
        {
            // Add the listener
            handlers += handler;

            // Subscribe to the events, if not already
            Subscribe(moduleName);
            return handlers;
        }

        // Perhaps return a dummy object to allow hooking
        return null;
    }

    void Error(object sender, SuperSocket.ClientEngine.ErrorEventArgs args)
    {
    }


    void DataReceived(object sender, DataReceivedEventArgs args)
    {
    }


    /// <summary>
    /// This is called when a message is received from the Vector
    /// </summary>
    /// <param name="sender">The websocket client that sent it</param>
    /// <param name="args"></param>
    void MessageReceived(object sender, MessageReceivedEventArgs args)
    {
        // Get the data as a JSON string
        var frameJSON = args.Message;// Encoding.UTF8.GetString(args.Data);

        // Get the dictionary from the JSON
        var frame = Util.toDict(JsonSerializer.Deserialize<Dictionary<string, object>>(frameJSON, JSONOptions));

        // look at the source
        if (frame.TryGetValue("module", out var moduleName)
           && frame.TryGetValue("data", out var data)
            )
        {
            // moduleName: the source that sent the event
            // Look up the handlers for the module
            if (moduleEventHandler.TryGetValue(((string)moduleName).ToLower(), out var handlers))
            {
                // Call the handlers
                handlers.Invoke(this, new ModuleEventArgs((string)moduleName, data));
            }
        }
    }

    /// <summary>
    /// This event is invoked when a connection has been established
    /// </summary>
    public event EventHandler Connected;

    /// <summary>
    /// This event is invoked when a connection has been lost
    /// </summary>
    public event EventHandler Disconnected;

    /// <summary>
    /// This is set to true when the variable lists from the Vector have been received
    /// </summary>
    bool hasVars = false;

    /// <summary>
    /// True if the client has connected (not just started to connect)
    /// </summary>
    bool connected8888 = false;
    bool connected8889 = false;


    /// <summary>
    /// This is internally called by each of the websockets as it establishes
    /// a connection.  When the variable list has been received, and all
    /// websockets have connected, it will emit a Connected event.
    /// </summary>
    /// <param name="sender">The websocket client that sent it</param>
    /// <param name="args"></param>
    void OnConnect(object sender, EventArgs args)
    {
        if (client8888 == sender)
            connected8888 = true;
        if (client8889 == sender)
            connected8889 = true;
        if (connected8888 && connected8889 && hasVars)
            if (null != Connected)
            {
                Console.WriteLine("Server connected");
                Connected.Invoke(sender, EventArgs.Empty);
            }
    }

    /// <summary>
    /// This is called when all of the console variables have been received from
    /// the Vector.  It may post the connected event to the application
    /// </summary>
    void ReceivedVars()
    {
        hasVars = true;
        if (connected8888 && connected8889 && hasVars)
            if (null != Connected)
            {
                Console.WriteLine("Server connected");
                Connected.Invoke(this, EventArgs.Empty);
            }
    }


    /// <summary>
    /// This is internally called by each of the websockets when they are closed
    /// </summary>
    /// <param name="sender">The websocket client that sent it</param>
    /// <param name="args"></param>
    void OnDisconnect(object sender, EventArgs args)
    {
        if (client8888 == sender)
        {
            client8888.Dispose();
            connected8888 = false;
            client8888    = null;
        }
        if (client8889 == sender)
        {
            client8888.Dispose();
            connected8889 = false;
            client8889    = null;
        }
        if (null != Disconnected)
            Disconnected.Invoke(sender, EventArgs.Empty);
        Console.WriteLine("Server disconnected");
    }


    /// <summary>
    /// A wrapper to send / fetch data to the far side
    /// </summary>
    /// <param name="Port">The port to send to</param>
    /// <param name="resourcePath">The resource path to use, including GET</param>
    /// <returns>NULL on error or no string, otherwise the </returns>
    async Task<string> HTTPFetch(int Port, string resourcePath)
    {
        using (var client = new HttpClient())
        {
            client.BaseAddress = new Uri("http://" + serverIP+":"+Port);
            client.DefaultRequestHeaders.Add("User-Agent", "Anything");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Send the request and get the data
            var response = await client.GetAsync(resourcePath);

            // Check the status code
            if (HttpStatusCode.OK != response.StatusCode)
                return null;

            // Convert to a string

            return await response.Content.ReadAsStringAsync();
        }
    }
}
}