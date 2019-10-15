using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// player's camera functionality (the camera that the player will use)
// this script controls the zoom-in ability of the player's camera when activated via right-mouse button click
// utilizes the scene camera though also
// it's attached to the main camera

public class CameraScript : MonoBehaviour {

    public Image scopeImage;
    public GameObject player;
    public GameObject GameManager;
    public Camera cam;
    public Canvas ui;
    private bool scopeOn;
    private float initialFOVThirdPerson; // this is the initial field of view of the scene camera. keep track of the initial value from when the game started
                                         // this is important for keeping the 3rd person view consistent when getting out of 1st person.
    private float initialFOVFirstPerson; // same as above, but when in first person

    // for taking a screenshot 
    private WaitForEndOfFrame waitForEnd;

    // turn off scope view if player gets out of first person view 
    public void turnOffScopeView()
    {
        scopeOn = false;
        scopeImage.enabled = false;
    }

    public bool isScopeOn()
    {
        return scopeOn;
    }

    // shoot a ray from the camera to hit a target object (namely the animals that will be photographed)
    void shootRay()
    {
        RaycastHit hit;
        Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(r, out hit))
        {
            if (hit.collider != null)
            {
                //Debug.Log("ray hit: " + hit.transform);

                // capture animals
                if (hit.transform.gameObject.tag == "animal")
                {
                    //Debug.Log(hit.transform);

                    // get distance 
                    // make sure to take into account FOV! (i.e. if camera is zoomed in, take that perceived distance into account)
                    // default FOV starts at 50, so take fraction of 50
                    Vector3 diff = hit.transform.position - player.transform.position;
                    float distance = diff.magnitude;
                    distance *= (Camera.main.fieldOfView / 50);

                    //Debug.Log("distance to player: " + distance);

                    // find out where object is located in viewport (2d) 
                    Vector3 viewportPos = cam.WorldToViewportPoint(hit.transform.position);
                    //Debug.Log("position of object in viewport: " + viewportPos);

                    // get the orientation of the animal relative to the camera
                    // i.e., is it looking away, towards? use dot product for this
                    Vector3 targetForward = hit.transform.forward;
                    Vector3 cameraForward = cam.transform.forward;
                    //Debug.Log(cameraForward);
                    float product = Vector3.Dot(targetForward, cameraForward);
                    //Debug.Log(product);

                    // what is the animal doing? 
                    // call getState from its interface implementation
                    AnimalBehavior animalState = hit.transform.gameObject.GetComponent<Animal>().getState();
                    //Debug.Log(animalState);

                    IEnumerator coroutine = getSnapshot(hit.transform.name, distance, viewportPos, targetForward, cameraForward, animalState);
                    StartCoroutine(coroutine);
                }
            }
        }
    }


    // taking a snapshot 
    IEnumerator getSnapshot(string targetName, float distance, Vector3 viewportPos, Vector3 cameraForward, Vector3 targetForward, AnimalBehavior state)
    {
        // hide the canvas ui temporarily - this script is attached to the canvas
        ui.enabled = false;

        yield return waitForEnd;

        Texture2D tex = new Texture2D(Screen.width, Screen.height, TextureFormat.ARGB32, false);
        tex.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0, false); // capture screen
        tex.Apply(); // update texture

        // add snapshot 
        Snapshot newPic = new Snapshot(targetName, distance, viewportPos, cameraForward, targetForward, tex, state);
        GameManager.GetComponent<GameManager>().updatePicturesTaken(newPic);
        //picturesTaken.Add(newPic);

        // save pic as test to disk 
        newPic.saveScreenshot();

        // restore ui 
        ui.enabled = true;
    }


	// Use this for initialization
	void Start () {
        scopeOn = false;
        scopeImage.enabled = false;
        initialFOVThirdPerson = cam.fieldOfView;
        waitForEnd = new WaitForEndOfFrame();
    }
	
	// Update is called once per frame
	void Update () {

        if (!player.GetComponent<PlayerControllerBall>().isInFirstPerson())
        {
            turnOffScopeView();
            // set camera's field of view back to initial 
            cam.fieldOfView = initialFOVThirdPerson;
        }

        // player's camera snapshot on left-mouse button down
        if (Input.GetMouseButtonDown(0) && scopeOn)
        {
            shootRay();
        }

        // activate/deactivate the player's camera
        if (Input.GetMouseButtonDown(1) && player.GetComponent<PlayerControllerBall>().isInFirstPerson())
        {
            if (scopeOn)
            {
                turnOffScopeView(); // put camera away
                // go back to initial first-person view
                cam.fieldOfView = initialFOVFirstPerson;
            }
            else
            {
                // put on scope
                scopeImage.enabled = true;
                scopeOn = true;
                // keep track of initial fov 
                initialFOVFirstPerson = cam.fieldOfView;
            }
        }

        // allow zoom-in of player's camera when activated
        if (scopeOn)
        {
            float fieldOfView = cam.fieldOfView;
            fieldOfView += Input.GetAxis("Mouse ScrollWheel") * 10f;
            fieldOfView = Mathf.Clamp(fieldOfView, -10f, 50f); // clamp the fov

            cam.fieldOfView = fieldOfView;
            //Debug.Log("field of view value: " + fieldOfView);
        }
    }
}
