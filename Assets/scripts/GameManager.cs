using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Text.RegularExpressions;


// this script is used to keep track of the game state
// i.e., when player has taken X number of pictures, it transitions to a new scene to evaluate the pictures 
// this script also stores the player's Snapshot objects (see Snapshot.cs) to use in the new scene 
// and updates the picture counter in the top right of the screen

public class GameManager : MonoBehaviour {

    private List<Snapshot> picturesTaken; // store pictures taken in Snapshot object array 
    private int maxPhotos;
    public Text pictureCounter;

    // this is for looping throuhg the pictures during evaluation 
    private int pictureIndex;
    private int currentScore;
    private bool receivedTotalScore; // keep track if player already saw all pics so we don't keep adding to currentScore

    private void Awake()
    {
        // let this game manager persist through the whole game
        DontDestroyOnLoad(this.gameObject);
    }

    // Use this for initialization
    void Start () {
        maxPhotos = 5;
        pictureIndex = 0;
        currentScore = 0;
        receivedTotalScore = false;
        picturesTaken = new List<Snapshot>();
	}
	
	// Update is called once per frame
	void Update () {

        Scene s = SceneManager.GetActiveScene();

        if(s.name == "pictureEvaluationScene")
        {
            //Debug.Log(picturesTaken.Count);
            if(pictureIndex == maxPhotos && Input.GetKeyDown("space"))
            {
                // show total score and ask if user wants to try again
                Text scoreText = GameObject.Find("imageDisplayCanvas").transform.Find("ScoreText").GetComponent<Text>();
                scoreText.text = "";

                Text evalText = GameObject.Find("imageDisplayCanvas").transform.Find("EvaluationText").GetComponent<Text>();
                evalText.text = "Your total score is: " + currentScore + "!" + " Use the arrow keys to review your pictures, or quit with Q or play again with R.";

                receivedTotalScore = true;
                return;
            }

            if(Input.GetKeyDown(KeyCode.LeftArrow) && receivedTotalScore)
            {
                if(pictureIndex == 0)
                {
                    return;
                }else
                {
                    pictureIndex--;
                    displayPictureAndEvaluation(picturesTaken[pictureIndex]);
                }
            }

            if (Input.GetKeyDown(KeyCode.RightArrow) && receivedTotalScore)
            {
                if(pictureIndex == picturesTaken.Count - 1)
                {
                    return;
                }else
                {
                    pictureIndex++;
                    displayPictureAndEvaluation(picturesTaken[pictureIndex]);
                }
            }

            if (Input.GetKeyDown("r"))
            {
                // clear data 
                picturesTaken.Clear();
                currentScore = 0;

                // restart game 
                SceneManager.LoadScene("mainScene");
            }

            // allow player to quit 
            if (Input.GetKeyDown("q"))
            {
                Application.Quit();
            }

            // display pictures
            if (pictureIndex == 0 || (pictureIndex > 0 && Input.GetKeyDown("space")))
            {
                displayPictureAndEvaluation(picturesTaken[pictureIndex++]);
            }
        }


    }

    private void displayPictureAndEvaluation(Snapshot thePicture)
    {
        Snapshot currSnapshot = thePicture; //picturesTaken[pictureIndex];

        // create the sprite first 
        Texture2D currTex = currSnapshot.getPicture();
        Sprite currPicture = Sprite.Create(currTex, new Rect(0, 0, currTex.width, currTex.height), new Vector2(0.5f, 0.5f), 100f);

        // place the sprite 
        Image imageDisplay = GameObject.Find("imageDisplayCanvas").GetComponentInChildren<Image>();
        imageDisplay.sprite = currPicture;

        // evaluate pic and assign points
        string animalName = currSnapshot.getPictureTarget();
        float distance = currSnapshot.getDistanceAway();
        Snapshot.Orientation orientation = currSnapshot.getOrientation();
        Snapshot.ScreenPlacement placement = currSnapshot.getScreenPlacement();
        AnimalBehavior animalState = currSnapshot.getAnimalState();

        //Debug.Log("distance: " + distance);

        // parse animal name to exclude numbers 
        string patternToMatch = "[a-zA-z]+";
        animalName = Regex.Match(animalName, patternToMatch).Value;

        // get score and eval
        ScoreCard eval = calculateScore(distance, orientation, placement, animalState);

        // update evaluation text
        Text evalText = GameObject.Find("imageDisplayCanvas").transform.Find("EvaluationText").GetComponent<Text>();

        string etext = "Looks like you snapped a picture of a(n) ";
        etext += (eval.behaviorPoints == "") ? "" : eval.behaviorPoints + " ";
        etext += animalName + "! Great! ";
        etext += eval.orientationPoints + " ";
        etext += eval.distancePoints + " ";
        etext += eval.viewportBalancePoints;

        evalText.text = etext;

        // update score text
        Text scoreText = GameObject.Find("imageDisplayCanvas").transform.Find("ScoreText").GetComponent<Text>();
        scoreText.text = "Score: " + eval.score;

        if (!receivedTotalScore)
        {
            currentScore += eval.score;
        }
    }

    private ScoreCard calculateScore(float distance, Snapshot.Orientation o, Snapshot.ScreenPlacement sp, AnimalBehavior animalState)
    {
        string distancePoints = "";
        string orientationPoints = "";
        string viewportBalancePoints = "";
        string behaviorPoints = ""; // a string of the current state's name 

        // with regards to distance, too close and too far are bad. how to decide what is too close and too far?
        // between 1 and 5 seem to be good. any less or more is not so good. but more than 5 is better than < 1, which is too close
        int totalScore = 0;

        if (distance < 1)
        {
            totalScore += 2;
            distancePoints = "A bit too close, so +2 points.";
        }else if(distance >= 1 && distance < 5)
        {
            totalScore += 8;
            distancePoints = "Good distance, +8 points!";
        }
        else if(distance >= 5 && distance < 6)
        {
            totalScore += 5;
            distancePoints = "A bit far, but not too bad so +5 points.";
        }
        else
        {
            totalScore -= 2;
            distancePoints = "A bit too far, so -2 points.";
        }

        if (o == Snapshot.Orientation.AWAY)
        {
            totalScore += 5;
            orientationPoints = "Hmm, it's facing a bit away but that's not bad. +5 points.";
        }
        else if(o == Snapshot.Orientation.COMPLETELY_AWAY)
        {
            // if facing completely away, bad - deduct score 
            totalScore -= 5;
            orientationPoints = "Why is it facing away from the camera!? -5 points :(";
        }
        else
        {
            totalScore += 8;
            orientationPoints = "Good orientation! +8 points.";
        }

        if(sp == Snapshot.ScreenPlacement.LEFT || sp == Snapshot.ScreenPlacement.RIGHT)
        {
            // if not centered, deduct 1 point 
            totalScore -= 1;
            string place = (sp == Snapshot.ScreenPlacement.LEFT) ? "left" : "right";
            viewportBalancePoints = "It's a bit too much to the " + place + ". -1 point.";
        }else if(sp == Snapshot.ScreenPlacement.NOT_CENTER)
        {
            totalScore -= 1;
            viewportBalancePoints = "It's not quite centered. -1 point.";
        }
        else
        {
            totalScore += 5;
            viewportBalancePoints = "Good balance. +5 points.";
        }

        if (animalState == AnimalBehavior.Eating)
        {
            totalScore += 2;
            behaviorPoints = "eating";
        }else if(animalState == AnimalBehavior.Sitting)
        {
            totalScore += 2;
            behaviorPoints = "sitting";
        }else if(animalState == AnimalBehavior.GettingUp)
        {
            totalScore += 3;
            behaviorPoints = "getting up";
        }else if(animalState == AnimalBehavior.Angry)
        {
            totalScore += 5;
            behaviorPoints = "angry";
        }

        ScoreCard card = new ScoreCard(distancePoints, orientationPoints, viewportBalancePoints, behaviorPoints, totalScore);
        return card;
    }

    // update picturesTaken list. if reached maxPhotos, move to new scene
    public void updatePicturesTaken(Snapshot s)
    {
        picturesTaken.Add(s);

        // update picture counter 
        pictureCounter.text = "" + (maxPhotos - picturesTaken.Count);

        // check size
        if(picturesTaken.Count == maxPhotos)
        {
            //Debug.Log("no more pics to take!");
            moveToPictureEvaluationScene();
        }
    }


    // alert this class to move to next scene
    private void moveToPictureEvaluationScene()
    {
        // do a fade out 
        SceneManager.LoadScene("pictureEvaluationScene");
    }


    // button onclick actions
    public void playGame()
    {
        SceneManager.LoadScene("mainScene");
    }

    public void quitGame()
    {
        Application.Quit();
    }

}
