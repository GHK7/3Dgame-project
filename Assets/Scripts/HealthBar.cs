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
        SetMaxHealth(maxHealths);  //��l�ͩR��
    }
    private void Update()
    {
        if (slider.value == 0)  //�Ʊ���==0
        {
            EndGame();  //����C���޿�
        }
    }
    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
    }
    public void beenAttacked(int damage)//�Q����
    {
        slider.value -= damage;
    }
    void EndGame()
    {
        SceneManager.LoadScene("GameOver");
        Time.timeScale = 0f; 
    }
}
