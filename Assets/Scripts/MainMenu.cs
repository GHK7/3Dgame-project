using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    public GameObject playUI;

    private void Start()
    {
        // 隱藏選單
        playUI.SetActive(false);
    }


    private void Update()
    {
        // 檢查鍵盤的 Escape 鍵
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.JoystickButton0) && !EventSystem.current.IsPointerOverGameObject())
        {

        }
    }


    public void PlayUI()
    {
        playUI.SetActive(true);
        Time.timeScale = 0;
    }

    public void PlayGame()
    {
        SceneManager.LoadSceneAsync("Game");
        Time.timeScale = 1;

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
