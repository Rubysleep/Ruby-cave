using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class AndyController : MonoBehaviour
{
   [Header("移动参数")]
    public float walkSpeed = 7f;
    public float runSpeed = 9f;
    public float runDelay = 0.4f;

    [Header("跳跃参数")]
    public float jumpForce = 10f;
    public LayerMask groundLayer;
    public float rayLength = 0.15f;

    private Animator animator;
    private Rigidbody2D rb;
    private bool isGrounded;        
    private bool facingRight = true;
    private float runTimer;
    private float moveInput;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        if (transform.localScale.x < 0)
            facingRight = false;
    }

    void Update()
    {
      
        Vector2 rayOrigin = new Vector2(transform.position.x, transform.position.y - 0.5f);
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, rayLength, groundLayer);
        isGrounded = hit.collider != null;

        
        animator.SetBool("IsGrounded", isGrounded);

        
        moveInput = 0f;
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
            moveInput = -1f;
        else if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
            moveInput = 1f;

       
        HandleFlip(moveInput);

       
        float currentSpeed = walkSpeed;
        if (moveInput != 0)
        {
            runTimer += Time.deltaTime;
            if (runTimer >= runDelay)
                currentSpeed = runSpeed;
        }
        else
        {
            runTimer = 0f;
        }

        rb.velocity = new Vector2(moveInput * currentSpeed, rb.velocity.y);

       
        bool shouldRun = isGrounded && Mathf.Abs(moveInput) > 0.01f;
        animator.SetBool("IsRunning", shouldRun);

       
        if ((Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.wKey.wasPressedThisFrame) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            runTimer = 0f;

            animator.SetBool("IsRunning", false);

            animator.SetTrigger("Jump");
            
      
        }
    }

    
    void HandleFlip(float direction)
    {
        if (direction > 0 && !facingRight)
        {
            facingRight = true;
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x);   
            transform.localScale = scale;
        }
        else if (direction < 0 && facingRight)
        {
            facingRight = false;
            Vector3 scale = transform.localScale;
            scale.x = -Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
    }

    void OnDrawGizmosSelected()
    {
        Vector2 rayOrigin = new Vector2(transform.position.x, transform.position.y - 0.5f);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(rayOrigin, rayOrigin + Vector2.down * rayLength);
    }
}
