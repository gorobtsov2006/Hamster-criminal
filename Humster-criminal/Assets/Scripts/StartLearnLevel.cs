using UnityEngine;
using UnityEngine.SceneManagement;

public class StartLearnLevel : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("LearnLevel");
    }
}