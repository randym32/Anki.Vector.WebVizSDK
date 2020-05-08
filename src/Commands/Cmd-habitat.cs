//  Copyright © 2020 Randall Maas. All rights reserved.
using System.Collections.Generic;
namespace Anki.Vector.WebVizSDK
{
/// <summary>
/// This is the state of the habitat
/// </summary>
public enum HabitatState
{
    /// <summary>
    /// It is not knownwhether in the habitat
    /// </summary>
    unknown,

    /// <summary>
    /// Vector is in the habitat
    /// </summary>
    inHabitat,

    /// <summary>
    /// Vector is not in the habitat
    /// </summary>
    notInHabitat
}

public partial class Session
{
    /// <summary>
    /// Set the habitat state
    /// </summary>
    /// <param name="state">Whether or not Vector is in the habitat</param>
    /// <returns>false if there was an error, otherwise the item was submitted</returns>
    bool habitat(HabitatState state)
    {
        var cmd = new Dictionary<string, object>();
        cmd["forceHabitatState"] = HabitatState.inHabitat == state?"InHabitat":
            HabitatState.notInHabitat==state?"NotInHabitat":"Unknown";
        return Post("Habitat", cmd);
    }
}
}