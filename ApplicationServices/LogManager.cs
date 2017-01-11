using System;
using System.Diagnostics;
using Common;

namespace ApplicationServices
{
    /// <summary>
    /// Simple logging class for state machine and service events
    /// </summary>
    public class LogManager
    {
        #region singleton implementation
            // Create a thread-safe singleton wit lazy initialization
            private static readonly Lazy<LogManager> _logger =new Lazy<LogManager>(() => new LogManager());

            public static LogManager Instance { get { return _logger.Value; } }

            private LogManager()
            {
            }
        #endregion

        /// <summary>
        /// Log infos to debug window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void LogEventHandler(object sender, StateMachineEventArgs args)
        {
            // Log system events
            if (args.EventType!=StateMachineEventType.Notification)
            {
                Debug.Print(args.TimeStamp + " SystemEvent: " + args.EventName + " - Info: " + args.EventInfo + " - StateMachineArgumentType: " + args.EventType + " - Source: " + args.Source + " - Target: " + args.Target);
            }
            // Log state machine notifications
            else
            {
                Debug.Print(args.TimeStamp + " Notification: " + args.EventName + " - Info: " + args.EventInfo + " - StateMachineArgumentType: " + args.EventType + " - Source: " + args.Source + " - Target: " + args.Target);
            }
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