using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TankWars.Utility
{
    
    /// <summary>
    /// The recorder component is a tool that rewinds and fast forwards an object allowing for better debugging.
    /// </summary>
    
    public class Recorder : MonoBehaviour
    {
        
        #region Classes
        
        /// <summary>
        /// Class used for the storage of recorded object data.
        /// </summary>
        
        public class Record
        {
            public Vector2 position;
            public Quaternion rotation;
        
            public Record(Vector3 _position, Quaternion _rotation)
            {
                position = _position;
                rotation = _rotation;
            }
        }

        #endregion
        
        
        
        
        
        #region Properties
        
        /// <summary>
        /// Pauses the object and pauses the recording.
        /// Be aware that when unpaused, the previous velocity and angular velocity is reapplied and all rewound data is erased.
        /// </summary>
        
        public bool IsPaused
        {
            get => isPaused;
            set
            {
                isPaused = value;

                if (isPaused)
                {
                    onPauseEvents.Invoke();
                    return;
                }
                
                onResumeEvents.Invoke();
                
                for (var i = 0; i < index; i++) records.RemoveAt(0);
        
                index = 0;
            }
        }
        [SerializeField] public bool isPaused;
        public KeyCode pause = KeyCode.Keypad5;
        
        /// <summary>
        /// Rewinds the object, moving to previous positions.
        /// </summary>
        
        public bool IsRewinding
        {
            get => isRewinding;
            set
            {
                isRewinding = value;
                if (isRewinding) IsPaused = true;
            }
        }
        [SerializeField] private bool isRewinding;
        public KeyCode rewind = KeyCode.Keypad4;
        
        /// <summary>
        /// Fast forwards the object, moving to future positions that may have been rewound past.
        /// </summary>
        
        public bool IsFastForwarding
        {
            get => isFastForwarding;
            set
            {
                isFastForwarding = value;
                if (isFastForwarding) IsPaused = true;
            }
        }
        [SerializeField] private bool isFastForwarding;
        public KeyCode fastForward = KeyCode.Keypad6;
        
        /// <summary>
        /// This is the maximum record time (in sec). Be aware recording requires memory.
        /// </summary>
        
        private float RecordTime
        {
            get => Mathf.Round(recordTime / Time.fixedDeltaTime);
            set => recordTime = Mathf.Max(0.0f, value);
        }
        [Space] public float recordTime = 5f;
        
        #endregion

        
        
        
        
        #region Fields

        [SerializeField] private UnityEvent onPauseEvents;
        [SerializeField] private UnityEvent onResumeEvents;
        
        // This list holds all the currently recorded data.
        [HideInInspector] public List<Record> records = new List<Record>();
        
        // This is the frame index.
        public int index = 0;

        #endregion
        
        
        
        
        
        #region Functions
        
        /// <summary>
        /// Handles the users input and applies the relevant methods.
        /// </summary>
        
        private void HandleInput()
        {
            if (Input.GetKeyDown(rewind)) IsRewinding = true;
            if (Input.GetKeyUp(rewind)) IsRewinding = false;
        
            if (Input.GetKeyDown(pause)) IsPaused = !isPaused;
        
            if (Input.GetKeyDown(fastForward)) IsFastForwarding = true;
            if (Input.GetKeyUp(fastForward)) IsFastForwarding = false;
        }
        
        /// <summary>
        /// Records the objects tranform data.
        /// </summary>
        
        private void RecordObject()
        {
            if (records.Count > RecordTime) records.RemoveAt(records.Count - 1);
        
            records.Insert(0, new Record(transform.position, transform.rotation));
        }
        
        #endregion
        
        
        
        
        
        #region Monobehaviour
        
        /// <summary>
        ///  Handle user input.
        /// </summary>
        
        private void Update() => HandleInput();
        
        /// <summary>
        /// Perform recording.
        /// </summary>
        
        public void FixedUpdate()
        {
            // If paused, apply the recorded data.            
            if (IsPaused)
            {
                if (IsRewinding && records.Count - 1 > index) index++;
                if (IsFastForwarding && index > 0) index--;
        
                var record = records[index];
                transform.position = record.position;
                transform.rotation = record.rotation;
        
                return;
            }
        
            // If not paused, continue recording the object.
            RecordObject();
        }
        
        #endregion
        
    }
}