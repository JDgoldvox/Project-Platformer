using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    private Animator anim;
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip clip;

    void Awake(){
        anim = gameObject.GetComponent<Animator>();
    }


    void OnTriggerEnter2D(Collider2D collider){
        if(collider.CompareTag("Player")){
            anim.SetBool("CheckPointReached", true);
            source.PlayOneShot(clip);
        }
    }
}
