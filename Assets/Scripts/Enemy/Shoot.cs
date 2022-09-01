using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour
{   
    [SerializeField] private float fireRate = 2f;  
    private float fireRateCounter;
    private BulletPool bulletPool;
    
    public float speedOfBullet;
    public float directionX;
    public float directionY;
    public float damage;
    public float bulletTimeAlive;
    public float bulletSize;

    [Header("Sound")]
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip pew;


    void Awake(){
        bulletPool = GetComponent<BulletPool>();
    }   

    void Start(){
        fireRateCounter = 0f;


    }

    void Update()
    {
        FireBullet();
    }

    private void FireBullet(){

        if(fireRateCounter < 0f){
            source.PlayOneShot(pew);
            GameObject bullet = bulletPool.GetPooledObject();

            if(bullet != null){
                bullet.transform.position = transform.position;
                bullet.SetActive(true);
            }

            fireRateCounter = fireRate;
        }
        else{
            fireRateCounter -= Time.deltaTime;
        }
       
    }   
}
