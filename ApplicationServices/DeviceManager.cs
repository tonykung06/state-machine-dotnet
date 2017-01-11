using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Common;

namespace ApplicationServices
{
    /// <summary>
    /// Manages all devices
    /// </summary>
    public class DeviceManager
    {
        
        #region singleton implementation
            // Create a thread-safe singleton with lazy initialization
            private static readonly Lazy<DeviceManager> _deviceManager =new Lazy<DeviceManager>(() => new DeviceManager());

            public static DeviceManager Instance { get { return _deviceManager.Value; } }

            private DeviceManager()
            {
                DeviceList = new Dictionary<string, object>();
            }
        #endregion

        // Public members
        
        /// <summary>
        /// List of system devices
        /// </summary>
        public Dictionary<string, object> DeviceList { get; set; }

        // Public members
        // Device manager event is used for logging
        public event EventHandler<StateMachineEventArgs> DeviceManagerEvent;
        public event EventHandler<StateMachineEventArgs> DeviceManagerNotification;

        // Methods
        
        /// <summary>
        /// Add a system device
        /// </summary>
        /// <param name="name"></param>
        /// <param name="device"></param>
        public void AddDevice(string name, object device)
        {
            DeviceList.Add(name, device);
            RaiseDeviceManagerEvent("Added device", name);
        }

        /// <summary>
        /// Removes a system device
        /// </summary>
        /// <param name="name"></param>
        public void RemoveDevice(string name)
        {
            DeviceList.Remove(name);
            RaiseDeviceManagerEvent("Removed device", name);
        }

        /// <summary>
        /// Handler method for state machine commands
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void DeviceCommandHandler(object sender, StateMachineEventArgs args)
        {

            // Listen to command events only
            if (args.EventType != StateMachineEventType.Command) return;

            // Get device and execute command action method
            try
            {
                if (!DeviceList.Keys.Contains(args.Target)) return;
                // Convention device commands and method names must mach!
                var device = DeviceList[args.Target];
                MethodInfo deviceMethod = device.GetType().GetMethod(args.EventName);
                deviceMethod.Invoke(device, new object[] { });
                RaiseDeviceManagerEvent("DeviceCommand", "Successful device command: " + args.Target + " - " + args.EventName);
            }
            catch (Exception ex)
            {
                RaiseDeviceManagerEvent("DeviceCommand - Error", ex.ToString());
            }
        }

        /// <summary>
        ///  Handler method for special system events, e.g. initialization
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void SystemEventHandler(object sender, StateMachineEventArgs args)
        {
            // Initialize
            if (args.EventName == "OnInit" && args.EventType==StateMachineEventType.Command)
            {
                foreach ( var dev in DeviceList)
                {
                    try
                    {
                        MethodInfo initMethod = dev.Value.GetType().GetMethod("OnInit");
                        initMethod.Invoke(dev.Value, new object[]{});
                        RaiseDeviceManagerEvent("DeviceCommand - Initialization device",  dev.Key);
                    }
                    catch (Exception ex)
                    {
                        RaiseDeviceManagerEvent("DeviceCommand - Initialization error device" + dev.Key, ex.ToString());
                    }
                }
            }

            // Notification handling
            // because we use UI to trigger transitions devices would trigger normally themselves.
            // Nevertheless, this is common, if SW user interfaces control devices
            // View and device managers communicate on system event bus and use notifications to trigger state machine as needed!
            if (args.EventType == StateMachineEventType.Command)
            {
                // Check for right condition 
                if (args.EventName == "OnInit") return;
                if (!DeviceList.ContainsKey(args.Target)) return;
                
                // Dispatch command to device
                DeviceCommandHandler(this, args);
                //RaiseDeviceManagerNotification(args.EventName, "Routed through device manager: " + args.EventInfo, args.Source);
            }


        }

        /// <summary>
        /// Method to raise a device manager event for logging, etc.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="info"></param>
        private void RaiseDeviceManagerEvent(string name, string info)
        {
            var newDMArgs = new StateMachineEventArgs(name, "Device manager event: " + info,
                StateMachineEventType.System, "Device Manager");
            // Raise only, if subscribed
            if (DeviceManagerEvent != null)
            {
                DeviceManagerEvent(this, newDMArgs);
            }
        }


        /// <summary>
        /// Sends a command from device manager to state machine
        /// </summary>
        /// <param name="command"></param>
        /// <param name="info"></param>
        /// <param name="source"></param>
        public void RaiseDeviceManagerNotification(string command, string info, string source)
        {
            var newDMArgs = new StateMachineEventArgs(command, info, StateMachineEventType.Notification, source, "State Machine");
            // Raise only, if subscribed
            if (DeviceManagerNotification != null)
            {
                DeviceManagerNotification(this, newDMArgs);
            }
            
        }

        /// <summary>
        /// Loads device configuration
        /// </summary>
        /// <param name="devManConfiguration"></param>
        public void LoadDeviceConfiguration(IDeviceConfiguration devManConfiguration)
        {
            DeviceList = devManConfiguration.Devices;
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
