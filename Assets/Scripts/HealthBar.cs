using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public int maxHealths = 100;

    private void Start()
    {
        SetMaxHealth(maxHealths);  //初始生命值
    }
    private void Update()
    {
        if (slider.value == 0)  //滑條值==0
        {
            EndGame();  //結束遊戲邏輯
        }
    }
    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
    }
    public void beenAttacked(int damage)//被攻擊
    {
        slider.value -= damage;
    }
    void EndGame()
    {
        SceneManager.LoadScene("GameOver");
        Time.timeScale = 0f; 
    }
}
