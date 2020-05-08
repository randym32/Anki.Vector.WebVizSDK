//  Copyright © 2020 Randall Maas. All rights reserved.
using System.Collections.Generic;

namespace Anki.Vector.WebVizSDK
{
public partial class Session
{
    /// <summary>
    /// A behavior can be TBD?
    /// </summary>
    /// <param name="behaviorName">The name of a behavior.  TBD: is the identifier?</param>
    /// <param name="presetConditions">Force the behavior conditions to evaluate
    /// to this; if true, the behavior has "met" its conditions</param>
    /// <returns>false if there was an error, otherwise the item was submitted</returns>
    public bool behavior(string behaviorName, bool presetConditions)
    {
        var cmd = new Dictionary<string, object>();
        cmd["behaviorName"] = behaviorName;
        cmd["presetConditions"] = presetConditions;
        return Post("Behaviors", cmd);
    }

}
}