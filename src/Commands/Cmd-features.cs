//  Copyright © 2020 Randall Maas. All rights reserved.
using System.Collections.Generic;
namespace Anki.Vector.WebVizSDK
{
public partial class Session
{
    /// <summary>
    /// Whether to enable or disable an AI feature
    /// </summary>
    /// <param name="name">The name of the AI feature</param>
    /// <param name="_override">none/enabled/disabled</param>
    /// <returns>false if there was an error, otherwise the item was submitted</returns>
    public bool feature(string name, string _override)
    {
        var cmd = new Dictionary<string, object>();
        cmd["name"] = name;
        cmd["override"] = _override;
        cmd["type"] = "override";
        return Post("Features", cmd);
    }

    /// <summary>
    /// Resets an AI feature to its default value
    /// </summary>
    /// <param name="name">The name of the AI feature</param>
    /// <returns>false if there was an error, otherwise the item was submitted</returns>
    public bool featureReset(string name)
    {
        return feature(name, "none");
    }

    /// <summary>
    /// Enables an AI feature
    /// </summary>
    /// <param name="name">The name of the AI feature</param>
    /// <returns>false if there was an error, otherwise the item was submitted</returns>
    public bool featureEnable(string name)
    {
        return feature(name, "enabled");
    }

    /// <summary>
    /// Disables an AI feature
    /// </summary>
    /// <param name="name">The name of the AI feature</param>
    /// <returns>false if there was an error, otherwise the item was submitted</returns>
    public bool featureDisable(string name)
    {
        return feature(name, "disabled");
    }


    /// <summary>
    /// Reset all to their default state
    /// </summary>
    /// <param name="name">The name of the AI feature</param>
    /// <returns>false if there was an error, otherwise the item was submitted</returns>
    public bool featureResetAll(string name)
    {
        var cmd = new Dictionary<string, object>();
        cmd["type"] = "reset";
        return Post("Features", cmd);
    }
}
}