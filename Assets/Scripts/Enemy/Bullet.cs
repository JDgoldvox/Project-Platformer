using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float speed;
    private float directionX;
    private float directionY;
    private float damage;
    private float timeAlive;
    public float timeAliveCounter;    

    private Shoot shoot;

    void Awake(){
        shoot = gameObject.GetComponentInParent<Shoot>();
    }

    void Start()
    {
        speed = shoot.speedOfBullet;
        directionX = shoot.directionX;
        directionY = shoot.directionY;
        damage = shoot.damage;
        timeAlive = shoot.bulletTimeAlive;
        timeAliveCounter = shoot.bulletTimeAlive;
        gameObject.transform.localScale *= shoot.bulletSize;
    }

    void Update()
    {
        if(timeAliveCounter > 0f){
            transform.position += new Vector3(directionX * speed * Time.deltaTime, directionY * speed * Time.deltaTime, 0);
            timeAliveCounter -= Time.deltaTime;
        }
        else{
            timeAliveCounter = timeAlive;
            gameObject.SetActive(false);
        }   
        


    }

    void OnCollisionEnter2D(Collision2D collision){
        if(collision.gameObject.CompareTag("Player")){

            collision.gameObject.GetComponent<Health>().TakeDamage(damage);
            timeAliveCounter = timeAlive;
            gameObject.SetActive(false);
        }

        if(collision.gameObject.CompareTag("Ground")){
            timeAliveCounter = timeAlive;
            gameObject.SetActive(false);
        }
        
    }

}
