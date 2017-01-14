namespace TelephoneStateMachine
{
    public class TelephoneStateMachine : ActiveStateMachine.ActiveStateMachine
    {
        // Private members
        private TelephoneStateMachineConfiguration _config;

        // Methods

        /// <summary>
        ///     Constructor
        ///     Used to load the state configuration and to initialize the event manager
        /// </summary>
        /// <param name="telConfig"></param>
        public TelephoneStateMachine(TelephoneStateMachineConfiguration telConfig)
            : base(telConfig.TelephoneStateMachineStateList, telConfig.MaxEntries)
        {
            // Get configuration for the telephone
            _config = telConfig;

            // Configure event manager routing information. Events are registered and mapped to handlers
            telConfig.DoEventMappings(this, _config.TelephoneActivities);
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