using UnityEngine;
using UnityEngine.UI;

public class ButtonSound : MonoBehaviour
{
    public AudioClip buttonSound; // 拖入相同的按鈕音效文件
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void PlaySound()
    {
        if (buttonSound != null)
        {
            audioSource.PlayOneShot(buttonSound);
        }
        else
        {
            Debug.LogWarning("未設置音效！");
        }
    }
}
