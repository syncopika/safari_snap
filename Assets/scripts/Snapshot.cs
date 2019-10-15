using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class Snapshot {

    // NOT_CENTER is like if the object is centered along x, but not on y (i.e. too high or low)
    public enum ScreenPlacement { LEFT, RIGHT, CENTER, NOT_CENTER }; 

    // split the possible orientation into 3 choices 
    /*
     * the pairs of o's represents some possible orientations of an animal relative to camera.
     * the pairs going left and right will count as AWAY.
     * the pairs to the left and right of the center pair, as well as the center pair, count as TOWARDS.
     * this is based on the result of the dot product between the animal's and camera's forward vectors.
     * COMPLETELY_AWAY will be an orientation with a dot product > 0, since all of these other orientations are <= 0.
     * but a small positive amount (i.e. no more than .5 based on some tests?) is ok (that will just count as away)
     * 
     *   <-oo         oo-> 
     *       o   o   o
     *      o    o    o
     *           |
     *           V
     */
    public enum Orientation { AWAY, TOWARDS, COMPLETELY_AWAY };

    private string targetName;
    private float distanceAway;
    private ScreenPlacement screenPlacement;
    private Orientation orientation;
    private Texture2D theScreenshot;
    private AnimalBehavior animalState;

    public Snapshot(string name, float dist, Vector3 screenPlace, Vector3 cameraForward, Vector3 targetForward, Texture2D screenshot, AnimalBehavior state)
    {
        targetName = name;
        distanceAway = dist;
        screenPlacement = determineScreenPlace(screenPlace);
        orientation = determineOrientation(cameraForward, targetForward);
        theScreenshot = screenshot;
        animalState = state;
    }

    private Orientation determineOrientation(Vector3 a, Vector3 b)
    {
        float product = Vector3.Dot(a, b);
        //Debug.Log(product);
        if (product > 0.8f)
        {
            // target is looking away.
            return Orientation.COMPLETELY_AWAY;
        } else if (product <= 0.8f && product >= 0)
        {
            // target is perpendicular or slightly past 90 degrees
            return Orientation.AWAY;
        } else
        {
            return Orientation.TOWARDS;
        }
    }

    private ScreenPlacement determineScreenPlace(Vector3 coords)
    {
        //Debug.Log(coords);
        //Debug.Log(coords.x);
        //Debug.Log(coords.x >= 0.4f);
        //Debug.Log(coords.x <= 0.6f);

        ScreenPlacement place;

        // taken into account the y coordinate as well to determine 'centeredness'
        if (coords.x >= 0.4f && coords.x <= 0.6f && coords.y <= 0.65f && coords.y >= 0.45f)
        {
            place =  ScreenPlacement.CENTER;
        } else if (coords.x >= 0.4f && coords.x <= 0.6f) {
            //Debug.Log("not quite centered");
            place = ScreenPlacement.NOT_CENTER;
        } else if (coords.x < 0.4f)
        {
            place = ScreenPlacement.LEFT;
        } else
        {
            place = ScreenPlacement.RIGHT;
        }

        //Debug.Log(place);
        //Debug.Log("-----------");
        return place;
    }

    public void saveScreenshot()
    {
        byte[] bytes = theScreenshot.EncodeToPNG();
        File.WriteAllBytes(Application.dataPath + "/../" + System.DateTime.Now.Millisecond + "_safariSnap.png", bytes);
    }

    public String getPictureTarget()
    {
        return targetName;
    }

    public float getDistanceAway()
    {
        return distanceAway;
    }

    public ScreenPlacement getScreenPlacement()
    {
        return screenPlacement;
    }

    public Orientation getOrientation()
    {
        return orientation;
    }

    public Texture2D getPicture()
    {
        return theScreenshot;
    }

    public AnimalBehavior getAnimalState()
    {
        return animalState;
    }

}
