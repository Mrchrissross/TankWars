using UnityEngine;

[System.Serializable]
public class Sound
{
    // variables
    public string name;
    public AudioClip clip;
    private AudioSource source;

    // volume and pitch controller
    [Range(0f, 1f)]
    public float volume = 0.7f;
    [Range(0.5f, 1.5f)]
    public float pitch = 1.0f;

    // volume and pitch randomizer
    [Range(0f, 0.5f)]
    public float randomVolume = 0.1f;
    [Range(0f, 0.5f)]
    public float randomPitch = 0.1f;

    // true or false toggle to loop the sound
    public bool loop = false;

    /// <summary>
    /// Allows the script to assign the source clip.
    /// </summary>
    /// <param name="_source">The audio source component.</param>
    public void SetSource(AudioSource _source)
    {
        source = _source;
        source.clip = clip;
        source.loop = loop;
    }

    /// <summary>
    /// Plays a sound.
    /// </summary>
    public void Play()
    {
        source.volume = volume * (1 + Random.Range(-randomVolume / 2f, randomVolume / 2f));
        source.pitch = pitch * (1 + Random.Range(-randomPitch / 2f, randomPitch / 2f));
        source.Play();
    }

    /// <summary>
    /// Stops a sound.
    /// </summary>
    public void Stop()
    {
        source.Stop();
    }

    /// <summary>
    /// Pauses a sound.
    /// </summary>
    public void Pause()
    {
        source.Pause();
    }

    /// <summary>
    /// Checks if sound is playing.
    /// </summary>
    public bool IsPlaying()
    {
        return source.isPlaying;
    }
}

/// <summary>
/// The Audio Manager is created to allow the user to easily manage his or her sounds as well as easily assign them to various objects.
/// </summary>
public class AudioManager : Singleton<AudioManager>
{
    //Sound array allows to choose how many sounds we want to display.
    [SerializeField]
    Sound[] sounds;

    /// <summary>
    /// Sets up all the sounds.
    /// </summary>
    protected virtual void Start()
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            GameObject _go = new GameObject("Sound_" + i + "_" + sounds[i].name);
            _go.transform.SetParent(this.transform);
            sounds[i].SetSource(_go.AddComponent<AudioSource>());
        }
    }

    /// <summary>
    /// Plays the named sound. 
    /// </summary>
    /// <param name="name">The name of the sound.</param>
    public void PlaySound(string name)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].name == name)
            {
                sounds[i].Play();
                return;
            }
        }
        Debug.LogWarning("AudioManager: Sound not found in list. " + name);
    }

    /// <summary>
    /// Stops the named sound.
    /// </summary>
    /// <param name="name">The name of the sound.</param>
    public void StopSound(string name)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].name == name)
            {
                sounds[i].Stop();
                return;
            }
        }
    }

    /// <summary>
    /// Pauses the named sound.
    /// </summary>
    /// <param name="name">The name of the sound.</param>
    public void PauseSound(string name)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].name == name)
            {
                sounds[i].Pause();
                return;
            }
        }
    }

    /// <summary>
    /// Checks if a sound is playing.
    /// </summary>
    /// <param name="name">The name of the sound.</param>
    public bool IsPlaying(string name)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].name == name)
                return sounds[i].IsPlaying();
        }

        return false;
    }
}