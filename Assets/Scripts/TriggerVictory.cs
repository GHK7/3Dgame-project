using UnityEngine;
using UnityEngine.SceneManagement; // 引入場景管理命名空間
using UnityEngine.Video;
using System.Collections;

public class TriggerVictory : MonoBehaviour
{
    public VideoPlayer videoPlayer; // 影片播放器
    public string targetScene;
    public float delay = 3f;


    void Start()
    {
        // 檢查是否指定了VideoPlayer
        if (videoPlayer == null)
        {
            videoPlayer = GetComponent<VideoPlayer>();
        }

        // 設定影片播放完成事件的回呼函數
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached += OnVideoEnd;
        }
        else
        {
            Debug.LogError("VideoPlayer not assigned!");
        }
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        StartCoroutine(SwitchSceneWithDelay());
    }

    private IEnumerator SwitchSceneWithDelay()
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("VictoryScene");
    }

    // 確保在對象被銷毀時移除回呼
    private void OnDestroy()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoEnd;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 檢查進入觸發器的對象是否是玩家
        if (other.CompareTag("Player"))
        {
            // 暫停時間
            Time.timeScale = 0;

            // 加載勝利場景
            // 需要使用協程來先暫停，再跳轉場景
            StartCoroutine(LoadVictoryScene());
        }
    }

    private System.Collections.IEnumerator LoadVictoryScene()
    {
        // 等待一個幀，以便暫停生效
        yield return null;

        // 恢復時間縮放（可選：如果希望場景加載後時間恢復）
        Time.timeScale = 1;

        // 加載勝利場景
        SceneManager.LoadScene("Video 1");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void BackMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadSceneAsync("MainMenu");
    }

    public void Credit()
    {
        Time.timeScale = 1;
        SceneManager.LoadSceneAsync("Credit");
    }
}
