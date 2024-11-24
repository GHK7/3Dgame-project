using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bush : MonoBehaviour
{
    private float originalSpeed; // 記錄原始速度

    PlayerMovement playerMovement;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            // 取得玩家的 PlayerMovement 元件
            playerMovement = other.GetComponent<PlayerMovement>();

            playerMovement.speed = 10f;

            if (playerMovement.isDashing == true)
            {
                playerMovement.dashSpeed = 20f;

            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            playerMovement.speed = 40f;

            if (playerMovement.isDashing == true)
            {
                playerMovement.dashSpeed = 60f;

            }
        }
    }
}
