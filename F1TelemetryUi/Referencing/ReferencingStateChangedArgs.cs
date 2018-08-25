using System;

namespace F1TelemetryUi.Referencing
{
    public class ReferencingStateChangedEventArgs : EventArgs
    {
        public ReferencingStateChangedEventArgs(ReferencingState oldState, ReferencingState newState)
        {
            OldState = oldState;
            NewState = newState;
        }

        public ReferencingState NewState { get; }
        public ReferencingState OldState { get; }
    }
}
