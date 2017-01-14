using System;
using Common;

namespace TelephoneStateMachine
{
    /// <summary>
    /// This class contains all action implementations for the telephone incoming call use case sample
    /// </summary>
    public class TelephoneActivities
    {
        // Public members
        // Events to communicate from state machine to managers - wiring will be done via via event manager
        public event EventHandler<StateMachineEventArgs> TelephoneUiEvent;
        public event EventHandler<StateMachineEventArgs> TelephoneDeviceEvent;

        // Methods
        #region Telephone Action methods
            // Create actions used in transitions or states (entry/exit)

            //////////////////
            // Device actions
            //////////////////
            public void ActionBellRings()
            {
                // Error handling - do not do it  here, instead in device or UI!
                // Raising an event normally does not fail!
                RaiseDeviceEvent("Bell","Rings");
            }

            public void ActionBellSilent()
            {
              RaiseDeviceEvent("Bell","Silent");   
            }
            public void ActionLineOff()
            {
                RaiseDeviceEvent("PhoneLine","OffInternal");
            }
            public void ActionLineActive()
            {
                RaiseDeviceEvent("PhoneLine","ActiveInternal");
            }

            //////////////////    
            // View actions
            //////////////////
            public void ActionViewPhoneRings()
            {
                 
                RaiseTelephoneUiEvent("ViewPhoneRings");
            }

            public void ActionViewPhoneIdle()
            {
                RaiseTelephoneUiEvent("ViewPhoneIdle");
                System.Media.SystemSounds.Beep.Play();
            }
            public void ActionViewTalking()
            {
                RaiseTelephoneUiEvent("ViewTalking");
            }

            //////////////////    
            // Error actions
            //////////////////
            public void ActionErrorPhoneRings()
            {
                RaiseTelephoneUiEvent("ViewErrorPhoneRings");
            }


        #endregion 
        
        
        #region Event methods
            /// <summary>
            /// Helper to raise the telephone UI event
            /// </summary>
            /// <param name="command"></param>
            public void RaiseTelephoneUiEvent(string command)
            {
                var telArgs = new StateMachineEventArgs(command, "UI command", StateMachineEventType.Command, "State machine action", "View Manager");
                TelephoneUiEvent(this, telArgs);
            }

             public void RaiseDeviceEvent(string target, string command)
            {
                var telArgs = new StateMachineEventArgs(command, "Device command", StateMachineEventType.Command, "State machine action", target);
                TelephoneDeviceEvent(this, telArgs);
            }
        #endregion
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