using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{

    public GameObject flipModel;

    public float detectionTime;
    float startRun;
    bool firstDetection;

    //movement options
    public float runSpeed;
    public float walkSpeed;
    public bool facingRight = true;

    float moveSpeed;
    bool running;

    Rigidbody myRB;
    Animator myAnim;
    Transform detectedPlayer;

    bool Detected;

    // Start is called before the first frame update
    void Start()
    {
        myRB = GetComponentInParent<Rigidbody>();
        myAnim = GetComponentInParent<Animator>();

        running = false;
        Detected = false;
        firstDetection = false;
        moveSpeed = walkSpeed;

        if (Random.Range(0, 10) > 5) Flip();
    }

    
    void FixedUpdate()
    {
        if (Detected)
        {
            if (detectedPlayer.position.x < transform.position.x && facingRight)
            {
                Flip();
            }
            else if (detectedPlayer.position.x > transform.position.x && !facingRight)
            {
                Flip();
            }

            if(!firstDetection)
            {
                startRun = Time.time + detectionTime;
                firstDetection=true;
            }
        }
        if (Detected && !facingRight) 
        {
            myRB.velocity = new Vector3((moveSpeed * -1), myRB.velocity.y, 0);
        }else if (Detected && facingRight)
        {
            myRB.velocity = new Vector3((moveSpeed), myRB.velocity.y, 0);
        }
        if(!running && Detected)
        {
            if (startRun < Time.time)
            {
                moveSpeed = runSpeed;
                myAnim.SetTrigger("run");
                running = true;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player" && !Detected) 
        {
            Detected = true;
            detectedPlayer = other.transform;
            myAnim.SetBool("detected", Detected);
            if (detectedPlayer.position.x < transform.position.x && facingRight) 
            {
                Flip();
            }else if (detectedPlayer.position.x > transform.position.x && !facingRight)
            {
                Flip();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            firstDetection = false;
            if(running)
            {
                myAnim.SetTrigger("run");
                moveSpeed = walkSpeed;
                running = false;
            }
        }
    }

    void Flip()
    {
        facingRight = !facingRight;

        //flipModel.transform.Rotate(0f, 180f, 0f);

        Vector3 theScale = flipModel.transform.localScale;
        theScale.z *= -1;
        flipModel.transform.localScale = theScale;
    }
}
