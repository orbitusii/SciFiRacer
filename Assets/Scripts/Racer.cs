using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Racer : MonoBehaviour
{
    public bool IsScoringAllowed = false;
    public System.DateTime StartTime;
    public Dictionary<int, int> WaypointScores = new Dictionary<int, int>();

    public void ScoreWaypoint(int waypointIndex, int value)
    {
        if(WaypointScores.ContainsKey(waypointIndex))
        {
            int lastScore = WaypointScores[waypointIndex];

            WaypointScores[waypointIndex] = lastScore > value ? lastScore : value;
        }
        else
        {
            WaypointScores[waypointIndex] = value;
        }
    }

    public void StartScoring ()
    {
        IsScoringAllowed = true;
        StartTime = System.DateTime.Now;
        WaypointScores.Clear();
    }

    public RacerScoreData FinalizeScoring ()
    {
        IsScoringAllowed = false;
        System.DateTime end = System.DateTime.Now;
        return new RacerScoreData(StartTime, end, WaypointScores);
    }
}

public class RacerScoreData
{
    public System.TimeSpan Time;
    public int TotalScore = 0;
    public Dictionary<int, int> WaypointScores;

    public RacerScoreData(System.DateTime start, System.DateTime end, Dictionary<int, int> scores)
    {
        Time = end - start;
        WaypointScores = scores;
        TotalScore = 0;

        foreach(int value in WaypointScores.Values)
        {
            TotalScore += value;
        }
    }
}
