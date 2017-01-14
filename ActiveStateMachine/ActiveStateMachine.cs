using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common;

namespace ActiveStateMachine
{
    
    /// <summary>
    /// Base class for active state machines
    /// </summary>
    public class ActiveStateMachine
    {
        // Public members
        public Dictionary<String, State> StateList { get; private set; }
        public BlockingCollection<string> TriggerQueue { get; private set; }
        public State CurrentState { get; private set; }
        public State PreviousState { get; private set; }
        public EngineState StateMachineEngine { get; private set; }
        public event EventHandler<StateMachineEventArgs> StateMachineEvent;

        // Private members
        private Task _queueWorkerTask;
        private readonly State _initialState;
        private ManualResetEvent _resumer;
        private CancellationTokenSource _tokenSource;
        
        //Methods

        /// <summary>
        /// Constructor Active State Machine
        /// </summary>
        /// <param name="stateList"></param>
        /// <param name="queueCapacity"></param>
        public ActiveStateMachine(Dictionary<String, State> stateList, int queueCapacity)
        {
            // Configure state machine
            StateList = stateList;
            // Anything needs to start somewhere - the initial state
            _initialState = new State("InitialState", null,null,null);
            // Collection taking all triggers. It is thread-safe and blocking as well as FIFO!
            // Limiting its capacity protects against DOS like errors or attacks
            TriggerQueue = new BlockingCollection<string>(queueCapacity);

            // Initialize
            InitStateMachine();
            // Raise an event
            RaiseStateMachineSystemEvent("StateMachine: Initialized", "System ready to start");
            StateMachineEngine = EngineState.Initialized;
        }

        #region state machine engine

        /// <summary>
        /// Start the state machine
        /// </summary>
        public void Start()
        {
            // Create cancellation token for QueueWorker method
            _tokenSource = new CancellationTokenSource();

            // Create a new worker thread, if it does not exist
            _queueWorkerTask = Task.Factory.StartNew(QueueWorkerMethod,_tokenSource, TaskCreationOptions.LongRunning);

            // Set engine state
            StateMachineEngine = EngineState.Running;
            RaiseStateMachineSystemEvent("StateMachine: Started", "System running.");
        }

        /// <summary>
        /// Pauses the state machine worker thread.
        /// </summary>
        public void Pause()
        {
            // Set engine state
            StateMachineEngine = EngineState.Paused;
            _resumer.Reset();
            RaiseStateMachineSystemEvent("StateMachine: Paused", "System  waiting.");

        }

        public void Resume()
        {
            // Worker thread exists, just resume where it was paused.
            _resumer.Set();
            // Set engine state
            StateMachineEngine = EngineState.Running;
            RaiseStateMachineSystemEvent("StateMachine: Resumed", "System running.");
        }

        /// <summary>
        ///  Ends queue processing
        /// </summary>
        public void Stop()
        {
            // Cancel processing
            _tokenSource.Cancel();
            // Wait for thread to return
            _queueWorkerTask.Wait();
            // Free resources
            _queueWorkerTask.Dispose();
            // Set engine state
            StateMachineEngine = EngineState.Stopped;
            RaiseStateMachineSystemEvent("StateMachine: Stopped", "System execution stopped.");
        }




        /// <summary>
        /// Initializes state machine, but does not start it -> dedicated start method
        /// </summary>
        public void InitStateMachine()
        {
            // Set previous state to an unspecific initial state. THe initial state never will be used during normal operation
            PreviousState = _initialState;

            // Look for the default state, which is the state to begin with in StateList.
            foreach (var state in StateList)
            {
                if (state.Value.IsDefaultState)
                {
                    CurrentState = state.Value;
                    RaiseStateMachineSystemCommand("OnInit", "StateMachineInitialized");
                }
            }

            // This is the synchronization object for resuming - passing true means non-blocking (signaled), which is the normal operation mode.
            _resumer= new ManualResetEvent(true);
        }


        /// <summary>
        /// Enter a trigger to the queue
        /// </summary>
        /// <param name="newTrigger"></param>
        private void EnterTrigger(string newTrigger)
        {
            // Put trigger in queue    
            try
            {
                TriggerQueue.Add(newTrigger);
            }
            catch (Exception e)
            {
                RaiseStateMachineSystemEvent("ActiveStateMachine - Error entering trigger", newTrigger + " - " + e.ToString());
            }
            // Raise an event
            RaiseStateMachineSystemEvent("ActiveStateMachine - Trigger entered", newTrigger);
        }

        
        /// <summary>
        /// Worker method for trigger queue
        /// </summary>
        private void QueueWorkerMethod(object dummy)
        {
                // Blocks execution until it is reset. Used to pause the state machine.
                _resumer.WaitOne();

                // Block the queue and loop through all triggers available. Blocking queue guarantees FIFO and the GetConsumingEnumerable method
                // automatically removes triggers from queue!
            try
            {
                foreach (var trigger in TriggerQueue.GetConsumingEnumerable())
                {
                    if (_tokenSource.IsCancellationRequested)
                    {
                        RaiseStateMachineSystemEvent("State machine: QueueWorker", "Processing canceled!");
                        return;
                    }
                    // Compare trigger
                    foreach (
                        var transition in
                            CurrentState.StateTansitionList.Where(
                                transition => trigger == transition.Value.Trigger))
                    {
                        ExecuteTransition(transition.Value);
                    }
                    
                }
                // Do not place any code here, because it will not be executed!
                // The foreach loop keeps spinning on the queue until thread is canceled.
            }
            catch (Exception ex)
            {
                RaiseStateMachineSystemEvent("State machine: QueueWorker", "Processing canceled! Exception: " + ex.ToString());
                // Create a new queue worker task. THe previous one is completing right now.
                Start();
            }
        }

        /// <summary>
        /// Transition to a new state
        /// </summary>
        /// <param name="transition"></param>
        protected virtual void ExecuteTransition(Transition transition)
        {
            // Default checking, if this is a valid transaction.
            if (CurrentState.StateName != transition.SourceStateName)
            {
                String message =
                    String.Format("Transition has wrong source state {0}, when system is in {1}",
                        transition.SourceStateName, CurrentState.StateName);
                RaiseStateMachineSystemEvent("State machine: Default guard execute transition.", message);
                return;
            }
            if (!StateList.ContainsKey(transition.TargetStateName))
            {
                String message =
                        String.Format("Transition has wrong target state {0}, when system is in {1}. State not in global state list",
                            transition.SourceStateName, CurrentState.StateName);
                RaiseStateMachineSystemEvent("State machine: Default guard execute transition.", message);
                return;
            }


            //Self transition - Just do the transition without executing exit, entry actions or guards
            if (transition.SourceStateName == transition.TargetStateName)
            {
                transition.TransitionActionList.ForEach(t => t.Execute());
                return; //Important: Return directly from self-transition
            }

            // Run all exit actions of the old state
            CurrentState.ExitActions.ForEach( a => a.Execute());

            // Run all guards of the transition
            transition.GuardList.ForEach(g => g.Execute());
            string info = transition.GuardList.Count + " guard actions executed!";
            RaiseStateMachineSystemEvent("State machine: ExecuteTransition", info); 

            // Run all actions of the transition
            transition.TransitionActionList.ForEach(t => t.Execute());


            //////////////////
            // State change
            //////////////////
            info = transition.TransitionActionList.Count + " transition actions executed!";
            RaiseStateMachineSystemEvent("State machine: Begin state change!", info); 

                
            // First resolve the target state with the help of its name
            var targetState = GetStatefromStateList(transition.TargetStateName);
               
            // Transition successful - Change state
            PreviousState = CurrentState;
            CurrentState = targetState;
                
            // Run all entry actions of new state
            foreach (var entryAction in CurrentState.EntryActions)
            {
                entryAction.Execute();
            }

            RaiseStateMachineSystemEvent("State machine: State change completed successfully!", "Previous state: " 
                + PreviousState.StateName + " - New state = " + CurrentState.StateName);
        }

        /// <summary>
        /// Helper to load state from state list
        /// </summary>
        /// <param name="targetStateName"></param>
        /// <returns></returns>
        private State GetStatefromStateList(string targetStateName)
        {
            return StateList[targetStateName];
        }

        #endregion

        #region event infrastructure
        /// <summary>
        /// Helper method to raise state machine system events
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="eventInfo"></param>
        private void RaiseStateMachineSystemEvent(String eventName, String eventInfo)
        {
            // Raise event only, if subscribers exist. Otherwise an exception occurs
            if (StateMachineEvent != null) StateMachineEvent(this,new StateMachineEventArgs(eventName,eventInfo, StateMachineEventType.System, "State machine"));
        }
        
        /// <summary>
        /// Raises an event of type command
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="eventInfo"></param>
        private void RaiseStateMachineSystemCommand(String eventName, String eventInfo)
        {
            // Raise event only, if subscribers exist. Otherwise an exception occurs
            if (StateMachineEvent != null) StateMachineEvent(this, new StateMachineEventArgs(eventName, eventInfo, StateMachineEventType.Command, "State machine"));
        }


        /// <summary>
        /// Event Handler for internal events triggering the state machine
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="intArgs"></param>
        public void InternalNotificationHandler(object sender, StateMachineEventArgs intArgs)
        {
            // Catastrophic error
            if (intArgs.EventName == "CompleteFailure")
            {
                RaiseStateMachineSystemCommand("CompleteFailure", intArgs.EventInfo + " Device : " + intArgs.Source);
                // Stop state machine to avoid any damage
                Stop();
            }
                // Normal operation
            else
            {
                EnterTrigger(intArgs.EventName);
            }
        }
        #endregion
    }

// Engines states for state machine   
public enum EngineState
    {
        Running,
        Stopped,
        Paused,
        Initialized
    };

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