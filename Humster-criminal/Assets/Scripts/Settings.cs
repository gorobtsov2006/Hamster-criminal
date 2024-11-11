using UnityEngine;
using UnityEngine.SceneManagement;

public class Settings : MonoBehaviour
{
    public void OpenSettings()
    {
        SceneManager.LoadScene("Settings");
    }
}
