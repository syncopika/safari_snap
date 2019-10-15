using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// https://answers.unity.com/questions/457184/how-to-detect-all-gameobjectsie-enemies-in-field-o.html
// https://www.youtube.com/watch?v=XkAkdF5ZVPs
// https://www.youtube.com/watch?v=BUL1qDo5NYk
// https://answers.unity.com/questions/25118/how-do-you-zoom-with-a-sniper.html
// https://answers.unity.com/questions/649079/how-to-get-a-screenshot-in-game.html
// https://answers.unity.com/questions/13130/scene-to-scene-variables.html  // useful for capturing screenshots and viewing them in new scene


public class PlayerController : MonoBehaviour
{

    private Rigidbody rb;
    private bool inFirstPerson;
    private Animator anim;

    // set true if player is in first person
    public void setFirstPersonState(bool b)
    {
        inFirstPerson = b;
    }

    public bool isInFirstPerson()
    {
        return inFirstPerson;
    }


    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        //float horizontal = Input.GetAxis("Horizontal");
        //float vertical = Input.GetAxis("Vertical");

        //transform.Rotate(Vector3.forward, 1.5f * Time.deltaTime);
        //rb.AddForce(new Vector3(horizontal, 0.0f, vertical) * 1.5f);
        //Vector3 move = new Vector3(horizontal, 0, 0);

        if (Input.GetKeyDown("w"))
        {
            //anim.SetBool("isWalking", true);
            anim.SetTrigger("isWalking");
            anim.SetTrigger("isIdle"); // go back to idle
            //ransform.position += (Vector3.forward * Time.deltaTime * 2f);
        }
        else
        {
            //anim.SetBool("isWalking", false);
        }
        

        

        if (Input.GetKeyDown(KeyCode.Space))
        {

            //rb.AddForce(new Vector3(0.0f, 2.5f, 0.0f) * 2.0f, ForceMode.VelocityChange);
        }

    }
}
