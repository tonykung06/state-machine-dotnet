using System;
using System.Collections.Generic;
using System.Diagnostics;
using Common;

namespace ApplicationServices
{
    /// <summary>
    /// This class is responsible to route all events to appropriate subscribers
    /// </summary>
    public class EventManager
    {
        // Private members
        // Collection of registered events
        private Dictionary<string, object> EventList;

        // Public members
        // Event manger event is used for logging
        public event EventHandler<StateMachineEventArgs> EventManagerEvent;


        #region singleton implementation
        // Create a thread-safe singleton wit lazy initialization
        private static readonly Lazy<EventManager> _eventManager = new Lazy<EventManager>(() => new EventManager());

        public static EventManager Instance { get { return _eventManager.Value; } }

        private EventManager()
        {
            EventList = new Dictionary<string, object>();
        }
        #endregion

        //Methods

        /// <summary>
        /// Registration of an event used in the system
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="source"></param>
        public void RegisterEvent(string eventName, object source)
        {

            EventList.Add(eventName, source);
        }

        /// <summary>
        /// Subscription method maps handler method in a sink object to an event of the source object. 
        /// Of course, method signatures between delegate and handler need to match!
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="handlerMethodName"></param>
        /// <param name="sink"></param>
        public bool SubscribeEvent(string eventName, string handlerMethodName, object sink)
        {
            try
            {
                // Get event from list
                var evt = EventList[eventName];
                // Determine meta data from event and handler
                var eventInfo = evt.GetType().GetEvent(eventName);
                var methodInfo = sink.GetType().GetMethod(handlerMethodName);
                // Create new delegate mapping event to handler
                Delegate handler = Delegate.CreateDelegate(eventInfo.EventHandlerType, sink, methodInfo);
                eventInfo.AddEventHandler(evt, handler);
                return true;
            }
            catch (Exception ex)
            {
                // Log failure!
                var message = "Exception while subscribing to handler. Event:" + eventName + " - Handler: " + handlerMethodName + "- Exception: " + ex;
                Debug.Print(message);
                RaiseEventManagerEvent("EventManagerSystemEvent", message, StateMachineEventType.System);
                return false;
            }
        }

        private void RaiseEventManagerEvent(string eventName, string eventInfo, StateMachineEventType eventType)
        {
            var newArgs = new StateMachineEventArgs(eventName ,eventInfo, eventType, "Event Manager");
            if (EventManagerEvent != null) EventManagerEvent(this, newArgs);
        }
    }
}

// =======================================================================
// Disclaimer - Building State Machines in .NET
// =======================================================================
// 
//   THIS CODE IS EDUCATIONAL AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//   ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//   THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//   PARTICULAR PURPOSE.
// 
//    Copyright (C) 2014 Wechsler Consulting GmbH & Co. KG
// 
//    Alexander Wechsler, Enterprise Architect
//    Microsoft Regional Director Germany | eMVP
//    Wechsler Consulting GmbH & Co. KG
// 
// =======================================================================
