using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfaceFollower : MonoBehaviour
{
    public float TopSpeed = 3;
    public float Accel = 3;
    [field: SerializeField]
    public float CurrentSpeed { get; private set; }
    public Vector3 Velocity { get; private set; } = Vector3.zero;

    public float TurnSpeed = 1;
    [field:SerializeField]
    public float CurrentAngle { get; private set; }

    public float HoverDist = 0.25f;

    public Vector3 scanArea = Vector3.one;
    [Range(0, 0.25f)]
    public float ScanRadius = 0;
    float ScanLength => scanArea.y;

    private MainControls.DefaultActions inputs;

    private void Start()
    {
        MainControls c = new MainControls();
        inputs = c.Default;
        inputs.Enable();
    }

    void FixedUpdate()
    {
        transform.position += transform.forward * Time.fixedDeltaTime;

        Vector3[] scanPoints = GetScanPoints();

        Vector3 avgNormal = Vector3.zero;
        Vector3 avgPoint = Vector3.zero;
        bool somethingHit = false;
        int totalHits = 0;

        foreach (Vector3 sp in scanPoints)
        {
            RaycastHit hit;
            bool didHit;

            if(ScanRadius > 0)
            {
                didHit = Physics.SphereCast(sp, ScanRadius, -transform.up, out hit, ScanLength);
            }
            else
            {
                didHit = Physics.Raycast(sp, -transform.up, out hit, ScanLength);
            }

            avgNormal += didHit ? hit.normal : Vector3.zero;
            avgPoint += didHit ? hit.point : Vector3.zero;
            totalHits += didHit ? 1 : 0;

            somethingHit |= didHit;
        }

        avgNormal = somethingHit ? avgNormal / totalHits : transform.up;
        avgPoint = somethingHit ? 
            (avgPoint / totalHits) + (avgNormal * HoverDist) : 
            (transform.position + Vector3.down * Time.fixedDeltaTime);

        Vector3 newVel = Velocity - Vector3.Dot(Velocity, avgNormal.normalized) * avgNormal.normalized;

        float userAccel = inputs.Throttle.ReadValue<float>();

        CurrentSpeed = Mathf.Clamp(CurrentSpeed + userAccel * Time.fixedDeltaTime * Accel, 0, TopSpeed);

        transform.position = avgPoint;
        transform.rotation = Quaternion.LookRotation(newVel.sqrMagnitude == 0 ? transform.forward : newVel, avgNormal);
    }

    private void OnDrawGizmosSelected()
    {
        Vector3[] scanPoints = GetScanPoints();

        foreach (Vector3 sp in scanPoints)
        {
            Gizmos.DrawRay(sp, -transform.up * ScanLength);
        }
    }

    private Vector3[] GetScanPoints ()
    {
        float x = scanArea.x / 2;
        float y = scanArea.y / 2;
        float z = scanArea.z / 2;

        Vector3 LR = transform.TransformPoint(new Vector3(-x, y, -z));
        Vector3 RR = transform.TransformPoint(new Vector3(x, y, -z));
        Vector3 LF = transform.TransformPoint(new Vector3(-x, y, z));
        Vector3 RF = transform.TransformPoint(new Vector3(x, y, z));

        return new Vector3[] { LR, RR, LF, RF };
    }
}
