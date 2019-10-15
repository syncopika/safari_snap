using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this script controls the scene camera view. 
// it makes sure the camera is following the player properly
// it is attached to the main camera, along with CameraScript

public class CameraController : MonoBehaviour
{

    public GameObject player;
    //float lastAngle = 0;
    Vector3 offset; // offset from player
    bool inFirstPerson;

    public float sensitivity = 100.0f;
    private float clampY = 25.0f;

    private float rotY = 0.0f;
    private float rotX = 0.0f;

    // this is for storing the initial y rotation when player moves to 1st person
    // needed to clamp properly depending on the y rotation 
    private float initialYRot = 0.0f;

    // Use this for initialization
    void Start()
    {
        offset = player.transform.position - transform.position;
        inFirstPerson = false;

        Vector3 rot = transform.localRotation.eulerAngles;
        rotY = rot.y;
        rotX = rot.x;
    }

    // Update is called once per frame
    void Update()
    {
        //offset = player.transform.position - transform.position;

        // go into first person mode 
        if (Input.GetKeyDown(KeyCode.F1))
        {
            if (!inFirstPerson)
            {
                // first person view
                inFirstPerson = true;
                transform.position = player.transform.position; // put the camera in the same position as the player
                player.GetComponent<PlayerControllerBall>().setFirstPersonState(true);
                initialYRot = transform.localRotation.eulerAngles.y;
            }
            else
            {
                // get out of first person view
                inFirstPerson = false;
                transform.position = player.transform.position - offset;
                transform.LookAt(player.transform);
                transform.Rotate(Vector3.right, -10);
                player.GetComponent<PlayerControllerBall>().setFirstPersonState(false);
            }
        }

        if (inFirstPerson)
        {
            transform.position = player.transform.position; // make sure camera stays with player 

            // make sure x rotation is 0 so it's level (if it's > 0)!
            if(transform.rotation.x > 0)
            {
                transform.rotation = Quaternion.Euler(0, transform.rotation.y, transform.rotation.z);
            }

            if (Input.GetKey("q"))
            {
                // rotate counterclockwise about Y 
                transform.Rotate(-Vector3.up * Time.deltaTime * 300f);

                // get updated local rotation info 
                Vector3 rot = transform.localRotation.eulerAngles;
                rotY = rot.y;
                rotX = rot.x;

                initialYRot = transform.localRotation.eulerAngles.y;
            }
            else if (Input.GetKey("e"))
            {
                // rotate clockwise about Y 
                transform.Rotate(Vector3.up * Time.deltaTime * 300f);

                // get updated local rotation info 
                Vector3 rot = transform.localRotation.eulerAngles;
                rotY = rot.y;
                rotX = rot.x;

                initialYRot = transform.localRotation.eulerAngles.y;
            }

            if (transform.GetComponent<CameraScript>().isScopeOn())
            {
                // allow look around with mouse when scope is on
                float mouseX = Input.GetAxis("Mouse X");
                float mouseY = -Input.GetAxis("Mouse Y"); // negative so no inversion of axis to mouse move

                rotY += mouseX * sensitivity * Time.deltaTime;
                rotX += mouseY * sensitivity * Time.deltaTime;

                // fix rotX if needed 
                // restrict downward view to 15 degrees
                rotX = Mathf.Clamp(rotX, -90.0f, 15.0f);

                // also rotY
                rotY = Mathf.Clamp(rotY, initialYRot-clampY, initialYRot+clampY);

                // use quaternion to get new rotation
                Quaternion localRotation = Quaternion.Euler(rotX, rotY, 0.0f);

                transform.rotation = localRotation;
            }         
        }
        else
        {
            transform.position = player.transform.position - offset;
            transform.LookAt(player.transform);
            transform.Rotate(Vector3.right, -10);
        }

    }

}
