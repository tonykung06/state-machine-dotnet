using System;

namespace ApplicationServices
{
    /// <summary>
    /// Base class for all devices managed by device manager
    /// </summary>
    public abstract class Device
    {
        Action<string, string, string> _devEvMethod;
        public string DevName { get; private set; }


        /// <summary>
        /// Constructor taking device name and callback method of device manager
        /// </summary>
        /// <param name="deviceName"></param>
        /// <param name="eventCallBack"></param>
        public Device(string deviceName, Action<string, string, string> eventCallBack)
        {
            DevName = deviceName;
            _devEvMethod = eventCallBack;
        }

        // Device initialization method - needs to be implemented in derived classes
        public abstract void OnInit();
    

      // Event infrastructure
        public void RegisterEventCallback(Action<string, string, string > method)
        {
            _devEvMethod = method;

        }

        public void DoNotificationCallBack(string Name, string EventInfo, string source)
        {
            _devEvMethod.Invoke(Name, EventInfo, source);
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
