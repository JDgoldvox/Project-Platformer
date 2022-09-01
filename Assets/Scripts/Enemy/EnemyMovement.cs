using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("X move distance")]
    [SerializeField] float timeRunning = 2f;
    private float timeRunningCounter;
    private float distance;

    [Header("Direction")]
    private bool movingLeft = true;
    private float leftEdge;
    private float rightEdge;

    [Header("Speed")]
    [SerializeField] private float speed = 2f;
    [SerializeField] private float speedOfChase = 2f;

    [Header("General")]
    [SerializeField] private GameObject player;
    [SerializeField] private Transform playerTransform;    
    private float distanceTillChase = 20f;
    private Rigidbody2D rb; //This enemy
    public bool disableMovement = false;
    

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>(); 
    }

    void Update()
    {   
        //rb.velocity += new Vector2(5f * Time.deltaTime,0);

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //1. Gather distance of the player and this enemy
        //2. Compare if its greater than X amount

        if(!disableMovement){
            if(Vector2.Distance(playerTransform.position, transform.position) < distanceTillChase){ 

            //3. Set the enemy RB of this velocity to the direction of the player
            float Xdirection = Mathf.Sign(playerTransform.position.x - transform.position.x); //returns -1 or +1 depending on position
            rb.velocity = new Vector2(Xdirection * speedOfChase, rb.velocity.y);
            
        }
        else{ //Moves player left most position and right most position
            Patrol();
        }  
        }
        
    }

    private void Patrol(){

        if(movingLeft == true){
            if(timeRunningCounter > 0f){
                rb.velocity = new Vector2(-speed, rb.velocity.y);
                timeRunningCounter -= Time.deltaTime;
            }
            else{
                timeRunningCounter = timeRunning;
                movingLeft = false;
            }
        }
        else{
            if(timeRunningCounter > 0f){
                rb.velocity = new Vector2(speed, rb.velocity.y);
                timeRunningCounter -= Time.deltaTime;
            }
            else{
                timeRunningCounter = timeRunning;
                movingLeft = true;
            }
        }
    }

}
