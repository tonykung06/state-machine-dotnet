using System;
using System.Collections.Generic;
using ActiveStateMachine;
using ApplicationServices;
using Common;

namespace TelephoneStateMachine
{
    /// <summary>
    /// Configuration class for telephone state machine sample
    /// The sample shows the inner workings of a state machine and therefore has educational character
    /// A lot of issues have been simplified, for example there are only a few action and no guard actions.
    /// </summary>
    public  class TelephoneStateMachineConfiguration {

        // Public members
        // List of valid states for this state machine
        public Dictionary<String, State> TelephoneStateMachineStateList { get; set; }
        // List of activities in the system
        public TelephoneActivities TelephoneActivities { get; set; }

        // Max number of entries in trigger queue
        public int MaxEntries { get; set; }
       // Event Manager
        public EventManager TelephoneEventManager;
        // View Manager
        public ViewManager TelephoneViewManager;
        // Device Manager
        public DeviceManager TelephoneDeviceManager;
        // Logger
        public LogManager TelephoneLogManager;


        /// <summary>
        /// Constructor
        /// </summary>
        public TelephoneStateMachineConfiguration()
        {
            BuildConfig();
        }


        /// <summary>
        /// Build telephone state configuration
        /// </summary>
        private void BuildConfig()
        {
            // Set the maximum queue capacity
            MaxEntries = 50;

            ////////////////////////////////
            // Transitions and actions
            ////////////////////////////////
            # region Preparation of transitions and actions
            // Create the object holding implementation of all system actions
            TelephoneActivities = new TelephoneActivities();


            // Create actions and map action methods into the corresponding action object
            // Device actions
            var actionBellRings = new StateMachineAction("ActionBellRings", TelephoneActivities.ActionBellRings);
            var actionBellSilent = new StateMachineAction("ActionBellSilent", TelephoneActivities.ActionBellSilent);
            var actionLineOff = new StateMachineAction("ActionLineOff", TelephoneActivities.ActionLineOff);
            var actionLineActive = new StateMachineAction("ActionLineActive", TelephoneActivities.ActionLineActive);
            // View actions
            var actionViewPhoneRings = new StateMachineAction("ActionViewPhoneRings", TelephoneActivities.ActionViewPhoneRings);
            var actionViewPhoneIdle = new StateMachineAction("ActionViewPhoneIdle", TelephoneActivities.ActionViewPhoneIdle);
            var actionViewTalking = new StateMachineAction("ActionViewTalking", TelephoneActivities.ActionViewTalking);
            // Error action
            var actionViewErrorPhoneRings = new StateMachineAction("ActionViewErrorPhoneRings", TelephoneActivities.ActionErrorPhoneRings);


            //  Create transitions and corresponding triggers, states need to be added. 
            var emptyList = new List<StateMachineAction>(); // To avoid null reference exceptions, use an empty list where no actions are used.
            // transition IncomingCall
            var ICActions = new List<StateMachineAction>();
            ICActions.Add(actionViewPhoneRings);
            var transIncomingCall = new Transition("TransitionIncomingCall", "StatePhoneIdle", "StatePhoneRings", emptyList, ICActions, "OnLineExternalActive");
            // transition ErrorPhoneRings - self-transition on PhoneRings state
            var EPRActions = new List<StateMachineAction>();
            EPRActions.Add(actionViewErrorPhoneRings);
            var transErrorPhoneRings = new Transition("TransitionErrorPhoneRings", "StatePhoneRings", "StatePhoneRings", emptyList, EPRActions, "OnBellBroken");
            // transition CallBlocked
            var CBActions = new List<StateMachineAction>();
            CBActions.Add(actionViewPhoneIdle); // Go back to vie Phone Idle
            var transcallBlocked = new Transition("TransitionCallBlocked", "StatePhoneRings", "StatePhoneIdle", emptyList, CBActions, "OnReceiverDown");
            // transition CallAccepted
            var CAActions = new List<StateMachineAction>();
            CAActions.Add(actionViewTalking);
            var transCallAccepted = new Transition("TransitionCallAccepted", "StatePhoneRings", "StateTalking", emptyList, CAActions, "OnReceiverUp");
            // transition CallEnded
            var CEActions = new List<StateMachineAction>();
            CEActions.Add(actionViewPhoneIdle);
            var transcallEnded = new Transition("TransitionCallEnded", "StateTalking", "StatePhoneIdle", emptyList, CEActions, "OnReceiverDown");
            #endregion


            ////////////////////////////////
            // States
            ////////////////////////////////
            #region Assemble all states

            // Create states

            // State: PhoneIdle
            var transitionsPhoneIdle = new Dictionary<String, Transition>();
            var entryActionsPhoneIdle = new List<StateMachineAction>();
            var exitActionsPhoneIdle = new List<StateMachineAction>();
            transitionsPhoneIdle.Add("TransitionIncomingCall", transIncomingCall);
            // Always specify all action lists, even empty ones, do not pass null into a state -> Lists are read via foreach, which will return an error, if they are null!
            var phoneIdle = new State("StatePhoneIdle", transitionsPhoneIdle, entryActionsPhoneIdle, exitActionsPhoneIdle, true);

            // State: PhoneRings
            var transitionsPhoneRings = new Dictionary<String, Transition>();
            var entryActionsPhoneRings = new List<StateMachineAction>();
            entryActionsPhoneRings.Add(actionBellRings);
            var exitActionsPhoneRings = new List<StateMachineAction>();
            exitActionsPhoneRings.Add(actionBellSilent);
            transitionsPhoneRings.Add("TransitionCallBlocked", transcallBlocked);
            transitionsPhoneRings.Add("TransitionCallAccepted", transCallAccepted);
            transitionsPhoneRings.Add("TransitionErrorPhoneRings", transErrorPhoneRings);
            var phoneRings = new State("StatePhoneRings", transitionsPhoneRings, entryActionsPhoneRings, exitActionsPhoneRings);

            // State: Talking
            var transitionsTalking = new Dictionary<String, Transition>();
            var entryActionsTalking = new List<StateMachineAction>();
            entryActionsTalking.Add(actionLineActive);
            var exitActionsTalking = new List<StateMachineAction>();
            exitActionsTalking.Add(actionLineOff);
            transitionsTalking.Add("TransitionCallEnded", transcallEnded);
            //exitActionsTalking.AddFirst(lineDown);
            var talking = new State("StateTalking", transitionsTalking, entryActionsTalking, exitActionsTalking);

            // Adding all - now completed - state configurations to the global state list
            TelephoneStateMachineStateList = new Dictionary<string, State>
            {
                {"StatePhoneIdle", phoneIdle},
                {"StatePhoneRings", phoneRings},
                {"StateTalking", talking}
            };

            #endregion

            ////////////////////////////////
            // Application services
            ////////////////////////////////
            #region Application services
            // Get application services
            TelephoneEventManager = EventManager.Instance;
            TelephoneViewManager = ViewManager.Instance;
            TelephoneLogManager = LogManager.Instance;
            TelephoneDeviceManager = DeviceManager.Instance;

            #endregion

        }


        ////////////////////////////////
        // Event configuration
        ////////////////////////////////
        #region Events are  published and connected to subscribers

        /// <summary>
        /// Register all events and add subscribers
        /// Needs to be called from state machine, because the state machine instance is required
        /// </summary>
        /// <param name="telephoneStateMachine"></param>
        /// <param name="telephoneActivities"></param>
        public void DoEventMappings(TelephoneStateMachine telephoneStateMachine, TelephoneActivities telephoneActivities)
        {
            
            //////////////////////////////////
            //Register all  events
            //////////////////////////////////
            #region Register events
            // Events implemented fro use case
            TelephoneEventManager.RegisterEvent("TelephoneUiEvent", telephoneActivities);
            TelephoneEventManager.RegisterEvent("TelephoneDeviceEvent", telephoneActivities);
            
            // Framework / infrastructure event
            TelephoneEventManager.RegisterEvent("StateMachineEvent", telephoneStateMachine);
            TelephoneEventManager.RegisterEvent("UINotification", TelephoneViewManager);
            TelephoneEventManager.RegisterEvent("DeviceManagerNotification", TelephoneDeviceManager);
            TelephoneEventManager.RegisterEvent("EventManagerEvent", TelephoneEventManager);
            TelephoneEventManager.RegisterEvent("ViewManagerEvent", TelephoneViewManager);
            TelephoneEventManager.RegisterEvent("DeviceManagerEvent", TelephoneDeviceManager);
            #endregion

            ////////////////////////////////////////////////////////////////////
            // Subscribe handlers to events registered with event manager
            ////////////////////////////////////////////////////////////////////
            #region event mappings
            // Logging
            TelephoneEventManager.SubscribeEvent("DeviceManagerNotification", "LogEventHandler", TelephoneLogManager);
            TelephoneEventManager.SubscribeEvent("StateMachineEvent", "LogEventHandler", TelephoneLogManager);
            TelephoneEventManager.SubscribeEvent("EventManagerEvent", "LogEventHandler", TelephoneLogManager);
            TelephoneEventManager.SubscribeEvent("ViewManagerEvent", "LogEventHandler", TelephoneLogManager);
            TelephoneEventManager.SubscribeEvent("DeviceManagerEvent", "LogEventHandler", TelephoneLogManager);

            // Notifications / Triggers;
            TelephoneEventManager.SubscribeEvent("DeviceManagerNotification", "InternalNotificationHandler", telephoneStateMachine);
            
            // System event listeners in managers
            TelephoneEventManager.SubscribeEvent("TelephoneUiEvent", "ViewCommandHandler", TelephoneViewManager);
            TelephoneEventManager.SubscribeEvent("TelephoneDeviceEvent", "DeviceCommandHandler", TelephoneDeviceManager);
            TelephoneEventManager.SubscribeEvent("StateMachineEvent", "SystemEventHandler", TelephoneViewManager);
            TelephoneEventManager.SubscribeEvent("StateMachineEvent", "SystemEventHandler", TelephoneDeviceManager);
            TelephoneEventManager.SubscribeEvent("ViewManagerEvent", "DeviceCommandHandler", TelephoneDeviceManager); // Sends UI button clicks to device manager 


            #endregion

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