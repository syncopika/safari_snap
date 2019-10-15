using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElephantController : MonoBehaviour, Animal {

    private Animator anim;
    private AnimalBehavior currState;
    private bool isNotAngry;
    private float lastTime; // store last seen time to get elapsed time

    // Use this for initialization
    void Start()
    {
        anim = GetComponent<Animator>();
        anim.SetBool("isIdle", true);
        currState = AnimalBehavior.Idle;
        isNotAngry = true;
        lastTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {

        if (Time.time - lastTime > 6f && isNotAngry)
        {
            lastTime = Time.time;

            // do a new (or the same) action every 6 sec
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
            }
            else if (currState == AnimalBehavior.GettingUp)
            {
                // go to idle from getting up
                anim.SetBool("isGettingUp", false);
                anim.SetBool("isIdle", true);
                currState = AnimalBehavior.Idle;
            }
        }
    }

    // from Animal interface
    public AnimalBehavior getState()
    {
        return currState;
    }

    // get the state as a string 
    private string getStateString(AnimalBehavior a)
    {
        if(a == AnimalBehavior.Idle)
        {
            return "isIdle";
        }else if(a == AnimalBehavior.Eating)
        {
            return "isEating";
        }else if(a == AnimalBehavior.Sitting)
        {
            return "isSitting";
        }else if(a == AnimalBehavior.GettingUp)
        {
            return "isGettingUp";
        }else
        {
            return "isAngry";
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.name);
        if (other.name != "Player")
        {
            return;
        }
        else
        {
            isNotAngry = true;
            anim.SetBool(getStateString(currState), false);
            anim.SetBool("isAngry", true);
            //Debug.Log("i am angry");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // this is not really a good thing to do because the elephant may still be doing the angry animation when you leave the trigger!
        // but I do need to reset state so it can go back to idling 
        anim.SetBool("isAngry", false);
        isNotAngry = true;
        anim.SetBool("isIdle", true);
        //Debug.Log("i am no longer angry");
    }

}
