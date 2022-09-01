using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{   
    public Transform firstCheckPoint;
    private Transform currentCheckpoint;
    private Health playerHealth;

    void Start()
    {
        currentCheckpoint = firstCheckPoint;
        playerHealth = GetComponent<Health>();
    }

    public void Respawn(){
        transform.position = currentCheckpoint.position; //set position player to respawn location
        playerHealth.Respawn(); 
    }

    private void OnTriggerEnter2D(Collider2D collider){

        if(collider.transform.tag == "CheckPoint"){
            currentCheckpoint = collider.transform; //set position of checkpoint reached to the current teleport position for respawn
            collider.GetComponent<Collider2D>().enabled = false; //disable checkpoint collider
        }
    }
}
