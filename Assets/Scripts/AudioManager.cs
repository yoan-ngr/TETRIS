using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// The <b>AudioManager</b> class is responsible for managing the audio in the game, including music and sound.
/// </summary>
public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;

    /// <summary>
    /// The AudioManager singleton instance.
    /// </summary>
    public static AudioManager Instance;
    
    /// <summary>
    /// Is the music active ?
    /// </summary>
    public bool MusicActive { get; private set; }
    
    /// <summary>
    /// Is the sound active ?
    /// </summary>
    public bool SoundActive { get; private set; }
    
    // PlayerPref volume and sound key names
    private const string PlayerPrefMusicKey = "MusicActive";
    private const string PlayerPrefSoundKey = "SoundActive";

    // Update events
    public delegate void OnUpdateMusic();
    public delegate void OnUpdateSound();

    public OnUpdateMusic onUpdateMusic;
    public OnUpdateSound onUpdateSound;

    private void Awake()
    {
        // Singleton creation
        if (!Instance)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        // If there's no preference for volume and sound, create them
        if (!PlayerPrefs.HasKey(PlayerPrefMusicKey) || !PlayerPrefs.HasKey(PlayerPrefSoundKey))
        {
            PlayerPrefs.SetInt(PlayerPrefMusicKey, 1);
            PlayerPrefs.SetInt(PlayerPrefSoundKey, 1);
        }
        
        // Retrieve the preferences
        MusicActive = (PlayerPrefs.GetInt(PlayerPrefMusicKey) == 1);
        SoundActive = (PlayerPrefs.GetInt(PlayerPrefSoundKey) == 1);
        
        // Apply them
        SetMusic(MusicActive);
        SetSound(SoundActive);
    }

    /// <summary>
    /// Enables or disables the music.
    /// </summary>
    /// <param name="active"></param>
    public void SetMusic(bool active)
    {
        audioMixer.SetFloat("MusicVolume", active ? 0f : -80f);
        MusicActive = active;
        PlayerPrefs.SetInt(PlayerPrefMusicKey, active ? 1 : 0);
        onUpdateMusic?.Invoke();
    }
    
    /// <summary>
    /// Enables or disables the sound.
    /// </summary>
    /// <param name="active"></param>
    public void SetSound(bool active)
    {
        audioMixer.SetFloat("SoundVolume", active ? 0f : -80f);
        SoundActive = active;
        PlayerPrefs.SetInt(PlayerPrefSoundKey, active ? 1 : 0);
        onUpdateSound?.Invoke();
    }
}
