using System;
using Caliburn.Micro;

namespace F1TelemetryUi.Referencing
{
    public class ReferencingStateMachine : PropertyChangedBase
    {
        private ReferencingState _currentState;

        public event EventHandler<ReferencingStateChangedEventArgs> StateChanged;

        public ReferencingState CurrentState
        {
            get
            {
                return _currentState;
            }

            private set
            {
                ReferencingState oldState = _currentState;
                ReferencingState newState = value;
                _currentState = value;
                NotifyOfPropertyChange();
                OnStateChanged(oldState, newState);
            }
        }

        public void Disable()
        {
            SetState(ReferencingState.Disabled);
        }

        public void Enable()
        {
            SetState(ReferencingState.TakingFirstPoint);
        }

        public void Next()
        {
            CurrentState++;
        }

        public void Previous()
        {
            CurrentState--;
        }

        public void Toggle()
        {
            if (CurrentState == ReferencingState.Disabled)
            {
                Enable();
            }
            else
            {
                Disable();
            }
        }

        private void OnStateChanged(ReferencingState oldState, ReferencingState newState)
        {
            StateChanged?.Invoke(this, new ReferencingStateChangedEventArgs(oldState, newState));
        }

        private void SetState(ReferencingState newState)
        {
            CurrentState = newState;
        }
    }
}
