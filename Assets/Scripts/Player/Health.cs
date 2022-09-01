using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float startingHealth = 2;
    public float currentHealth{ get; private set; }
    private Animator anim;
    private bool dead;

    [Header("Iframes")]
    [SerializeField] private float iFramesDuration;
    [SerializeField] private float numOfFlashes;
    private SpriteRenderer spriteRend;
    private Material originalMaterial;
    [SerializeField] private Material flashMaterial;

    [Header("Respawn")]
    private PlayerRespawn playerRespawn;
    private NewPlayerController movementScript;

    [Header("Audio")]
    [SerializeField] AudioSource source;
    [SerializeField] AudioClip ouch;
    [SerializeField] AudioClip dies;
    
    
    void Awake()
    {
        anim = GetComponent<Animator>();
        spriteRend = GetComponent<SpriteRenderer>();
        movementScript = GetComponent<NewPlayerController>();
        playerRespawn = GetComponent<PlayerRespawn>();
    }

    void Start(){
        currentHealth = startingHealth;
        originalMaterial = spriteRend.material;
    }

    public void TakeDamage(float damage){
        currentHealth = Mathf.Clamp(currentHealth - damage, 0, startingHealth);

        if(currentHealth > 0){ //player hurt
            
            anim.SetTrigger("Hurt");
            source.PlayOneShot(ouch);
            StartCoroutine(Invulnerability());
        }
        else{ //player dies
            anim.SetTrigger("Die");
            source.PlayOneShot(dies);   
            movementScript.enabled = false;   
            dead = true;  
            GetComponent<Rigidbody2D>().velocity = new Vector2(0,0); //overwites velocity and kills player where it lands

            StartCoroutine(deathTime());
   
        }
    }

    public void AddHealth(float value){
        currentHealth = Mathf.Clamp(currentHealth + value, 0 , startingHealth);
    }

    public void Respawn(){
        AddHealth(startingHealth);
        anim.ResetTrigger("Die");
        anim.Play("Player_Idle");
        StartCoroutine(Invulnerability());
        dead = false;
        movementScript.enabled = true;
    }

    private void Update(){

        if(Input.GetKeyDown(KeyCode.E)){ //TAKE 1 POINT OF DAMAG PRESSING E
            TakeDamage(1);
        }

    }

    private IEnumerator Invulnerability(){
        Physics2D.IgnoreLayerCollision(7,9, true);
        
        for (int i = 0; i < numOfFlashes; i++)
        {
            spriteRend.material = flashMaterial;
            yield return new WaitForSeconds(0.1f);
            spriteRend.material = originalMaterial;
            yield return new WaitForSeconds(0.15f);
        }

        Physics2D.IgnoreLayerCollision(7,9, false);

    }

    private IEnumerator deathTime(){
        
        yield return new WaitForSeconds(1f);
        playerRespawn.Respawn(); 
        
    }

}
