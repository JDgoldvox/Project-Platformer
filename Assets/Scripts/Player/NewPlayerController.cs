using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPlayerController : MonoBehaviour
{   
    [Header("General")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private TrailRenderer trail;

    [Header("Audio")]
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip dash;
    [SerializeField] private AudioClip jump;

    [Header("X axis")]
    private float speed = 8f;
    private float horizontal;
    private bool facingRight = true;

    [Header("Y Axis")]
    private float jumpPower = 18f;
    private bool canJump = true;

    private bool canDoubleJump = false;
    private bool hasDoubleJumped = false;

    [Header("Koyote time")]
    private float koyoteJumpCounter;
    private float koyoteJumpTime = 0.2f;

    [Header("Jump Buffer")]
    private float jumpBufferCounter;
    private float jumpBufferTime = 0.2f;

    [Header("Dash")]
    private bool canDash = true;
    private bool isDashing;
    private float dashingPower = 12f;
    private float dashingTime = 0.1f;
    private float dashingCooldown = 1f;

    [Header("Wall Jump")]
    private float wallJumpTime = 0.1f;
    private float wallJumpCounter;
    private float wallSlideSpeed = 0.3f;
    private float wallDistance = 1f;
    private bool isWallSliding = false;
    public RaycastHit2D rayCheckWallHit; //returns a bool
    private bool hitWall;
    [SerializeField] LayerMask mask;
    private float jumpBackSpeed = 15f;
    private bool hasWallJumped = false;
    float disableMovementTime = 0.4f;
    float wallJumpPower = 20f;
    private bool canMove = true;

    [Header("Animation")]
    private Animator anim;

    private void Awake(){
        anim = GetComponent<Animator>();
    }

    private void Start(){
        DontDestroyOnLoad(this.gameObject);
    }
    void FixedUpdate()
    {   
        if (!canMove)
        {
            return;
        }

        if(isDashing){ //dash takes priority over other movements
            return; //return needs to be within update.
        }
        
        HorizontalMovement();

        if(isWallSliding){
            return;
        }
  
    }

    void Update(){

        Animation();
        if (!canMove)
        {
            return;
        }

        if(isDashing){ //dash takes priority over other movements
            return; //return needs to be within update.
        }

        HorizontalInput();
        WallJump();
       // HorizontalInput();

        if(isWallSliding){
            return;
        }

        if(isWallSliding == true){
            //dont jump
        }
        else{
            Jump();
        }
        
        Grounded();
        KoyoteTimer();
        JumpBufferTimer();
        Dash();

        
    }

    private void HorizontalMovement(){

        //controls the X movement. //no if statement
        rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
        flip();

        void flip(){
            if(facingRight && horizontal < 0f || !facingRight && horizontal > 0f){
                Vector3 localScale = transform.localScale; //makes vector3
                facingRight = !facingRight; //set facing right to opposite
                localScale.x *= -1; //converts x in the vector3 to opposite
                transform.localScale = localScale; //scales it
            }
        }
    }

    private void HorizontalInput(){
        horizontal = Input.GetAxisRaw("Horizontal");
    }

    private void Jump(){

        //player cannot jump when koyote timer is finished.
        ResetJump();

        if(jumpBufferCounter > 0f){ //accounts for jump button
            if(koyoteJumpCounter > 0f || canJump || canDoubleJump){

                anim.SetTrigger("Jump");
                source.PlayOneShot(jump);

                rb.velocity = new Vector2(rb.velocity.x,jumpPower);

                
                if(canDoubleJump == true){ //if you can have doubled jump, give the info
                    hasDoubleJumped = true;
                
                }

                canDoubleJump = !canDoubleJump; //set double jump to be enabled next jump

                jumpBufferCounter = 0;

                

            }   
        }

        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0) //decrease the jump if we aren't holding it
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
            koyoteJumpCounter = 0;
        }   

        void ResetJump(){
            
            if(Grounded() && !Input.GetButton("Jump")){
                canJump = true;
                canDoubleJump = false;
                hasDoubleJumped = false;
            }
            else{
                canJump = false;

                if(koyoteJumpCounter < 0f && hasDoubleJumped == false){ //if in air, enable double jump if we haven't already used it.
                    canDoubleJump = true;
                }
            }
            
        }
    }

    private bool Grounded(){
        if(Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer)){

            return true;
        }
        else{
            return false;
        }
    }
    
    private void KoyoteTimer(){
        if(Grounded()){
            koyoteJumpCounter = koyoteJumpTime;
        }
        else{
            koyoteJumpCounter -= Time.deltaTime;
        }
    }

    private void JumpBufferTimer(){ 
        if(Input.GetButtonDown("Jump")){
            jumpBufferCounter = jumpBufferTime;
        }
        else{
            jumpBufferCounter -= Time.deltaTime;
        }
    }

    private IEnumerator DashNumerator(){
        canDash = false;
        isDashing = true;
        source.PlayOneShot(dash);
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

    private void Dash(){
        if(Input.GetKeyDown(KeyCode.LeftShift) && canDash){
            StartCoroutine(DashNumerator());
        }
    }

     private void WallJump(){

        //ray cast into wall
        if(facingRight){
            rayCheckWallHit = Physics2D.Raycast(transform.position, new Vector2(wallDistance, 0f), wallDistance, mask);
            Debug.DrawRay(transform.position, new Vector2(wallDistance, 0f),Color.red);
            //Debug.Log("Ray Hit: " + rayCheckWallHit.transform.name);

        } 
        else{
            rayCheckWallHit = Physics2D.Raycast(transform.position, new Vector2(-wallDistance, 0f), wallDistance, mask);
            Debug.DrawRay(transform.position, new Vector2(-wallDistance, 0f),Color.red);
           //Debug.Log("Ray Hit: " + rayCheckWallHit.transform.name);

        }

        //Setting isWallSliding to true or false
        if(rayCheckWallHit && !Grounded() && horizontal != 0f && rb.velocity.y < 0f){ // rb.velocity.y < 0f makes it so, the player can jump if facing the wrong direction and raycast doesnt need size to hit wall
            isWallSliding = true;
            wallJumpCounter = wallJumpTime; 
        }
        else{
            wallJumpCounter -= Time.deltaTime;
            
            if(wallJumpCounter <= 0f){
                isWallSliding = false;
            }
        }

        //wall jumping
        if(isWallSliding && Input.GetButtonDown("Jump")){
            rb.velocity = new Vector2(-horizontal * jumpBackSpeed, wallJumpPower);
            source.PlayOneShot(jump);
            hasWallJumped = true;
            StartCoroutine(DisablePlayerMovement()); //makes sure that the player cannot move for some time after jumping
        }
        
        if(isWallSliding){
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, wallSlideSpeed, float.MaxValue));
        }
    }

    private IEnumerator DisablePlayerMovement()
    {
        canMove = false;
        yield return new WaitForSeconds(disableMovementTime);
        canMove = true;
    }

    private void Animation(){
        anim.SetBool("Run", horizontal != 0);
        anim.SetBool("Grounded", Grounded());

    }
    
}
