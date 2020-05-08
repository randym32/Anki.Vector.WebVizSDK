//  Copyright © 2020 Randall Maas. All rights reserved.
using System.Collections.Generic;
namespace Anki.Vector.WebVizSDK
{
public partial class Session
{
    /// <summary>
    /// Connect to the cube
    /// </summary>
    /// <returns>false if there was an error, otherwise the item was submitted</returns>
    public bool cubeConnect()
    {
        var cmd = new Dictionary<string, object>();
        cmd["connectCube"] = true;
        return Post("Cubes", cmd);
    }

    /// <summary>
    /// Disconnect from the cube
    /// </summary>
    /// <returns>false if there was an error, otherwise the item was submitted</returns>
    public bool cubeDisconnect()
    {
        var cmd = new Dictionary<string, object>();
        cmd["disconnectCube"] = true;
        return Post("Cubes", cmd);
    }

    /// <summary>
    /// Flash the cube's lights
    /// </summary>
    /// <returns>false if there was an error, otherwise the item was submitted</returns>
    public bool cubeFlashLights()
    {
        var cmd = new Dictionary<string, object>();
        cmd["flashCubeLights"] = true;
        return Post("Cubes", cmd);
    }

    /// <summary>
    /// Unpair with Vector's preferred cube
    /// </summary>
    /// <returns>false if there was an error, otherwise the item was submitted</returns>
    public bool cubeForget()
    {
        var cmd = new Dictionary<string, object>();
        cmd["forgetPreferredCube"] = true;
        return Post("Cubes", cmd);
    }

    /// <summary>
    /// </summary>
    /// <returns>false if there was an error, otherwise the item was submitted</returns>
    public bool cubeInteractableSubscribe()
    {
        var cmd = new Dictionary<string, object>();
        cmd["subscribeInteractable"] = true;
        return Post("Cubes", cmd);
    }

    /// <summary>
    /// </summary>
    /// <returns>false if there was an error, otherwise the item was submitted</returns>
    public bool cubeInteractableUnsubscribe()
    {
        var cmd = new Dictionary<string, object>();
        cmd["subscribeInteractable"] = false;
        return Post("Cubes", cmd);
    }


    /// <summary>
    /// </summary>
    /// <returns>false if there was an error, otherwise the item was submitted</returns>
    public bool cubeTempInteractableSubscribe()
    {
        var cmd = new Dictionary<string, object>();
        cmd["subscribeTempInteractable"] = true;
        return Post("Cubes", cmd);
    }


    /// <summary>
    /// </summary>
    /// <returns>false if there was an error, otherwise the item was submitted</returns>
    public bool cubeTempInteractableUnsubscribe()
    {
        var cmd = new Dictionary<string, object>();
        cmd["subscribeTempInteractable"] = false;
        return Post("Cubes", cmd);
    }

    /// <summary>
    /// </summary>
    /// <returns>false if there was an error, otherwise the item was submitted</returns>
    public bool cubeBackgroundSubscribe()
    {
        var cmd = new Dictionary<string, object>();
        cmd["subscribeBackground"] = true;
        return Post("Cubes", cmd);
    }


    /// <summary>
    /// </summary>
    /// <returns>false if there was an error, otherwise the item was submitted</returns>
    public bool cubeBackgroundUnsubscribe()
    {
        var cmd = new Dictionary<string, object>();
        cmd["subscribeBackground"] = false;
        return Post("Cubes", cmd);
    }


    /// <summary>
    /// </summary>
    /// <returns>false if there was an error, otherwise the item was submitted</returns>
    public bool cubeTempBackgroundSubscribe()
    {
        var cmd = new Dictionary<string, object>();
        cmd["subscribeTempBackground"] = true;
        return Post("Cubes", cmd);
    }


    /// <summary>
    /// </summary>
    /// <returns>false if there was an error, otherwise the item was submitted</returns>
    public bool cubeTempBackgroundUnsubscribe()
    {
        var cmd = new Dictionary<string, object>();
        cmd["subscribeTempBackground"] = false;
        return Post("Cubes", cmd);
    }
}
}
