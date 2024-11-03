using UnityEngine;
using UnityEngine.SceneManagement; // 引入場景管理命名空間

public class TriggerVictory : MonoBehaviour
{
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
        SceneManager.LoadScene("VictoryScene");
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
