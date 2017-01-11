using System;
using System.Collections.Generic;

namespace ActiveStateMachine
{
   /// <summary>
   /// State machine transition class
   /// </summary>
    public class Transition
    {
        //Public members
        public String Name { get; private set; }
        public String SourceStateName { get; private set; }
        public String TargetStateName { get; private set; }
        public List<StateMachineAction> GuardList { get; private set; }
        public List<StateMachineAction> TransitionActionList { get; private set; }
        public string Trigger { get; private set; }

        /// <summary>
        /// Constructor Transition
        /// </summary>
        /// <param name="name"></param>
        /// <param name="sourceState"></param>
        /// <param name="targetState"></param>
        /// <param name="guardList"></param>
        /// <param name="transitionActionList"></param>
        /// <param name="trigger"></param>
        public Transition(string name, string sourceState, string targetState, List<StateMachineAction> guardList, List<StateMachineAction> transitionActionList, string trigger  )
        {
            // Initialize transition properties
            Name = name;
            SourceStateName = sourceState;
            TargetStateName = targetState;
            GuardList = guardList;
            TransitionActionList=transitionActionList;
            Trigger = trigger;
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
