using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private int nextScene;
    private GameObject player;

    private void Start(){
        player = GameObject.Find("Player");
    }
    void OnTriggerEnter2D(Collider2D collider){
        
        if(collider.CompareTag("Player")){
            player.GetComponent<Rigidbody2D>().velocity = new Vector2(0f,0f);
            player.transform.position = new Vector3(0,0,0);
            SceneManager.LoadScene(nextScene);
            
            
            
        }
        
    }
}


