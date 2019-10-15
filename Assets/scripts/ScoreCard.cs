using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreCard {

    public string distancePoints;
    public string orientationPoints;
    public string viewportBalancePoints;
    public string behaviorPoints;
    public int score;

    public ScoreCard(string dp, string op, string vbp, string bp, int s)
    {
        distancePoints = dp;
        orientationPoints = op;
        viewportBalancePoints = vbp;
        behaviorPoints = bp;
        score = s;
    }

}
