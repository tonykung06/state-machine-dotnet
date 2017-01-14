using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms.VisualStyles;
using Common;

namespace Telephone_UI
{
    /// <summary>
    /// Class holding view states
    /// </summary>
    class TelephoneViewStateConfiguration : IViewStateConfiguration
    {
        // Public members

        // List of available view states
        public Dictionary<string, object> ViewStates { get; private set; }
        // Plain array holding just the names of the view states, to be used in view manager
        public string[] ViewStateList { get; private set; }
        public string DefaultViewState
        {
            get
             {
                 foreach (var item in ViewStates.Values.Cast<TelephoneViewState>().Where(item => item.IsDefaultViewState))
                    {
                        return item.Name;
                    }
                 throw new Exception("Missing default view state");
             }
        }

        // Methods

        /// <summary>
        /// Constructor
        /// </summary>
        public TelephoneViewStateConfiguration()
        {
            InitViewStates();
        }

        /// <summary>
        /// Configuration:
        /// Create all telephone view state configurations and define a default view state
        /// </summary>
        private void InitViewStates()
        {
            // Create new view states and add them to the dictionary
            ViewStates =new Dictionary<string, object>
            {
                {"ViewPhoneIdle", new TelephoneViewState("ViewPhoneIdle", false, false, true, true)}, // Default view state
                {"ViewPhoneRings", new TelephoneViewState("ViewPhoneRings", true, false, true)},
                {"ViewErrorPhoneRings", new TelephoneViewState("ViewErrorPhoneRings", true, false, true)},
                {"ViewTalking", new TelephoneViewState("ViewTalking", false, true, false)}
            };

            ViewStateList = new[] { "ViewPhoneIdle", "ViewErrorPhoneRings", "ViewPhoneRings", "ViewTalking" };
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
