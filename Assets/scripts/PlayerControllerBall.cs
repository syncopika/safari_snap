using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerControllerBall : MonoBehaviour
{

    private Rigidbody rb;
    private bool inFirstPerson;
    private bool isFrozen;

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
        isFrozen = false;
        rb = GetComponent<Rigidbody>();
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        rb.AddTorque(new Vector3(vertical, 0.0f, -horizontal) * 1.2f);

        if (Input.GetKeyDown(KeyCode.Space) && transform.position.y < 3)
        {
            rb.AddForce(new Vector3(0.0f, 1f, 0.0f) * 3.0f, ForceMode.VelocityChange);
        }

        // allow player to anchor themself
        if (Input.GetKeyDown("x"))
        {
            // freeze position 
            if (!isFrozen)
            {
                rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
                isFrozen = true;
            } else
            {
                isFrozen = false;
                rb.constraints = RigidbodyConstraints.None;
            }
        }

    }


}
