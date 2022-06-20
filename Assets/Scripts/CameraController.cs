using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    public Transform toFollow;
    public Vector3 followPosition;

    public float MinSpeed = 35;
    public float MaxSpeed = 200;
    public MinMax FOVRange = new MinMax(50, 70, 165);

    private Vector3 lastPosition;

    private Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        if(toFollow == null)
        {
            enabled = false;
            return;
        }

        FOVRange.Scale = MaxSpeed - MinSpeed;

        lastPosition = transform.position;

        cam = GetComponent(typeof(Camera)) as Camera;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 newPos = toFollow.TransformPoint(followPosition);

        transform.position = newPos;
        transform.rotation = toFollow.rotation;

        Vector3 vel = transform.position - lastPosition;
        vel /= Time.fixedDeltaTime;

        float fov = FOVRange.Eval(vel.magnitude - MinSpeed);

        lastPosition = transform.position;
    }

    private void OnDrawGizmos()
    {
        if(toFollow)
        {
            Gizmos.color = Color.cyan;

            Vector3 pos = toFollow.TransformPoint(followPosition);

            Gizmos.DrawSphere(pos, 0.5f);
        }
    }
}
