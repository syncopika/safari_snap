using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdController : MonoBehaviour, Animal {

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
    void Update()
    {

        if (Time.time - lastTime > 8f)
        {
            lastTime = Time.time;

            // do a new (or the same) action every 8 sec
            float randomNum = Random.Range(-10, 10);


            if (currState == AnimalBehavior.Idle && randomNum < 6)
            {
                // switch to eating 
                anim.SetBool("isIdle", false);
                anim.SetBool("isEating", true);
                currState = AnimalBehavior.Eating;
            }
            else if (currState == AnimalBehavior.Eating && randomNum > 8)
            {
                // switch to idle
                anim.SetBool("isEating", false);
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
