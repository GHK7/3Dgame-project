using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI; // 暫停選單的 UI 物件
    public Slider volumeSlider; // 調整音量的 Slider
    private bool isPaused = false;

    private void Start()
    {
        // 初始化音量 Slider，假設音量預設為1
        volumeSlider.value = AudioListener.volume;
        volumeSlider.onValueChanged.AddListener(AdjustVolume);

        // 隱藏暫停選單
        pauseMenuUI.SetActive(false);
    }

    private void Update()
    {
        // 檢查鍵盤的 Escape 鍵
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    private void TogglePause()
    {
        if (isPaused)
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false); // 隱藏暫停選單
        Time.timeScale = 1; // 恢復遊戲時間
        isPaused = false;
    }

    private void Pause()
    {
        pauseMenuUI.SetActive(true); // 顯示暫停選單
        Time.timeScale = 0; // 暫停遊戲時間
        isPaused = true;
    }

    // 調整音量
    private void AdjustVolume(float volume)
    {
        AudioListener.volume = volume;
    }

    // 返回主選單
    public void LoadMainMenu()
    {
        Time.timeScale = 1; // 確保遊戲時間恢復正常
        SceneManager.LoadScene("MainMenu");
    }

    // 退出遊戲
    public void QuitGame()
    {
        Application.Quit();
    }

    public void PauseButton()
    {
        Pause();
    }
}
