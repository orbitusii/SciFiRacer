using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Waypoint : MonoBehaviour
{
    public float gizmoSize = 10;
    public int Index { get; private set; }
    public bool isStartFinish = false;

    public void SetIndex (int ind)
    {
        Index = ind;
    }

    private void Start()
    {
        Collider col = GetComponent(typeof(Collider)) as Collider;

        col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        Spacecraft sc = other.GetComponent(typeof(Spacecraft)) as Spacecraft;

        if(sc)
        {
            RaceManager.PassWaypoint(sc, this);
        }
    }

    public virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;;

        Gizmos.DrawWireSphere(transform.position, gizmoSize);
    }
}
