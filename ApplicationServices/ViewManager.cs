using System;
using System.Linq;
using Common;

namespace ApplicationServices
{
    public class ViewManager
    {
        // Private members
        private string[] _viewStates;
        private string DefaultViewState;
        // UI - make this a Dictionary<string,IUserInterfcae>, if you have to handle more than one
        private IUserInterface _UI;

        // Public members
        public event EventHandler<StateMachineEventArgs> ViewManagerEvent;
        public string CurrentView { get; private set; }
        public IViewStateConfiguration ViewStateConfiguration { get; set; }

        //Methods 
        #region singleton implementation
            // Create a thread-safe singleton wit lazy initialization
            private static readonly Lazy<ViewManager> _viewManager =new Lazy<ViewManager>(() => new ViewManager());

            public static ViewManager Instance { get { return _viewManager.Value; } }

        private ViewManager()
            {
            }
        #endregion

        //Methods
        
        // Load view state configuration
        public void LoadViewStateConfiguration(IViewStateConfiguration viewStateConfiguration, IUserInterface userInterface)
        {
            ViewStateConfiguration = viewStateConfiguration;
            _viewStates = viewStateConfiguration.ViewStateList;
            _UI = userInterface;
            DefaultViewState = viewStateConfiguration.DefaultViewState;
        }

       
        /// <summary>
        /// Handler method for state machine commands
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void ViewCommandHandler(object sender, StateMachineEventArgs args)
        {
            // This approach assumes that there is a dedicated view state for every state machine UI command.
            try
            {
                if (_viewStates.Contains(args.EventName))
                {
                    // Convention: view command event names matches corresponding view state
                    _UI.LoadViewState(args.EventName);
                    CurrentView = args.EventName;
                    RaiseViewManagerEvent("View Manager Command", "Successfully loaded view state: " + args.EventName);
                }
                else
                {
                    RaiseViewManagerEvent("View Manager Command", "View state not found!");
                }
                

            }
            catch (Exception ex)
            {

                RaiseViewManagerEvent("View Manager Command - Error", ex.ToString());
            }
        }

        /// <summary>
        ///  Handler method for special system events, e.g. initialization
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void SystemEventHandler(object sender, StateMachineEventArgs args)
        {
            // Initialize
            if (args.EventName == "OnInit")
            {
                _UI.LoadViewState(DefaultViewState);
                CurrentView = DefaultViewState;
            }

            // Catastrophic Error handling
            if (args.EventName == "CompleteFailure")
                _UI.LoadViewState("CompleteFailure");

        }


        /// <summary>
        /// Method to raise a view manager event for logging, etc
        /// </summary>
        /// <param name="name"></param>
        /// <param name="info"></param>
        public void RaiseViewManagerEvent(string name, string info, StateMachineEventType eventType= StateMachineEventType.System)
        {
            var newVMargs = new StateMachineEventArgs(name, "View manager event: " + info,
                eventType, "View Manager");
            // Raise event only, if there are subscribers!
            if (ViewManagerEvent != null) ViewManagerEvent(this, newVMargs);
        }


        /// <summary>
        /// Sends a command to another service
        /// </summary>
        /// <param name="name"></param>
        /// <param name="info"></param>
        public void RaiseUICommand(string command, string info, string source, string target)
        {
            var newUIargs = new StateMachineEventArgs(command, info, StateMachineEventType.Command, source, target);
            if (ViewManagerEvent != null) ViewManagerEvent(this, newUIargs);
        }
    }
}
