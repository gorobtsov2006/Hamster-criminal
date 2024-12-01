using UnityEngine;
using UnityEngine.UI;

public class VolumeController : MonoBehaviour
{
    public Slider volumeSlider;

    private const string VolumeKey = "GameVolume";

    private void Start()
    {
        if (PlayerPrefs.HasKey(VolumeKey))
        {
            float savedVolume = PlayerPrefs.GetFloat(VolumeKey);
            AudioListener.volume = savedVolume;
            volumeSlider.value = savedVolume;
        }
        else
        {
            AudioListener.volume = 1f;
            volumeSlider.value = 1f;
        }

        volumeSlider.onValueChanged.AddListener(SetVolume);
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat(VolumeKey, volume);
    }
}
