using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class fireBullet : MonoBehaviour
{

    public float timeBetweenBullets = 0.15f;
    public GameObject projectile;

    //bullet info
    public Slider playerAmmoSlider;
    public int maxRounds;
    public int startingRounds;
    int remainingRounds;

    float nextBullet;

    [Header("Audio Settings")]
    public AudioSource audioSource;  // The single AudioSource attached to the player
    public AudioClip shootClip;      // Shooting sound clip

    public PlayerMovement myPlayer;

    // Use this for initialization
    void Awake()
    {
        nextBullet = 0f;
        remainingRounds = startingRounds;
        playerAmmoSlider.maxValue = maxRounds;
        playerAmmoSlider.value = remainingRounds;

    }

    // Update is called once per frame
    void Update()
    {
        if (myPlayer.movementType == PlayerMovement.MovementType.Keyboard)
        {
            if (Input.GetAxisRaw("Fire1") > 0 && nextBullet < Time.time && remainingRounds > 0)
            {

                PlayShootSound();

                nextBullet = Time.time + timeBetweenBullets;
                Vector3 rot;

                if (myPlayer.is3DView == false)
                {
                    if (myPlayer.GetFacing() == -1f)
                    {
                        rot = new Vector3(0, -90, 0);
                    }
                    else rot = new Vector3(0, 90, 0);
                    Instantiate(projectile, transform.position, Quaternion.Euler(rot));

                    remainingRounds -= 1;
                    playerAmmoSlider.value = remainingRounds;
                }

                if (myPlayer.is3DView == true)
                    Instantiate(projectile, transform.position, transform.rotation);

                remainingRounds -= 1;
                playerAmmoSlider.value = remainingRounds;

            }
        }
        if (myPlayer.movementType == PlayerMovement.MovementType.Controller)
        {
            if (Input.GetAxisRaw("ShootGamepad") > 0 && nextBullet < Time.time && remainingRounds > 0)
            {

                PlayShootSound();

                nextBullet = Time.time + timeBetweenBullets;
                Vector3 rot;

                if (myPlayer.is3DView == false)
                {
                    if (myPlayer.GetFacing() == -1f)
                    {
                        rot = new Vector3(0, -90, 0);
                    }
                    else rot = new Vector3(0, 90, 0);
                    Instantiate(projectile, transform.position, Quaternion.Euler(rot));

                    remainingRounds -= 1;
                    playerAmmoSlider.value = remainingRounds;
                }

                if (myPlayer.is3DView == true)
                    Instantiate(projectile, transform.position, transform.rotation);

                remainingRounds -= 1;
                playerAmmoSlider.value = remainingRounds;

            }
        }



    }

    private void PlayShootSound()
    {
        if (shootClip != null)
        {
            audioSource.PlayOneShot(shootClip);
        }
    }

    public void reload (int ammo)
    {
        remainingRounds += ammo;
        playerAmmoSlider.value = remainingRounds;
    }

}
