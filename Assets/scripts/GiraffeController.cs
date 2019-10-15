using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiraffeController : MonoBehaviour, Animal {

    private Animator anim;
    private AnimalBehavior currState;
    private float lastTime; // store last seen time to get elapsed time

    // Use this for initialization
    void Start () {
        anim = GetComponent<Animator>();
        anim.SetBool("isIdle", true);
        currState = AnimalBehavior.Idle;
        lastTime = Time.time;
    }
	
	// Update is called once per frame
	void Update () {

        //Debug.Log(anim.GetCurrentAnimatorStateInfo(0).IsName("Armature|Idle"));

        if (Time.time - lastTime > 7f)
        {
            lastTime = Time.time;

            // do a new (or the same) action every 7 sec
            float randomNum = Random.Range(-10, 10);


            if (currState == AnimalBehavior.Idle && randomNum > 2 && randomNum < 4)
            {
                // switch to eating 
                anim.SetBool("isIdle", false);
                anim.SetBool("isEating", true);
                currState = AnimalBehavior.Eating;
            }
            else if (currState == AnimalBehavior.Eating && randomNum > 4)
            {
                // switch to idle
                anim.SetBool("isEating", false);
                anim.SetBool("isIdle", true);
                currState = AnimalBehavior.Idle;
            }
            else if (currState == AnimalBehavior.Idle && randomNum > 6)
            {
                // switch to sitting if idle
                anim.SetBool("isIdle", false);
                anim.SetBool("isSitting", true);
                currState = AnimalBehavior.Sitting;
            }
            else if (currState == AnimalBehavior.Sitting && randomNum < 2)
            {
                // switch to getting up
                anim.SetBool("isSitting", false);
                anim.SetBool("isGettingUp", true);
                currState = AnimalBehavior.GettingUp;
            }else if(currState == AnimalBehavior.GettingUp)
            {
                // go to idle from getting up
                anim.SetBool("isGettingUp", false);
                anim.SetBool("isIdle", true);
                currState = AnimalBehavior.Idle;
            }
        }
    }


    public AnimalBehavior getState()
    {
        return currState;
    }
}
