using System;
using System.Collections.Generic;

namespace ActiveStateMachine
{
    /// <summary>
    /// Base class for a state
    /// implementations derive from it
    /// </summary>
    public class State
    {
        //Public members
        public String StateName { get; private set; }
        public Dictionary<String, Transition> StateTansitionList { get; private set; }
        public List<StateMachineAction> EntryActions { get; private set; }
        public List<StateMachineAction> ExitActions { get; private set; }
        public bool IsDefaultState { get; private set; }


        /// <summary>
        ///  Constructor for a state
        /// </summary>
        /// <param name="name"></param>
        /// <param name="transitionList"></param>
        /// <param name="defaultState"></param>
        public State(String name, Dictionary<String, Transition> transitionList, List<StateMachineAction> entryActions, List<StateMachineAction> exitActions, bool defaultState = false)
        {
            //Configure state properties
            StateName = name;
            StateTansitionList = transitionList;
            IsDefaultState = defaultState;
            EntryActions = entryActions;
            ExitActions = exitActions;
        }
    }
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