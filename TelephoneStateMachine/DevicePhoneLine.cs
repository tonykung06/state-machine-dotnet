using System;
using ApplicationServices;

namespace TelephoneStateMachine
{
    public class DevicePhoneLine : Device
    {
        // private members

        // Public members
        public bool LineActiveExternal { get; set; }
        public bool LineActiveInternal { get; set; }

        // Methods
        // Constructor

        public DevicePhoneLine(string deviceName, Action<string, string, string> eventCallBack)
            : base(deviceName, eventCallBack)
        {
        }

        /// <summary>
        /// Incoming part of line  - active cannot be controlled by device
        /// </summary>
        public void ActiveExternal()
        {
            LineActiveExternal = true;
            DoNotificationCallBack("OnLineExternalActive","Phone line set to active",DevName);
        }

        /// <summary>
        /// Internal line - controlled by device
        /// </summary>
        public void ActiveInternal()
        {
            LineActiveInternal = true;
        }

        public void OffInternal()
        {
            LineActiveInternal = false;
            System.Media.SystemSounds.Hand.Play();
        }

        // Initialization
        public override void OnInit()
        {
            LineActiveInternal = false;
            LineActiveExternal = false;
        }
    }
}

// =======================================================================
// Disclaimer - Building State Machines in .NET
// =======================================================================
// 
//   THIS CODE IS EDUCATIONAL AND INFORMATION IS PROVIDED "AS IS" WITHOUT 
//   WARRANTY OF
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