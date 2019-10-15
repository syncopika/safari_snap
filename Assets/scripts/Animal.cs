using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// common animal states
public enum AnimalBehavior { Idle, Eating, Sitting, GettingUp, Angry };

public interface Animal {

    AnimalBehavior getState();

}
