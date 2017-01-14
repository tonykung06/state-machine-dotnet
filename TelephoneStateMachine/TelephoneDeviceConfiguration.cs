using System.Collections.Generic;
using ApplicationServices;
using Common;

namespace TelephoneStateMachine
{
    /// <summary>
    /// Class holding device configuration
    /// </summary>
    public class TelephoneDeviceConfiguration : IDeviceConfiguration
    {
        // Private members
        private  DeviceManager _devMan;
        // Public members

        // List of available devices
        public Dictionary<string, object> Devices { get; set; }
        
        // Methods

        // Constructor
        public TelephoneDeviceConfiguration()
        {
            _devMan= DeviceManager.Instance;
            InitDevices();
        }

        /// <summary>
        /// Configure all telephone devices used by state machine.
        /// </summary>
        private void InitDevices()
        {
            var bell = new DeviceBell("Bell", _devMan.RaiseDeviceManagerNotification);
            var phoneLine = new DevicePhoneLine("PhoneLine", _devMan.RaiseDeviceManagerNotification);
            var receiver = new DeviceReceiver("Receiver", _devMan.RaiseDeviceManagerNotification);

            Devices = new Dictionary<string, object> { { "Bell", bell }, { "PhoneLine", phoneLine }, { "Receiver", receiver } };
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
//    Copyright (C) 2013 Wechsler Consulting GmbH & Co. KG
// 
//    Alexander Wechsler, Enterprise Architect
//    Microsoft Regional Director Germany | eMVP
//    Wechsler Consulting GmbH & Co. KG
// 
// =======================================================================