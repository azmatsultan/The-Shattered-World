using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class playerHealth : MonoBehaviour
{
    
    public float fullHealth;
    float currentHealth;

    public GameObject playerDeathFX;
    public GameObject GameManager;
    GameManager gameManager;

    public bool isDead;

    //HUD
    public Slider playerHealthSlider;
    public Image damageScreen;
    Color flashColor = new Color(255f, 0f, 0f, 1f);
    float flashSpeed = 5f;
    bool damaged = false;

    [Header("Audio Settings")]
    public AudioSource audioSource;  // The single AudioSource attached to the player
    public AudioClip DamageTakenClip;       // Damage sound clip
    public AudioClip PlayerDeathSound;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = fullHealth;
        playerHealthSlider.maxValue = fullHealth;
        playerHealthSlider.value = currentHealth;

    }

    private void Awake()
    {
        gameManager = GameManager.GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(damaged)
        {
            damageScreen.color = flashColor;
        }else
        {
            damageScreen.color= Color.Lerp(damageScreen.color, Color.clear, flashSpeed*Time.deltaTime);
        }
        damaged = false;
    }

    public void addDamage (float damage)
    {
        audioSource.PlayOneShot(DamageTakenClip);
        currentHealth -= damage;
        playerHealthSlider.value = currentHealth;
        damaged = true;
        if (currentHealth <= 0 && !isDead)
        {
            isDead = true;
            makeDead();
        }
    }

    public void addHealth (float health)
    {
        currentHealth += health;
        if (currentHealth > fullHealth) 
        {
            currentHealth = fullHealth; 
        }
        playerHealthSlider.value = currentHealth;
    }

    public void makeDead()
    {
        if (PlayerDeathSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(PlayerDeathSound);
            Instantiate(playerDeathFX, transform.position, Quaternion.Euler(new Vector3(-90, 0, 0)));
            damageScreen.color = flashColor;

            // Disable player controls and collider
            GetComponent<Collider>().enabled = false;

            // Disable all SkinnedMeshRenderers (instead of MeshRenderers)
            foreach (SkinnedMeshRenderer smr in GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                smr.enabled = false;
            }

            // Wait for the sound to finish before going to the main menu
            StartCoroutine(HandleDeath());
        }
        else
        {
            gameManager.ReturnToMainMenu(); // If no sound, return immediately
            Destroy(gameObject);
        }
    }

    private IEnumerator HandleDeath()
    {
        yield return new WaitForSeconds(1); // Wait for the sound to finish
        gameManager.ReturnToMainMenu(); // Now return to the main menu
        Destroy(gameObject);
    }

}
