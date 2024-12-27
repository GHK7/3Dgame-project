using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using UnityEngine.UI;

public class Video : MonoBehaviour
{
    public VideoPlayer videoPlayer; // 影片播放器
    public CanvasGroup button1CanvasGroup; // 第一個按鈕的 Canvas Group
    public CanvasGroup button2CanvasGroup; // 第二個按鈕的 Canvas Group
    public float fadeDuration = 1f; // 每個按鈕的淡入持續時間（秒）
    public float delayBetweenButtons = 0.5f; // 兩個按鈕淡入的延遲時間
    public GameObject skipButton; // 跳過按鈕
    public GameObject Buttom;
    public GameObject Buttom2;
    private AudioSource audioSource;
    public AudioClip ButtonSound;

    private void Start()
    {
        videoPlayer.loopPointReached += OnVideoEnd;
        SetButtonState(button1CanvasGroup, false);
        SetButtonState(button2CanvasGroup, false);
        if (skipButton != null)
        {
            skipButton.SetActive(false); // 隱藏跳過按鈕
        }
        audioSource = gameObject.AddComponent<AudioSource>();

        // 啟動協程在影片播放開始後顯示跳過按鈕
        StartCoroutine(ShowSkipButtonAfterDelay(3f));
    }

    private void Update()
    {
    }

    private IEnumerator ShowSkipButtonAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (skipButton != null)
        {
            skipButton.SetActive(true); // 顯示跳過按鈕
        }
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        StartCoroutine(FadeInButtons());
    }

    private IEnumerator FadeInButtons()
    {
        yield return StartCoroutine(FadeInButton(button1CanvasGroup)); // 淡入第一個按鈕
        yield return new WaitForSeconds(delayBetweenButtons); // 延遲
        yield return StartCoroutine(FadeInButton(button2CanvasGroup)); // 淡入第二個按鈕
    }

    // 單個按鈕的淡入效果
    private IEnumerator FadeInButton(CanvasGroup canvasGroup)
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            yield return null;
        }

        // 啟用按鈕交互
        SetButtonState(canvasGroup, true);
    }

    private void SetButtonState(CanvasGroup canvasGroup, bool state)
    {
        canvasGroup.alpha = state ? 1 : 0;
        canvasGroup.interactable = state;
        canvasGroup.blocksRaycasts = state;
    }

    void OnDestroy()
    {
        // 取消訂閱事件以避免內存洩漏
        videoPlayer.loopPointReached -= OnVideoEnd;
    }

    public void PlayGame()
    {
        SceneManager.LoadSceneAsync("MainMenu");
        PlayButtonSound();
    }

    public void VideoPlayAgain()
    {
        SceneManager.LoadSceneAsync("Video");
        PlayButtonSound();
    }

    public void SkipVideo()
    {
        videoPlayer.Stop(); // 停止影片播放
        OnVideoEnd(videoPlayer); // 手動觸發影片結束邏輯
        SceneManager.LoadSceneAsync("MainMenu");
    }

    private void PlayButtonSound()
    {
        if (audioSource != null && ButtonSound != null)
        {
            audioSource.PlayOneShot(ButtonSound);
        }
    }
}
