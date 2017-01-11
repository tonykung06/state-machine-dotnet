using System;


namespace ActiveStateMachine
{
    /// <summary>
    /// Class for any actions in the state machine.
    /// It is used for guards, transition actions as well as entry / exit actions
    /// </summary>
    public class StateMachineAction
    {
        //Public members
        public String Name { get; private set; }
        
       
        // Private members
        // Delegate pointing to the implementation of method to be executed
        private System.Action _method;


        // Methods

        /// <summary>
        /// Constructor for state machine action
        /// </summary>
        /// <param name="name"></param>
        /// <param name="method"></param>
        public StateMachineAction(string name, System.Action method)
        {
            Name = name;
            _method = method;
        }

        /// <summary>
        /// Method running the action. 
        /// Will be called e.g. by state machine, when a transition is executed. 
        /// Could also be used in a guard, entry or exit action
        /// </summary>
        public void Execute()
        {
            
            // Invoke the state machine action method 
            _method.Invoke();

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