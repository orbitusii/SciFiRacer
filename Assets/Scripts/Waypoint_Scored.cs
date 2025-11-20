using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Waypoint_Scored : MonoBehaviour
{
    public float GizmoSize = 10;
    public int Index { get; private set; }
    public bool IsStart = false;
    public bool IsFinish = false;

    public MinMax HighScoreRange = new MinMax(-10, 10);
    public MinMax MedScoreRange = new MinMax(-25, -10);
    public MinMax LowScoreRange = new MinMax(10, 20);

    public Vector3Int Points = new(5, 3, 1);

    public float ScoreRangeDisplayDist = 10;

    public void SetIndex(int ind)
    {
        Index = ind;
    }

    // Start is called before the first frame update
    void Start()
    {
        Collider col = GetComponent(typeof(Collider)) as Collider;

        col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        Racer racer = other.GetComponent(typeof(Racer)) as Racer;

        if (racer)
        {
            Debug.Log($"Racer {racer.gameObject.name} passed waypoint {Index}");

            if (IsStart)
            {
                racer.StartScoring();
            }
            else if (IsFinish)
            {
                var scores = racer.FinalizeScoring();

                Debug.Log($"Racer {racer.gameObject.name} completed course, scoring {scores.TotalScore} in {scores.Time.TotalSeconds} seconds");
            }
            else
            {
                ScoreRacer(racer);
            }
        }
    }

    protected void ScoreRacer (Racer r)
    {
        float y = transform.InverseTransformPoint(r.transform.position).y;

        int score = 0;

        if (HighScoreRange.Contains(y)) score = Points.x;
        else if (MedScoreRange.Contains(y)) score = Points.y;
        else if (LowScoreRange.Contains(y)) score = Points.z;

        r.ScoreWaypoint(Index, score);
    }

    public virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow; ;

        Gizmos.DrawWireSphere(transform.position, GizmoSize);

        Vector3 HiScMin = transform.TransformPoint(ScoreRangeDisplayDist-1, HighScoreRange.Min, 0);
        Vector3 HiScMax = transform.TransformPoint(ScoreRangeDisplayDist-1, HighScoreRange.Max, 0);

        Vector3 MidScMin = transform.TransformPoint(ScoreRangeDisplayDist, MedScoreRange.Min, 0);
        Vector3 MidScMax = transform.TransformPoint(ScoreRangeDisplayDist, MedScoreRange.Max, 0);

        Vector3 LoScMin = transform.TransformPoint(ScoreRangeDisplayDist+1, LowScoreRange.Min, 0);
        Vector3 LoScMax = transform.TransformPoint(ScoreRangeDisplayDist+1, LowScoreRange.Max, 0);


        Gizmos.color = Color.red;
        Gizmos.DrawLine(LoScMin, LoScMax);
        Gizmos.DrawRay(LoScMin, -transform.right);
        Gizmos.DrawRay(LoScMax, -transform.right);

        Gizmos.color = Color.white;
        Gizmos.DrawLine(MidScMin, MidScMax);
        Gizmos.DrawRay(MidScMin, -transform.right);
        Gizmos.DrawRay(MidScMax, -transform.right);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(HiScMin, HiScMax);
        Gizmos.DrawRay(HiScMin, -transform.right);
        Gizmos.DrawRay(HiScMax, -transform.right);
    }
}
