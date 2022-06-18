using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceManager : MonoBehaviour
{
    public static RaceManager singleton;

    public bool DrawGizmosAlways = false;

    public GlobalTuningData Tuning;

    public float StartSpeed;
    public List<Transform> StartPoints = new List<Transform>();

    public List<Waypoint> waypoints = new List<Waypoint>();

    public List<RaceUser> racers = new List<RaceUser>();

    public bool PracticeLap = true;
    public int Laps = 3;
    public int lapLength = 0;

    private void Awake()
    {
        if(singleton)
        {
            Debug.LogWarning("Multiple race managers present! Please delete any extras.", this);
            enabled = false;
            return;
        }

        singleton = this;

        lapLength = 0;
        foreach(Waypoint wp in waypoints)
        {
            wp.SetIndex(lapLength++);
        }
    }

    public static void RegisterRacer (Spacecraft sc)
    {
        RaceUser newUser = new RaceUser(sc);
        newUser.lapCount = singleton.PracticeLap ? -1 : 0;
        singleton.racers.Add(newUser);

        Debug.Log("Registered racer", sc);
    }

    public static bool PassWaypoint (Spacecraft sc, Waypoint wp)
    {
        RaceUser user = singleton.racers.Find(x => x.Equals(sc));

        if (user.waypoint == wp.Index - 1)
        {
            Debug.Log($"Racer passed waypoint {wp.name}", sc);
            user.waypoint++;
        }
        else if (user.waypoint == singleton.lapLength - 1 && wp.isStartFinish)
        {
            user.waypoint = 0;
            user.lapCount++;
            Debug.Log($"Racer passed the start and is now on lap {user.lapCount + 1}/{singleton.Laps}", sc);

            if (user.lapCount == singleton.Laps)
            {
                Debug.Log("Racer completed the race!");
                Time.timeScale = 0;
            }
        }


        return false;
    }
    public static GlobalTuningData GetTuningData()
    {
        return singleton.Tuning;
    }

    public static List<RaceUser> GetRaceUsers()
    {
        return singleton.racers;
    }

    private void OnDrawGizmos()
    {
        if (DrawGizmosAlways)
        {
            OnDrawGizmosSlected();
        }
    }

    private void OnDrawGizmosSlected()
    {
        DrawStartPoints();

        DrawRaceLine();
    }

    public void DrawStartPoints ()
    {
        Gizmos.color = Color.blue;

        for (int i = 0; i < StartPoints.Count; i++)
        {
            Transform sp = StartPoints[i];

            Gizmos.DrawSphere(sp.position, 1f);

            Gizmos.DrawRay(sp.position, sp.forward);
        }
    }

    public void DrawRaceLine ()
    {
        if (waypoints.Count >= 2)
        {
            Gizmos.color = Color.red;

            for (int i = 0; i < waypoints.Count - 1; i++)
            {
                Vector3 pos1 = waypoints[i].transform.position;
                Vector3 pos2 = waypoints[i + 1].transform.position;

                Gizmos.DrawLine(pos1, pos2);
            }

            Vector3 posi = waypoints[waypoints.Count - 1].transform.position;
            Vector3 pos0 = waypoints[0].transform.position;

            Gizmos.DrawLine(posi, pos0);
        }
    }
}

[System.Serializable]
public class RaceUser
{
    public Spacecraft spacecraft;

    public int lapCount = 0;
    public int waypoint = 0;

    public RaceUser (Spacecraft sc)
    {
        spacecraft = sc;
        lapCount = 0;
        waypoint = 0;
    }

    public bool Equals (Spacecraft other)
    {
        return spacecraft == other;
    }

    public bool Equals (int player)
    {
        return spacecraft.player == player;
    }
}
