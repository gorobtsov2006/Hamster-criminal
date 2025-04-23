using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public GameObject musicPrefab; 
    public AudioClip[] musicTracks; 
    private AudioSource audioSource;
    private static MusicManager instance;

    void Awake()
    {
        
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        
        if (musicPrefab == null)
        {
            Debug.LogError("MusicPrefab не назначен!");
            return;
        }

        
        if (musicTracks == null || musicTracks.Length == 0)
        {
            Debug.LogError("Массив musicTracks пуст! Назначьте хотя бы один трек в инспекторе.");
            return;
        }

        
        GameObject existingMusic = GameObject.Find("BGMusic");
        if (existingMusic == null)
        {
            GameObject musicObject = Instantiate(musicPrefab);
            musicObject.name = "BGMusic";
            DontDestroyOnLoad(musicObject);

            audioSource = musicObject.GetComponent<AudioSource>();
            if (audioSource == null)
            {
                Debug.LogError("AudioSource не найден на MusicPrefab!");
                return;
            }
        }
        else
        {
            audioSource = existingMusic.GetComponent<AudioSource>();
        }
    }

    void Start()
    {
        
        if (audioSource != null && musicTracks.Length > 0)
        {
            audioSource.clip = musicTracks[0]; 
            audioSource.volume = 0.4f; 
            audioSource.loop = true; 
            audioSource.Play();
            Debug.Log("Музыка запущена: " + audioSource.clip.name);
        }
    }

    
    public void StopMusic()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }

    
    public void SetVolume(float volume)
    {
        if (audioSource != null)
        {
            audioSource.volume = Mathf.Clamp01(volume);
        }
    }
}