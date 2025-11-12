using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfaceFollower : MonoBehaviour
{
    public float TopSpeed = 3;
    public float Accel = 3;
    [field: SerializeField]
    public float CurrentSpeed { get; private set; }

    [SerializeField]
    private Vector2 SurfacePos = Vector2.zero;
    [SerializeField]
    private Vector2 SurfaceVel = Vector2.zero;

    private SplineColliderData currentCollider;

    public float TurnSpeed = 1;
    [field:SerializeField]
    public float CurrentAngle { get; private set; }

    public float HoverDist = 0.25f;

    public float ScanDist = 0.2f;

    private MainControls.DefaultActions inputs;

    private void Start()
    {
        MainControls c = new MainControls();
        inputs = c.Default;
        inputs.Enable();
    }

    void FixedUpdate()
    {
        float slew = inputs.Slew.ReadValue<float>() * Accel;
        float throttle = inputs.Throttle.ReadValue<float>() * Accel * Time.fixedDeltaTime;

        CurrentSpeed = Mathf.Clamp(CurrentSpeed + throttle, 0, TopSpeed);

        SurfaceVel = new Vector3(CurrentSpeed, slew);

        Vector3 worldVel = (transform.TransformVector(new Vector3(SurfaceVel.x, 0, SurfaceVel.y)) + (Vector3.down * 0.2f)) * Time.fixedDeltaTime;
        Vector3 nextPos = transform.position + worldVel;
        Quaternion nextRot = transform.rotation;

        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, ScanDist))
        {
            if(hit.collider.GetComponent<SplineColliderData>() is SplineColliderData scd)
            {
                if (currentCollider is null)
                {
                    currentCollider = scd;
                    Vector2 posNormalized = scd.GetSurfacePosition(hit.triangleIndex, hit.point);
                    float width = scd.Spline.Width(scd.Spline.TimeLUT.Evaluate(posNormalized.x));
                    SurfacePos = new Vector2(posNormalized.x * scd.Spline.Length, Mathf.Lerp(-width, width, posNormalized.y));
                    Debug.Log($"{posNormalized} => {SurfacePos}");
                }
                else if (scd != currentCollider)
                {
                    SurfacePos.x -= currentCollider.Spline.Length;
                    currentCollider = scd;
                }

                SurfacePos += SurfaceVel * Time.fixedDeltaTime;

                bool onCurrent = SurfacePos.x <= scd.Spline.Length;

                Spline targetSpline = onCurrent ? scd.Spline : scd.Next;
                float lengthCorrection = onCurrent ? 0 : scd.Spline.Length;

                float tDist = (SurfacePos.x - lengthCorrection) / targetSpline.Length;
                float tTrue = targetSpline.TimeLUT.Evaluate(SurfacePos.x - lengthCorrection);
                float wT = SurfacePos.y / (targetSpline.Width(tTrue) / 2);

                Matrix4x4 worldMat = targetSpline.GetMatrixAtPoint(new Vector2(tDist, wT));

                Vector3 DebugPoint = worldMat.MultiplyPoint3x4(Vector3.zero);

                Debug.DrawRay(DebugPoint, worldMat.MultiplyVector(Vector3.forward), Color.blue);
                Debug.DrawRay(DebugPoint, worldMat.MultiplyVector(Vector3.right), Color.red);
                Debug.DrawRay(DebugPoint, worldMat.MultiplyVector(Vector3.up), Color.green);

                nextPos = worldMat.MultiplyPoint3x4(new Vector3(0, HoverDist, 0));
                nextRot = worldMat.rotation;
            }
        }

        transform.position = nextPos;
        transform.rotation = nextRot;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, ScanDist);
        currentCollider?.OnDrawGizmosSelected();
    }
}
