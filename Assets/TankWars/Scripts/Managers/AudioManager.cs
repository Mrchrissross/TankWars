using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TankWars.Managers
{
    /// <summary>
    /// Class created for the storage of sound data.
    /// </summary>
    
    [System.Serializable]
    public class Sound
    {
        #region Fields

        // Unity Inspector.
        [HideInInspector] public bool hideSection;
        
        // Variables.
        public string name;
        public AudioClip clip;
        public AudioSource audioSource;
        public List<AudioSource> audioSourcePool = new List<AudioSource>();

        // Volume and pitch controller.
        public Vector2 audioVolume = new Vector2(0.5f, 0.9f);
        public Vector2 audioPitch = new Vector2(0.8f, 1.2f);

        // Loops the sound.
        public bool loop;

        #endregion
        
        
        
        #region Functions

        /// <summary>
        /// Assigns the source clip.
        /// </summary>
        /// <param name="source">The audio source component.</param>
        
        public void SetSource(AudioSource source) => audioSource = source;

        /// <summary>
        /// Plays a sound.
        /// </summary>
        
        public void Play()
        {
            var sourceToPlay = audioSource;
            
            if (Application.isPlaying && IsPlaying())
            {
                while (true)
                {
                    var length = audioSourcePool.Count;

                    if (length == 0)
                    {
                        sourceToPlay = AddSource(length);
                        break;
                    }
                    
                    foreach (var source in audioSourcePool.Where(source => !source.isPlaying))
                    {
                        sourceToPlay = source;
                        break;
                    }
                    
                    if(sourceToPlay == audioSource)
                        sourceToPlay = AddSource(length);
                    
                    break;
                }
            }
            
            sourceToPlay.clip = clip;
            sourceToPlay.loop = loop;
            
            sourceToPlay.volume = Random.Range(audioVolume.x, audioVolume.y);
            sourceToPlay.pitch = Random.Range(audioPitch.x, audioPitch.y);
            
            sourceToPlay.Play();
        }

        /// <summary>
        /// Stops a sound.
        /// </summary>
        
        public void Stop() => audioSource.Stop();

        /// <summary>
        /// Pauses a sound.
        /// </summary>
        
        public void Pause() => audioSource.Pause();

        /// <summary>
        /// Checks if sound is playing.
        /// </summary>
        
        public bool IsPlaying() => audioSource.isPlaying;

        /// <summary>
        /// Adds an audio source to the pool.
        /// </summary>
        
        public AudioSource AddSource(int index)
        {
            var go = new GameObject("Sound " + index);
            go.transform.SetParent(audioSource.transform);
            
            var source = go.AddComponent<AudioSource>();
            source.playOnAwake = false;
            
            audioSourcePool.Add(source);
            
            return source;
        }
        
        /// <summary>
        /// Creates a new sound.
        /// </summary>
        
        public Sound() => name = "new Sound";

        /// <summary>
        /// Creates a copy from another sound.
        /// </summary>
        
        public Sound(Sound copy)
        {
            name = copy.name + "copy";
            clip = copy.clip;
            audioVolume = copy.audioVolume;
            audioPitch = copy.audioPitch;
            loop = copy.loop;
        }
        
        #endregion
        
    }

    
    
    
    
    /// <summary>
    /// The audio manager easily manages, assigns and uses sounds.  Best usage is in games that do use 3D reverberation.
    /// </summary>
    
    public sealed class AudioManager : Singleton<AudioManager>
    {

        #region Fields

        // Storage of sounds, also disable warning saying it is unused.
        #pragma warning disable 0649
        public List<Sound> sounds = new List<Sound>();
        #pragma warning restore 0649
        
        #endregion

        
        
        #region Functions

        /// <summary>
        /// Plays the named sound. 
        /// </summary>
        /// <param name="soundName">The name of the sound.</param>
        
        public void PlaySound(string soundName) => GetSound(soundName)?.Play();

        /// <summary>
        /// Stops the named sound.
        /// </summary>
        /// <param name="soundName">The name of the sound.</param>
        
        public void StopSound(string soundName) => GetSound(soundName)?.Stop();

        /// <summary>
        /// Pauses the named sound.
        /// </summary>
        /// <param name="soundName">The name of the sound.</param>
        
        public void PauseSound(string soundName) => GetSound(soundName)?.Pause();

        /// <summary>
        /// Checks if the named sound is playing.
        /// </summary>
        /// <param name="soundName">The name of the sound.</param>
        
        public bool IsPlaying(string soundName) => (from sound in sounds where sound.name == soundName select sound.IsPlaying()).FirstOrDefault();

        /// <summary>
        /// Returns the specified sound from the sound list.
        /// </summary>
        /// <param name="soundName">Name of the desired sound.</param>
        
        public Sound GetSound(string soundName)
        {
            if (soundName == "None") return null;
            
            foreach (var sound in sounds.Where(sound => sound.name == soundName))
                return sound;
            
            Debug.LogWarning("Audio Manager: Sound not found in list." + soundName);
            return null;
        }

        /// <summary>
        /// Creates a new sound.
        /// </summary>

        public void AddSound()
        {
            var sound = new Sound();
            sounds.Add(sound);
        } 
        
        /// <summary>
        /// Creates a copy of specified sound.
        /// </summary>
        /// <param name="sound">Sound to copy.</param>
        
        public void CopySound(Sound sound) => sounds.Add(new Sound(sound));

        /// <summary>
        /// Creates the audio source for a sound.
        /// </summary>
        /// <param name="index">Index within the list of sounds.</param>
        /// <param name="sound"></param>
        
        public void CreateSource(int index, Sound sound)
        {
            var goName = "Sound " + index + ": " + sound.name;

            if (sound.audioSource != null)
            {
                sound.audioSource.name = goName;
                return;
            }
            
            var go = new GameObject(goName);
            go.transform.SetParent(transform);
            sound.SetSource(go.AddComponent<AudioSource>());
            sound.audioSource.playOnAwake = false;
        }
        
        #endregion

        
        
        
        #region MonoBehaviour

        /// <summary>
        /// Initialises all sounds.
        /// </summary>
        
        private void Start()
        {
            for (var index = 0; index < sounds.Count; index++) 
                CreateSource(index, sounds[index]);
        }

        #endregion        
    }
}
