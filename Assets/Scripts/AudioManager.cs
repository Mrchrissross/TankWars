using UnityEngine;

[System.Serializable]
public class Sound
{
    //variables
    public string name;
    public AudioClip clip;
    private AudioSource source;
    //volume and pitch controller
    [Range(0f, 1f)]
    public float volume = 0.7f;
    [Range(0.5f, 1.5f)]
    public float pitch = 1.0f;
    //volume and pitch randomizer
    [Range(0f, 0.5f)]
    public float randomVolume = 0.1f;
    [Range(0f, 0.5f)]
    public float randomPitch = 0.1f;
    //true or false toggle to loop the sound
    public bool loop = false;

//This method is allows the script to assign the source clip.
    public void SetSource(AudioSource _source)
    {
        source = _source;
        source.clip = clip;
        source.loop = loop;
    }
//This method allow the sound to play
    public void Play()
    {
        source.volume = volume * (1 + Random.Range(-randomVolume / 2f, randomVolume / 2f));
        source.pitch = pitch * (1 + Random.Range(-randomPitch / 2f, randomPitch / 2f));
        source.Play();
    }
//This method allow the sound to stop
    public void Stop()
    {
        source.Stop();
    }
}

//The Audio Manager is created to allow the user to easily manage his or her sounds as well as easily assign them to various objects.
public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    
    //Sound array allows to choose how many sounds we want to display.
    [SerializeField]
    Sound[] sounds;

    //This is used when changing scenes, if there is already an Audio Manager open. this script will destroy the extra audio manager and only allow one open at a time.
    void Awake()
    {
        if (instance != null)
        {
            if (instance != this)
                Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
    }
    
    //This method is called upon as soon as the game starts and as seen below, it will play Menu-Music.
    void Start()
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            GameObject _go = new GameObject("Sound_" + i + "_" + sounds[i].name);
            _go.transform.SetParent(this.transform);
            sounds[i].SetSource(_go.AddComponent<AudioSource>());
        }
    }
    
    //This method will play the sound whenever it is scripted into one of the other gameobjects. 
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
//This will do the opposite to the PlaySound, and Stop the sound from playing.
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

}
