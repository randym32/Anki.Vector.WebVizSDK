//  Copyright © 2020 Randall Maas. All rights reserved.
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
namespace Anki.Vector.WebVizSDK
{
public partial class Session
{
    /// <summary>
    /// This maps each variable to its owning module
    /// </summary>
    readonly Dictionary<string, string> var2Module = new Dictionary<string, string>();

    /// <summary>
    /// This maps each variable to its value
    /// </summary>
    readonly Dictionary<string, object> var2Value = new Dictionary<string, object>();

    /// <summary>
    /// This maps each variable to the port that it can be accessedon
    /// </summary>
    readonly Dictionary<string, int> var2Port = new Dictionary<string, int>();


    /// <summary>
    /// This parses the initial variable value list
    /// </summary>
    /// <param name="text">The text received</param>
    /// <param name="port">The port it was received from</param>
    void ParseVarList(string text, int port)
    {
        // Sanity check
        if (null == text)
            return;

        // Go thru and split the string up into lines
        // and then by spaces
        var lines = Regex.Split(text, "\r\n|\r|\n");

        // Split each line into the parts
        //   varname [module name] = stuff
        foreach (var line in lines)
            try
            {
                // Skip empty and info lines
                if ("" == line || 0 == line.IndexOf("=="))
                    continue;

                // Split the line up
                var leftBracket = line.IndexOf('[');
                var rightBracket = line.IndexOf(']');
                var varName = line.Substring(0, leftBracket).Trim();
                var moduleName = line.Substring(leftBracket + 1, (rightBracket - leftBracket) - 1).Trim();
                var eqIndex = line.IndexOf('=', rightBracket);
                var valueStr = line.Substring(eqIndex + 1).Trim();

                // Convert the value into something we are used to
                object value = null;
                if ("false" == valueStr)
                    value = false;
                else if ("true" == valueStr)
                    value = false;
                else if (0 <= line.IndexOf('.'))
                {
                    if (float.TryParse(valueStr, out var v))
                        value = v;
                }
                else
                    if (int.TryParse(valueStr, out var v))
                    value = v;

                // Store which port and module it goes with
                var2Port  [varName] = port;
                var2Module[varName] = moduleName;
                var2Value [varName] = value;
            }
            catch (Exception e) { }
    }


    /// <summary>
    /// This is done to get the initial set of 
    /// </summary>
    /// <returns></returns>
    async Task InitialVarList()
    {
        // Get the list of variables for each port
        // For both ports
        ParseVarList(await HTTPFetch(8888, "/consolefunccall?func=List_Variables"), 8888);
        ParseVarList(await HTTPFetch(8889, "/consolefunccall?func=List_Variables"), 8889);
        ReceivedVars();
    }


    /// <summary>
    /// This is used to set a console variable to the given vlaue
    /// </summary>
    /// <param name="varName">The console variable to set</param>
    /// <param name="value">The </param>
    /// <returns></returns>
    public async Task SetConsoleVar(string varName, object value)
    {
        // The resource name to send
        var resourceName = String.Format("/consolevarset?key={0}&value={1}"
            , Uri.EscapeDataString(varName)
            , Uri.EscapeDataString(value.ToString())
            );

        // And set the GET to change the value
        await HTTPFetch(var2Port[varName], resourceName);
        var2Value[varName] = value;
    }


    /// <summary>
    /// This is used to reset all of the console variables to their default values
    /// </summary>
    /// <returns></returns>
    public async Task ResetConsoleVars()
    {
        await HTTPFetch(8888, "/consolefunccall?func=ResetConsoleVars");
        await HTTPFetch(8889, "/consolefunccall?func=ResetConsoleVars");
    }
}
}
