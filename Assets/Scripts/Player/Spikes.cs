using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour
{

    void OnTriggerEnter2D(Collider2D collider){
        if(collider.tag == "Spikes"){
            GetComponent<Health>().TakeDamage(1000);
        }
    }

}
