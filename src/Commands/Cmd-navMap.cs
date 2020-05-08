//  Copyright © 2020 Randall Maas. All rights reserved.
using System.Collections.Generic;

namespace Anki.Vector.WebVizSDK
{

public partial class Session
{
    /// <summary>
    /// Request updated nave map
    /// </summary>
    /// <returns>false if there was an error, otherwise the item was submitted</returns>
    public bool NavMapUpdate()
    {
        var cmd = new Dictionary<string, object>();
        cmd["update"] = true;
        return Post("NavMap", cmd);
    }
}
}
