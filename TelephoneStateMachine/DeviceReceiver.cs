using System;
using ApplicationServices;

namespace TelephoneStateMachine
{
    public class DeviceReceiver : Device
    {
        // Public members
        public bool ReceiverLifted { get; set; }

        // Public methods
        // Constructor
        public DeviceReceiver(string deviceName, Action<string, string, string> eventCallback)
            : base(deviceName, eventCallback)
        {}

        // Active method not used by state machine -  could be e.g. called by a device driver
        public void OnReceiverUp()
        {
            ReceiverLifted = true;
            DoNotificationCallBack("OnReceiverUp", "Receiver lifted", "Receiver");
        }
        public void OnReceiverDown()
        {
            ReceiverLifted = false;
            DoNotificationCallBack("OnReceiverDown", "Receiver down", "Receiver");
        }

        // Initialization
        public override void OnInit()
        {
            ReceiverLifted = false;
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