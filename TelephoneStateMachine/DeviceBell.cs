
using System;
using ApplicationServices;


namespace TelephoneStateMachine
{
    /// <summary>
    /// Class implementing bell device
    /// </summary>
 
    public class DeviceBell : Device
    {
 
         // public members
        public bool Ringing { get; set; }

        // Device Methods  
        public DeviceBell(string name, Action<string, string, string> eventCallBack): base(name, eventCallBack){}

        public void Rings()
        {
            // Sample error handling
            try
            {
                // Sample errors
                // throw (new SystemException("System device completely failed- Fatal hardware error!")); // Catastrophic error stopping the system 
                // throw (new SystemException("OnBellBroken")); // Minor HW error to be displayed in UI - System keeps running! Uses a self transition on PhoneRings state
                Ringing = true;
                System.Media.SystemSounds.Hand.Play();
            }
            catch (Exception ex)
            {
                if (ex.Message == "OnBellBroken")
                {
                    DoNotificationCallBack("OnBellBroken", ex.Message, "Bell");
                }
                else
                {
                    DoNotificationCallBack("CompleteFailure", ex.Message, "Bell");
                }
            }
         }
        public void Silent()
        {
            Ringing = false;
        }

        public override void OnInit()
        {
            Ringing = false;
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