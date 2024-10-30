using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public CharacterController controller;
    public Transform cam;

    public float dashSpeed = 20f; // �Ĩ�t��
    public float dashDuration = 0.2f; // �Ĩ����ɶ�
    public float dashCooldown = 2f; // �Ĩ�N�o�ɶ�

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    public float speed = 6f;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    Vector3 velocity;
    bool isGrounded;

    private bool isDashing = false;
    private float dashCooldownTimer = 0f;

    // Update is called once per frame
    void Update()
    {
        // �ˬd�O�_�ۦa
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

        // ����b�a���W�æV�U���ʮɭ��]Y��V�t��
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;  // �קK���O����֥[
        }

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        // ���D
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        if (Input.GetKeyDown(KeyCode.F) && dashCooldownTimer <= 0 && move.magnitude > 0)
        {
            StartCoroutine(Dash());
        }

        // ���O�v�T
        velocity.y += gravity * Time.deltaTime;

        // ���V���s�ɰ�������t��
        if (direction.magnitude < 0.1f)
        {
            velocity.x = 0f;
            velocity.z = 0f;
        }
        controller.Move(velocity * Time.deltaTime);

        // �����ʿ�J�ɶi�����M����
        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }
    }
    private IEnumerator Dash()
    {
        isDashing = true;
        dashCooldownTimer = dashCooldown;
        yield return new WaitForSeconds(dashDuration);
        isDashing = false;
    }
}
