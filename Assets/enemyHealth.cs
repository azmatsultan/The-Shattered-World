using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class enemyHealth : MonoBehaviour
{

    public float enemyMaxHealth;
    public float damageModifier;
    public GameObject damageParticles;
    public bool drops;
    public GameObject drop;
    public bool canBurn;
    public float burnDamage;
    public float burnTime;
    public GameObject burnEffects;
    public bool isDead;

    bool onFire;
    float nextBurn;
    float burnInterval = 1f;
    float endBurn;

    float currentHealth;

    public Slider enemyHealthIndicator;

    public AudioSource enemyAudioSource;
    public AudioClip deathSound;

    
    
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = enemyMaxHealth;
        enemyHealthIndicator.maxValue = enemyMaxHealth;
        enemyHealthIndicator.value = currentHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (onFire && Time.time > nextBurn)
        {
            addDamage(burnDamage);
            nextBurn += burnInterval;
        }
        if (onFire && Time.time > endBurn)
        {
            onFire = false;
            burnEffects.SetActive(false);
        }
    }

    public void addDamage(float damage)
    {
        enemyHealthIndicator.gameObject.SetActive(true);
        damage = damage * damageModifier;
        if (damage <= 0f) return;
        currentHealth -= damage;
        enemyHealthIndicator.value = currentHealth;
        if (currentHealth <= 0 && !isDead)
        {
            isDead = true;
            makeDead();
        }
    }

    public void damageFX(Vector3 point, Vector3 rotation)
    {
        Instantiate(damageParticles, point, Quaternion.Euler(rotation));
    }

    public void addFire()
    {
        if(!canBurn) return;
        onFire = true;
        burnEffects.SetActive(true);
        endBurn = Time.time+burnTime;
        nextBurn = Time.time+burnInterval;
    }

    void makeDead()
    {
        
        if(drops) Instantiate(drop, transform.position, transform.rotation);
        Instantiate(damageParticles, transform.position, Quaternion.Euler(new Vector3(-90, 0, 0)));

        if (deathSound != null && enemyAudioSource != null)
        {

            // Disable enemy controls and collider
            GetComponent<Collider>().enabled = false;

            // Disable all SkinnedMeshRenderers (instead of MeshRenderers)
            foreach (SkinnedMeshRenderer smr in GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                smr.enabled = false;
            }

            enemyAudioSource.PlayOneShot(deathSound);
            Destroy(gameObject, deathSound.length); // Delay destruction until sound finishes
        }
        else
        {
            Destroy(gameObject); // Fallback in case audio is missing
        }

        //Destroy(gameObject.transform.root.gameObject);
        //Destroy(gameObject);
    }

}
