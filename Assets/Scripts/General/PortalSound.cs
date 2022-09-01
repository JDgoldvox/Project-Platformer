using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalSound : MonoBehaviour
{
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip woosh;

    void OnTriggerEnter2D(Collider2D collider){
        
        if(collider.CompareTag("Portal")){
            source.PlayOneShot(woosh);
        }
        
    }
}
