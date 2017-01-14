using System;
using System.Diagnostics;

namespace Common
{
    /// <summary>
    /// State machine event args are used to specify a state machine event.
    /// </summary>
    public class StateMachineEventArgs 
    {
        // Argument properties
        public String EventName { get; set; }
        public String EventInfo { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Source { get; set; }
        public string Target { get; set; }
        public StateMachineEventType EventType { get; set; }
        
        // Constructor
        public StateMachineEventArgs(string eventName, string eventInfo, StateMachineEventType eventType, string source, string target="All" )
        {
            EventName = eventName;
            EventInfo = eventInfo;
            EventType = eventType;
            Source = source;
            Target = target;
            TimeStamp = DateTime.Now; // Time stamp automatically added, when args are created. Does not need to be provided.
        }
    }

    public enum StateMachineEventType { System, Command, Notification, External}



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