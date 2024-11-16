using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadSceneAsync("SampleScene");
    }

    public void Credit()
    {
        SceneManager.LoadSceneAsync("Credit");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void BackMenu()
    {
        SceneManager.LoadSceneAsync("MainMenu");
    }
}
