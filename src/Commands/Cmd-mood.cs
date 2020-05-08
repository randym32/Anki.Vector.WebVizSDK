//  Copyright © 2020 Randall Maas. All rights reserved.
using System.Collections.Generic;

namespace Anki.Vector.WebVizSDK
{
public partial class Session
{
    /// <summary>
    /// Sets a dimension of Vector's mood
    /// </summary>
    /// <param name="mood">The name of the dimension of emotion.</param>
    /// <param name="level"></param>
    /// <returns>false if there was an error, otherwise the item was submitted</returns>
    public bool Mood(string dimension, float level)
    {
        var cmd = new Dictionary<string, object>();
        cmd[dimension] = level;
        return Post("Mood", cmd);
    }
}
}
