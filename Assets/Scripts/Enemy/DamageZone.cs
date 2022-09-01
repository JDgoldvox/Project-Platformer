using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageZone : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField] private float damage = 1;
    [SerializeField] private float damageTime = 5;
    private float damageCounter;
    private bool doDamage;
    private bool startTimer;

    void Start(){
        damageCounter = -1; //Do damage instantly the first time player enters trigger zone
    }
    void Update(){
        if(startTimer == true){ //initiate the timer when player outside the damage zone
            StartCounter();
        }
    }

    void OnTriggerStay2D(Collider2D collider){ //If inside damage zone
        if(collider.CompareTag("Player")){
            startTimer = true; //start timer for when next time can be damged

            if(doDamage == true){ //if the damage bool is true, take damage
                collider.GetComponent<Health>().TakeDamage(damage); //dealing "X(damage)" damage here.
                doDamage = false;
            }
        }

        

    }

    void StartCounter(){

        if(damageCounter < 0f){ //if timer is adequate
            damageCounter = damageTime; 
            doDamage = true; //allow damage
            
        }
        else{
            damageCounter -= Time.deltaTime;
        }
    }
}
