using System;
using System.Collections.Generic;
// using MoveBot.Controllers;
using UnityEngine;

namespace TankWars.Utility
{
    
    /// <summary>
    /// The recorder component is a tool that rewinds and fast forwards the character allowing better debugging.
    /// </summary>
    
    public class Recorder : MonoBehaviour
    {
        // /// <summary>
        // /// Class used for the storage of recorded character data.
        // /// </summary>
        //
        // public class Record
        // {
        //     public Vector3 position;
        //     public Quaternion rotation;
        //     public Vector3 velocity;
        //     public Vector3 angularVelocity;
        //
        //     public Record(Vector3 _position, Quaternion _rotation, Vector3 _velocity, Vector3 _angularVelocity)
        //     {
        //         position = _position;
        //         rotation = _rotation;
        //         velocity = _velocity;
        //         angularVelocity = _angularVelocity;
        //     }
        // }
        //
        //
        //
        //
        //
        // #region Cached Components
        //
        // private new Rigidbody rigidbody
        // {
        //     get
        //     {
        //         if (_rigidbody) return _rigidbody;
        //
        //         _rigidbody = GetComponent<Rigidbody>();
        //         ExtensionsLibrary.CheckComponent(_rigidbody, "Rigidbody Component", name);
        //         return _rigidbody;
        //     }
        // }
        //
        // [Tooltip("Cached 'Rigidbody' component.")]
        // private Rigidbody _rigidbody;
        //
        //
        // private CharacterData data
        // {
        //     get
        //     {
        //         if (_data) return _data;
        //
        //         _data = GetComponent<CharacterData>();
        //         ExtensionsLibrary.CheckComponent(_data, "CharacterData Component", name);
        //         return _data;
        //     }
        // }
        //
        // [Tooltip("Cached 'CharacterData' component.")]
        // private CharacterData _data;
        //
        // #endregion
        //
        //
        //
        //
        //
        //
        // #region Properties
        //
        // /// <summary>
        // /// Pauses the character and pauses the recording.
        // /// Be aware that when unpaused, the previous velocity and angular velocity is reapplied and all rewound data is erased.
        // /// </summary>
        //
        // public bool isPaused
        // {
        //     get => _isPaused;
        //     set
        //     {
        //         _isPaused = value;
        //         data.pause = value;
        //         rigidbody.isKinematic = value;
        //
        //         if (_isPaused) return;
        //
        //         rigidbody.velocity = records[index].velocity;
        //         rigidbody.angularVelocity = records[index].angularVelocity;
        //
        //         for (var i = 0; i < index; i++) records.RemoveAt(0);
        //
        //         index = 0;
        //     }
        // }
        // [SerializeField, ReadOnly] public bool _isPaused;
        // public KeyCode pause = KeyCode.Keypad5;
        //
        // /// <summary>
        // /// Rewinds the character, moving to previous positions.
        // /// </summary>
        //
        // public bool isRewinding
        // {
        //     get => _isRewinding;
        //     set
        //     {
        //         _isRewinding = value;
        //         if (_isRewinding) isPaused = true;
        //     }
        // }
        // [SerializeField, ReadOnly] private bool _isRewinding;
        // public KeyCode rewind = KeyCode.Keypad4;
        //
        // /// <summary>
        // /// Fast forwards the character, moving to future positions that may have been rewound past.
        // /// </summary>
        //
        // public bool isFastForwarding
        // {
        //     get => _isFastForwarding;
        //     set
        //     {
        //         _isFastForwarding = value;
        //         if (_isFastForwarding) isPaused = true;
        //     }
        // }
        // [SerializeField, ReadOnly] private bool _isFastForwarding;
        // public KeyCode fastForward = KeyCode.Keypad6;
        //
        //
        //
        // /// <summary>
        // /// This is the maximum record time (in sec). Be aware recording requires memory.
        // /// </summary>
        //
        // private float recordTime
        // {
        //     get => Mathf.Round(_recordTime / Time.fixedDeltaTime);
        //     set => _recordTime = Mathf.Max(0.0f, value);
        // }
        // [Space] public float _recordTime = 5f;
        //
        // /// <summary>
        // /// This list holds all the currently recorded data.
        // /// </summary>
        //
        // [HideInInspector] public List<Record> records = new List<Record>();
        //
        // /// <summary>
        // /// This is the frame index.
        // /// </summary>
        //
        // public int index = 0;
        //
        // #endregion
        //
        //
        //
        //
        //
        // #region Functions
        //
        // /// <summary>
        // /// Handles the users input and applies the relevant methods.
        // /// </summary>
        //
        // private void HandleInput()
        // {
        //     if (Input.GetKeyDown(rewind)) isRewinding = true;
        //     if (Input.GetKeyUp(rewind)) isRewinding = false;
        //
        //     if (Input.GetKeyDown(pause)) isPaused = !_isPaused;
        //
        //     if (Input.GetKeyDown(fastForward)) isFastForwarding = true;
        //     if (Input.GetKeyUp(fastForward)) isFastForwarding = false;
        // }
        //
        // /// <summary>
        // /// Records the characters rigidbody data.
        // /// </summary>
        //
        // private void RecordCharacter()
        // {
        //     if (records.Count > recordTime) records.RemoveAt(records.Count - 1);
        //
        //     records.Insert(0,
        //         new Record(rigidbody.position, rigidbody.rotation, rigidbody.velocity, rigidbody.angularVelocity));
        // }
        //
        // #endregion
        //
        //
        //
        //
        //
        // #region Monobehaviour
        //
        // private void Update()
        // {
        //     // Handle user input.
        //     HandleInput();
        // }
        //
        // public void FixedUpdate()
        // {
        //     // If paused, apply the recorded data.            
        //     if (isPaused)
        //     {
        //         if (isRewinding && records.Count - 1 > index) index++;
        //         if (isFastForwarding && index > 0) index--;
        //
        //         var record = records[index];
        //         rigidbody.position = record.position;
        //         rigidbody.rotation = record.rotation;
        //
        //         return;
        //     }
        //
        //     // If not paused, continue recording the character.
        //     RecordCharacter();
        // }
        //
        // #endregion
        //
    }
}