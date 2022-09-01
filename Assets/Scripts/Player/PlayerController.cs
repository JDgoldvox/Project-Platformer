using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("General")]
    private float horizontal;   
    private float speed = 8f;
    private float jumpingPower = 16f;
    private bool isFacingRight = true;
    private bool canDoubleJump;
    private bool hasDoubleJumped;

    [Header("Coyote Time")]
    private float coyoteTime = 0.2f;
    private float coyoteTimeCounter;

    [Header("Jump Buffer")]
    private float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;

    [Header("Dash")]
    private bool canDash = true;
    private bool isDashing;
    private float dashingPower = 24f;
    private float dashingTime = 0.2f;
    private float dashingCooldown = 1f;

    [Header("Wall Jump")]
    private bool canMove;
    private float wallSlidingSpeed = 2f;
    private float wallJumpingTime = 0.2f;
    private float wallJumpingCounter;
    private float wallJumpingDirection;
    private Vector2 wallJumpingPower = new Vector2(8f, 16f);
    [SerializeField] private float wallDistance = 0.75f;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private TrailRenderer trail;

    private void Update()
    {
        if (canMove)
        {
            return;
        }

        if (isDashing) //dash takes priority over other movements
        {
            return;
        }

        HorizontalInput();
        CoyoteTimer(); //Time allowed to fly and jump
        JumpBufferTimer(); //Time allowed to auto jump after hitting ground
        DoubleJumpReset();
        Jump();
        Flip();
        TriggerDash();
        WallSlide();
        WallJump();

        Debug.DrawRay(transform.position, transform.localScale.x * Vector2.right, Color.red);
    }

    private void FixedUpdate()
    {
        if (canMove)
        {
            return;
        }

        if (isDashing) //dash takes priority over other movements
        {
            return;
        }
        
        rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private void Flip()
    {
        if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            Vector3 localScale = transform.localScale;
            isFacingRight = !isFacingRight;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }
    
    private void CoyoteTimer()
    {
        if (IsGrounded())
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }
    }

    private void JumpBufferTimer()
    {
        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }
    }

    private void HorizontalInput()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
    }

    private void DoubleJumpReset()
    {

        if(IsGrounded() && !Input.GetButton("Jump")){
                canDoubleJump = false;
                hasDoubleJumped = false;
            }
            else{
                if(coyoteTimeCounter < 0f && hasDoubleJumped == false){ //if in air, enable double jump if we haven't already used it.
                    canDoubleJump = true;
                }
            }
    }
    
    private void Jump()
    {
        if(jumpBufferCounter > 0f){ //accounts for jump button
            if(coyoteTimeCounter > 0f || canDoubleJump){
                rb.velocity = new Vector2(rb.velocity.x, jumpingPower);

                
                if(canDoubleJump == true){ //if you can have doubled jump, give the info
                    hasDoubleJumped = true;
                
                }

                canDoubleJump = !canDoubleJump; //set double jump to be enabled next jump

                jumpBufferCounter = 0;

            }   
        }

        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0) //auto jump if we press jump early
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);

            coyoteTimeCounter = 0f;
        }
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale; //store original gravity becuz dash doesnt use gravity
        rb.gravityScale = 0f;
        rb.velocity = new Vector2(transform.localScale.x * dashingPower, 0f); //transform.localScale.x is the direction
        trail.emitting = true;
        yield return new WaitForSeconds(dashingTime);
        trail.emitting = false;
        rb.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown); //does cooldown for dash
        canDash = true;
    }

    private void TriggerDash()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }
    }

    private bool IsWalled() //checks if player is in range of a wall
    {
        return Physics2D.Raycast(transform.position, transform.localScale.x * Vector2.right, wallDistance, groundLayer);
    }

    private void WallSlide() //wall jump if sliding... so we jump back!
    {
        if (IsWalled() && !IsGrounded() && horizontal != 0f && rb.velocity.y < 0f) //checks if the player is actually on teh wall
        {
            wallJumpingCounter = wallJumpingTime; 
            wallJumpingDirection = -transform.localScale.x; //sets jumping direction oppoosite of the way we are facing when attached to the wall
            rb.velocity = new Vector2(rb.velocity.x, -wallSlidingSpeed); //sliding down the wall speed!
        }
        else
        {
            wallJumpingCounter -= Time.deltaTime;
        }
    }

    private void WallJump()
    {
        if (Input.GetButtonDown("Jump") && wallJumpingCounter > 0f) //when we jump off a wall
        {
            wallJumpingCounter = 0f;
            rb.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y); //jumping!
            StartCoroutine(DisablePlayerMovement()); //makes sure that the player cannot move for some time after jumping
        }
    }

    private IEnumerator DisablePlayerMovement()
    {
        canMove = true;
        yield return new WaitForSeconds(0.4f);
        canMove = false;
    }
}