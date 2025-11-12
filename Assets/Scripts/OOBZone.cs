using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OOBZone : MonoBehaviour
{
    public bool OnObjectExit = false;
    public Vector3 ResetPosition = new(0, 10, 0);
    public bool ClearVelocity = true;

    private void Start()
    {
        var collider = GetComponent<Collider>();
        if (collider != null) collider.isTrigger = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!OnObjectExit) return;

        var rigidbody = other.GetComponent<Rigidbody>();

        if (rigidbody == null)
        {
            rigidbody = other.GetComponentInParent<Rigidbody>();
        }

        if(rigidbody == null)
        {
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;
            rigidbody.position = ResetPosition;

            if (ClearVelocity) rigidbody.AddForce(-rigidbody.velocity, ForceMode.VelocityChange);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (OnObjectExit) return;

        var rigidbody = other.GetComponent<Rigidbody>();

        if (rigidbody == null)
        {
            rigidbody = other.GetComponentInParent<Rigidbody>();
        }
        if (rigidbody == null)
        {
            rigidbody.MovePosition(ResetPosition);

            if (ClearVelocity) rigidbody.AddForce(-rigidbody.velocity, ForceMode.VelocityChange);
        }
    }
}
