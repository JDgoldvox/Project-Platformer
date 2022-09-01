using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private float totalHealth = 3;
    [SerializeField] private float currentHealth;

    void Start()
    {
        currentHealth = totalHealth; //initiates health for enemy
    }

    public void TakeDamage(float damage){
        
        currentHealth -= damage;

        if(currentHealth <= 0){ //enemy dies
            Destroy(transform.parent.gameObject);
        }
        
    }
}
