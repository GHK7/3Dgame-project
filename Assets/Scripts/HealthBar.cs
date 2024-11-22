using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;

    private void Update()
    {
        if (slider.value == 0)
        {
            EndGame();
        }
    }
    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
    }
    public void SetHealth(int health)
    {
        slider.value = health;

        
    }
    void EndGame()
    {
        SceneManager.LoadScene("GameOver");
        Time.timeScale = 0f; 
    }
}
