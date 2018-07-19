﻿using UnityEngine;
using UnityEngine.Events;

namespace MicroState
{
	public class StateBehaviour<StateType> : MonoBehaviour where StateType : MicroState.State, new()
	{
		#region Private attributes      
		private StateType state_ = new StateType();
		private MicroState.StateHandler<StateType> stateHandler_ = null;

#if UNITY_EDITOR
		private bool mightHaveChanges = false;
#endif
        #endregion

		public StateType State { get { return state_; } }

		public StateHandler<StateType> StateHandler { get {
			if (stateHandler_ == null) stateHandler_ = new StateHandler<StateType>(state_);
			return stateHandler_;
		}}
      
		#region Configurable Attributes      
		public bool PullChanges = false;
		public bool PushChanges = false;
		#endregion


		#region Events
        public UnityEvent ChangeEvent;
		#endregion

		void Awake()
        {
			if (state_ == null) state_ = new StateType();         


			state_.ChangeEvent.AddListener(this.OnStateChange);
         
			if (PushChanges) {
				StateType editorState = this.GetEditorState();
				this.state_.TakeContentFrom(editorState);
			}

			if (PullChanges) Pull(this.state_);
        }

		private void OnStateChange()
		{
			if (PullChanges) Pull(this.state_);
			this.ChangeEvent.Invoke();
		}

#if UNITY_EDITOR
        private void Update()
        {
            if (PushChanges && mightHaveChanges)
            {
				mightHaveChanges = false;
				// get a state instance with the value in our editorput our editor attributes into a state instance
				StateType editorState = this.GetEditorState();
				state_.TakeContentFrom(editorState);            
            }
        }

		void OnValidate()
        {
			this.mightHaveChanges = true;
        }
#endif

		/// <summary>
        /// Method should pull content from our state_ into our public properties
        /// </summary>
		protected virtual void Pull(StateType state) {
			// virtual
		}

		/// <summary>
        /// Method should push content from our public properties into our state_
        /// </summary>      
		protected virtual void Push(StateType state) {
            // virtual
		}
      
		protected StateType GetEditorState() {
			var state = new StateType();
			this.Push(state);
			return state;
		}      
	}
}