using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfaceFollower : MonoBehaviour
{
    public float Speed = 3;
    public float Accel = 3;
    [field: SerializeField]
    public float CurrentSpeed { get; private set; }

    public float TurnSpeed = 1;
    [field:SerializeField]
    public float CurrentAngle { get; private set; }

    public Vector3 scanArea = Vector3.one;

    void FixedUpdate()
    {

    }

    private void OnDrawGizmosSelected()
    {
        float x = scanArea.x / 2;
        float y = scanArea.y / 2;
        float z = scanArea.z / 2;

        float scanLength = scanArea.y;

        Vector3 LR = transform.TransformPoint(new Vector3(-x, y, -z));
        Vector3 RR = transform.TransformPoint(new Vector3(x, y, -z));
        Vector3 LF = transform.TransformPoint(new Vector3(-x, y, z));
        Vector3 RF = transform.TransformPoint(new Vector3(x, y, z));

        Gizmos.DrawRay(LR, -transform.up * scanLength);
        Gizmos.DrawRay(RR, -transform.up * scanLength);
        Gizmos.DrawRay(LF, -transform.up * scanLength);
        Gizmos.DrawRay(RF, -transform.up * scanLength);
    }
}
