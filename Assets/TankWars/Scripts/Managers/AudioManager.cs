using System.Linq;
using UnityEngine;

namespace TankWars.Managers
{
    /// <summary>
    /// Class created for the storage of sound data.
    /// </summary>
    
    [System.Serializable]
    public class Sound
    {

        #region Fields

        // variables.
        public string name;
        public AudioClip clip;
        private AudioSource _audioSource;

        // Volume and pitch controller.
        [Range(0.0f, 1.0f)] public float volume = 0.7f;
        [Range(0.5f, 1.5f)] public float pitch = 1.0f;

        // Randomizes volume and pitch.
        [Range(0f, 0.5f)] public float randomVolume = 0.1f;
        [Range(0f, 0.5f)] public float randomPitch = 0.1f;

        // Loops the sound.
        public bool loop;

        #endregion
        
        
        
        #region Functions

        /// <summary>
        /// Assigns the source clip.
        /// </summary>
        /// <param name="source">The audio source component.</param>
        
        public void SetSource(AudioSource source)
        {
            _audioSource = source;
            _audioSource.clip = clip;
            _audioSource.loop = loop;
        }

        /// <summary>
        /// Plays a sound.
        /// </summary>
        
        public void Play()
        {
            _audioSource.volume = volume * (1 + Random.Range(-randomVolume / 2f, randomVolume / 2f));
            _audioSource.pitch = pitch * (1 + Random.Range(-randomPitch / 2f, randomPitch / 2f));
            _audioSource.Play();
        }

        /// <summary>
        /// Stops a sound.
        /// </summary>
        
        public void Stop() => _audioSource.Stop();

        /// <summary>
        /// Pauses a sound.
        /// </summary>
        
        public void Pause() => _audioSource.Pause();

        /// <summary>
        /// Checks if sound is playing.
        /// </summary>
        
        public bool IsPlaying() => _audioSource.isPlaying;
        
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
        [SerializeField] private Sound[] sounds;
        #pragma warning restore 0649
        
        #endregion

        
        
        #region Functions

        /// <summary>
        /// Initialises all sounds.
        /// </summary>
        
        private void Start()
        {
            for (var i = 0; i < sounds.Length; i++)
            {
                var go = new GameObject("Sound_" + i + "_" + sounds[i].name);
                go.transform.SetParent(this.transform);
                sounds[i].SetSource(go.AddComponent<AudioSource>());
            }
        }

        /// <summary>
        /// Plays the named sound. 
        /// </summary>
        /// <param name="soundName">The name of the sound.</param>
        
        public void PlaySound(string soundName)
        {
            foreach (var sound in sounds)
            {
                if (sound.name != soundName) continue;
                
                sound.Play();
                return;
            }

            Debug.LogWarning("Audio Manager: Sound not found in list." + soundName);
        }

        /// <summary>
        /// Stops the named sound.
        /// </summary>
        /// <param name="soundName">The name of the sound.</param>
        
        public void StopSound(string soundName)
        {
            foreach (var sound in sounds)
            {
                if (sound.name != soundName) continue;
                
                sound.Stop();
                return;
            }
        }

        /// <summary>
        /// Pauses the named sound.
        /// </summary>
        /// <param name="soundName">The name of the sound.</param>
        
        public void PauseSound(string soundName)
        {
            foreach (var sound in sounds)
            {
                if (sound.name != soundName) continue;
                
                sound.Pause();
                return;
            }
        }

        /// <summary>
        /// Checks if the named sound is playing.
        /// </summary>
        /// <param name="soundName">The name of the sound.</param>
        
        public bool IsPlaying(string soundName) => (from sound in sounds where sound.name == soundName select sound.IsPlaying()).FirstOrDefault();
        
        #endregion 
        
    }
}
