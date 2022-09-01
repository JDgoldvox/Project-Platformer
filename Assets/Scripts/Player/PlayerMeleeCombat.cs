using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMeleeCombat : MonoBehaviour
{
    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private Animator anim;
    [SerializeField] private float attackRange;
    [SerializeField] private float attackDamage = 1;

    [Header("Knockback")]
    [SerializeField] private float thrust = 500f;

    [Header("Combat timer")]
    private float combatCDCounter;
    private float combatCD = 1f;

    [Header("Audio")]
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip slash;

    void Update()
    {

        if(combatCDCounter < 0f && Input.GetMouseButtonDown(0)){

            Attack();
            combatCDCounter = combatCD;
        }
        else{
            combatCDCounter -= Time.deltaTime;
        }
        
    }

    private void Attack(){

        anim.SetTrigger("Swing");
        source.PlayOneShot(slash);
        
        Collider2D[] hitEnemiesCollider = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);

        foreach(Collider2D enemy in hitEnemiesCollider){
            Debug.Log("we hit " + enemy.name);

            if (enemy != null && enemy.GetComponentInParent<Rigidbody2D>() != null)
            {
                Rigidbody2D enemyRB = enemy.GetComponentInParent<Rigidbody2D>();
                StartCoroutine(KnockCoroutine(enemyRB));
            }
        }
    }

    private IEnumerator KnockCoroutine(Rigidbody2D enemyRB)
    {
        enemyRB.GetComponent<EnemyMovement>().disableMovement = true;

        Vector2 forceDirection = enemyRB.transform.position - transform.position;
        Vector2 force = forceDirection.normalized * thrust;

        enemyRB.velocity += force;
        yield return new WaitForSeconds(.3f);

        enemyRB.velocity += new Vector2(); //no idea what this does
        enemyRB.GetComponent<EnemyMovement>().disableMovement = false;
        enemyRB.GetComponentInChildren<EnemyHealth>().TakeDamage(attackDamage);
    }

    void OnDrawGizmosSelected(){
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
