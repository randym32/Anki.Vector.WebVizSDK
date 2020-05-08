//  Copyright © 2020 Randall Maas. All rights reserved.
using System;

namespace Anki.Vector.WebVizSDK
{
/// <summary>
/// This event is with each event received from a Vetor module
/// </summary>
public class ModuleEventArgs : EventArgs
{
    /// <summary>
    /// This is the name of the Vector module that sent it
    /// </summary>
    public string moduleName;

    /// <summary>
    /// This is the received data. It can be a value, an array, or a dictionary.
    /// </summary>
    public object data;

    /// <summary>
    /// Creates the event
    /// </summary>
    /// <param name="name">The name of the Vector module that sent the data</param>
    /// <param name="data">The received data from the module</param>
    public ModuleEventArgs(string name, object data) : base()
    {
        this.moduleName = name;
        this.data       = data;
    }
}

/// <summary>
/// This is used to allow setting event handlers for each module
/// </summary>
public class ModuleEventElement
{
    public delegate void EventDelegate(object sender, ModuleEventArgs kEvent);

    /// <summary>
    /// The internal chain of event handlers
    /// </summary>
    internal event EventDelegate EventHandler;

    /// <summary>
    /// This checks to see if any handlers have been attached to the delegate
    /// </summary>
    internal bool HasHandlers
    {
        get { return null != EventHandler; }
    }

    /// <summary>
    /// This is used to invoke the event
    /// </summary>
    /// <param name="sender">The event sender</param>
    /// <param name="eventArgs">The event arguments</param>
    public void Invoke(object sender, ModuleEventArgs eventArgs)
    {
        if (null != EventHandler)
            EventHandler.Invoke(sender, eventArgs);
    }


    /// <summary>
    /// Adds the event handler to the delegate chain
    /// </summary>
    /// <param name="element">The internal element</param>
    /// <param name="handler">The event handler to add</param>
    /// <returns>The internal element</returns>
    public static ModuleEventElement operator +(ModuleEventElement element, EventDelegate handler)
    {
        element.EventHandler += handler;
        return element;
    }


    /// <summary>
    /// Adds the event handler to the delegate chain
    /// </summary>
    /// <param name="element">The internal element</param>
    /// <param name="handler">The event handler to add</param>
    /// <returns>The internal element</returns>
    public static ModuleEventElement operator -(ModuleEventElement element, EventDelegate handler)
    {
        element.EventHandler -= handler;
        return element;
    }
}
}