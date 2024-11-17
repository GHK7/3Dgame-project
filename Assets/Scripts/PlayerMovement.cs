using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public Transform cam;

    public float dashSpeed = 20f; // 衝刺速度
    public float dashDuration = 0.2f; // 衝刺持續時間
    public float dashCooldown = 2f; // 衝刺冷卻時間

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    public float speed = 20f;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    Vector3 velocity;
    bool isGrounded;

    private bool isDashing = false;
    private float dashCooldownTimer = 0f;
    private AudioSource audioSource; // 音效來源
    public AudioClip runSound; // 跑步音效
    private bool isPlayingRunSound = false; // 確保跑步音效不重複播放
    private Animator animator;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        animator = GetComponent<Animator>();  // 获取动画组件
        
    }

    // Update is called once per frame
    void Update()
    {
        // 檢查是否著地
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (dashCooldownTimer > 0)
        {
            dashCooldownTimer -= Time.deltaTime;
        }

        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;
        if (isDashing)
        {
            controller.Move(transform.forward * dashSpeed * Time.deltaTime);
        }
        else
        {
            controller.Move(move * speed * Time.deltaTime);
        }

        

        // 當角色在地面上並向下移動時重設Y方向速度
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;  // 避免重力持續累加
        }

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        // 跳躍
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            //Debug.Log("Jump");
            
        }

        if (Input.GetKeyDown(KeyCode.F) && dashCooldownTimer <= 0 && move.magnitude > 0)
        {
            StartCoroutine(Dash());
        }

        // 重力影響
        velocity.y += gravity * Time.deltaTime;

        // 當方向為零時停止水平速度
        if (direction.magnitude < 0.1f)
        {
            velocity.x = 0f;
            velocity.z = 0f;
        }

        controller.Move(velocity * Time.deltaTime);

        if (isGrounded && move.magnitude > 0 && !isDashing)
        {
            if (!audioSource.isPlaying && !isPlayingRunSound)
            {
                audioSource.clip = runSound;
                audioSource.loop = true;
                audioSource.Play();
                isPlayingRunSound = true;
            }
        }
        else
        {
            if (audioSource.isPlaying && isPlayingRunSound)
            {
                audioSource.Stop();
                isPlayingRunSound = false;
            }
        }

        // 當有移動輸入時進行旋轉和平移
        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
            animator.SetBool("isRunning", true);
            animator.SetBool("isIdle", false);
            
        }
        else
        {
            
            animator.SetBool("isRunning", false);
            animator.SetBool("isIdle", true);
        }
    }
    private IEnumerator Dash()
    {
        isDashing = true;
        dashCooldownTimer = dashCooldown;
        if (audioSource.isPlaying && audioSource.isPlaying)
        {
            audioSource.Stop(); // 停止跑步音效
            isPlayingRunSound = false;
            animator.SetBool("isRunning", false);
        }
        yield return new WaitForSeconds(dashDuration);
        isDashing = false;
    }
}
