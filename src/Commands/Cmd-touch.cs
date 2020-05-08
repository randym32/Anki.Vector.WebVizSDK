//  Copyright © 2020 Randall Maas. All rights reserved.
using System.Collections.Generic;
namespace Anki.Vector.WebVizSDK
{
public partial class Session
{

    /// <summary>
    /// Enables/disables the touch sensor
    /// </summary>
    /// <param name="enableDisable">true to enable the touch sensor, false to disable it</param>
    /// <returns>false if there was an error, otherwise the item was submitted</returns>
    public bool Touch(bool enableDisable)
    {
        var cmd = new Dictionary<string, object>();
        cmd["enabled"] = enableDisable?"true":"false";
        return Post("Touch", cmd);
    }

    /// <summary>
    /// Enables the touch sensor
    /// </summary>
    /// <returns>false if there was an error, otherwise the item was submitted</returns>
    public bool TouchEnable()
    {
        return Touch(true);
    }

    /// <summary>
    /// Disables the touch sensor
    /// </summary>
    /// <returns>false if there was an error, otherwise the item was submitted</returns>
    public bool TouchDisable()
    {
        return Touch(false);
    }

    /// <summary>
    /// Resets the touch sensor count
    /// </summary>
    /// <returns>false if there was an error, otherwise the item was submitted</returns>
    public bool TouchResetCount()
    {
        var cmd = new Dictionary<string, object>();
        cmd["resetCount"] = "true";
        return Post("Touch", cmd);
    }
}
}
