//  Copyright © 2020 Randall Maas. All rights reserved.
using System.Collections.Generic;

namespace Anki.Vector.WebVizSDK
{

public partial class Session
{
    /// <summary>
    /// Enable and disable power save
    /// </summary>
    /// <param name="enable">Whether or not to enable the power save</param>
    /// <returns>false if there was an error, otherwise the item was submitted</returns>
    public bool PowerSave(bool enable)
    {
        var cmd = new Dictionary<string, object>();
        cmd["enablePowerSave"] = enable;
        return Post("Power", cmd);
    }
}
}