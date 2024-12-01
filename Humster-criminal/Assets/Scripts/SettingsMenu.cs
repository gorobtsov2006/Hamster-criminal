using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
public class SettingsMenu : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown;
    public TMP_Dropdown qualityDropdown;
    
    Resolution[] resolutions;

    //Toggle
    public Toggle toggle;
    public Sprite spriteOn;
    public Sprite spriteOff;
    private Image toggleImage;
    // Start is called before the first frame update
    void Start()
    {
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        resolutions = Screen.resolutions;
        int curResolutionIndex = 0;
        
        for(int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width+"x"+resolutions[i].height+" "+resolutions[i].refreshRateRatio+"Hz";
            options.Add(option);
            if (resolutions[i].width==Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                curResolutionIndex = i;
            }
        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.RefreshShownValue();
        LoadSettings(curResolutionIndex);

        //Toggle
        toggleImage = toggle.targetGraphic as Image;
        UpdateToggleAppearance(toggle.isOn);
        toggle.onValueChanged.AddListener(UpdateToggleAppearance);
    }



    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width,resolution.height,Screen.fullScreen);
    }
    
    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }
    public void ExitSettings()
    {
        SceneManager.LoadScene("Menu");
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetInt("QualitySettingPreference",qualityDropdown.value);
        PlayerPrefs.SetInt("ResolutionPreference",resolutionDropdown.value);
        PlayerPrefs.SetInt("FullscreenPreference", System.Convert.ToInt32(Screen.fullScreen));
    }

    public void LoadSettings(int currentResolutionIndex)
    {
        if (PlayerPrefs.HasKey("QualitySettingPreference"))
        {
            qualityDropdown.value = PlayerPrefs.GetInt("QualitySettingPreference");
        }
        else
        {
            qualityDropdown.value = 3;
        }
        if (PlayerPrefs.HasKey("ResolutionPreference"))
        {
            resolutionDropdown.value = PlayerPrefs.GetInt("ResolutionPreference");
        }
        else
        {
            resolutionDropdown.value = currentResolutionIndex;
        }
        if (PlayerPrefs.HasKey("FullscreenPreference"))
        {
            Screen.fullScreen = System.Convert.ToBoolean(PlayerPrefs.GetInt("FullscreenPreference"));
        }
        else
        {
            Screen.fullScreen = true;
        }
    }
    //Toggle
    void UpdateToggleAppearance(bool isOn)
    {
        if (toggleImage != null)
        {
            toggleImage.sprite = isOn ? spriteOn : spriteOff;
        }
    }
}
