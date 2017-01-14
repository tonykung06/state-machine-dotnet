using System;
using ApplicationServices;

namespace TelephoneStateMachine 
{
    /// <summary>
    /// This class is not needed in the incoming call use case
    /// </summary>
    public class DeviceDial : Device
    {
        public DeviceDial(string name, Action<string, string, string> eventCallBack) : base(name, eventCallBack)
        {
        }
        public override void OnInit()
        {
        }

        // No functionality to implement here!- Yet.
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