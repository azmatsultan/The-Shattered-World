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
        PlayerMovement myPlayer = transform.root.GetComponent<PlayerMovement>();

        if (Input.GetAxisRaw("Fire1") > 0 && nextBullet < Time.time && remainingRounds>0)
        {
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
            
            if(myPlayer.is3DView == true)
            Instantiate(projectile,transform.position, transform.rotation);
            
            remainingRounds -= 1;
            playerAmmoSlider.value = remainingRounds;

        }



    }
}
