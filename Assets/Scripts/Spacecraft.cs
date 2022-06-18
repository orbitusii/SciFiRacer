using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Spacecraft : MonoBehaviour
{
    public SpacecraftData Data;
    public int player;

    public float MinSpeed;
    public float TopSpeed;
    public float Accel;
    public float Boost;

    public float Turn;
    public float Roll;
    public float IRA;

    public float Drift;

#pragma warning disable CS0108
    public Rigidbody Rigidbody { get; private set; }
#pragma warning restore CS0108

    private List<InputSnapshot> inputCache = new List<InputSnapshot>();
    private int cacheSize;

    private Vector3 steerDirection = Vector3.forward;

    void Start()
    {
        RaceManager.RegisterRacer(this);

        var td = RaceManager.GetTuningData();

        MinSpeed = td.MinSpeed;
        TopSpeed = td.SpeedRange.Eval(Data.Speed);
        Accel = td.AccelRange.Eval(Data.Acceleration);
        Boost = td.BoostRange.Eval(Data.Boost);

        Turn = td.TurnRange.Eval(Data.TurnRate);
        Roll = td.RollCorrection;
        IRA = td.IgnoreRollAngle;

        Drift = td.DriftRange.Eval(Data.Drift);

        Rigidbody = GetComponent(typeof(Rigidbody)) as Rigidbody;

        cacheSize = td.InputCacheSize;
        if (cacheSize < 1) cacheSize = 1;
    }

    public void SetFrameInput (InputSnapshot snap)
    {
        if(player == snap.Player)
        {
            inputCache.Insert(0, snap);

            if(inputCache.Count > cacheSize)
            {
                inputCache.RemoveRange(cacheSize - 1, inputCache.Count - cacheSize);
            }
        }
    }

    void FixedUpdate()
    {
        if(inputCache.Count > 0 && !inputCache[0].Used)
        {
            InputSnapshot snap = inputCache[0];
            // Rotations
            Vector3 euler = transform.localEulerAngles;

            float pitch = Turn * snap.Pitch;
            float yaw = Turn * snap.Yaw;

            float rollCorrection = 0;

            float pitchAngle = euler.x > 180f ? euler.x - 360 : euler.x;
            if (Mathf.Abs(pitchAngle) < IRA)
            {

                // Clamp roll angle for sane behavior
                float rollAngle = euler.z > 180f ? euler.z - 360 : euler.z;
                rollCorrection = rollAngle * Mathf.Deg2Rad * Roll * (1 - Mathf.Abs(snap.Pitch));
            }

            Rigidbody.AddRelativeTorque(pitch, yaw, rollCorrection, ForceMode.Acceleration);

            // Forward Forces
            Vector3 localVel = transform.InverseTransformVector(Rigidbody.velocity);

            float deltaV = snap.Throttle * Accel;

            float topDeltaV = TopSpeed - localVel.z;
            float lowDeltaV = MinSpeed - localVel.z;

            float clampedDV = Mathf.Clamp(deltaV, lowDeltaV, topDeltaV);

            Rigidbody.AddRelativeForce(0, 0, clampedDV, ForceMode.Acceleration);

            // Drift Forces
            Vector3 driftCounter = new Vector3(-localVel.x, -localVel.y, 0);

            driftCounter *= Drift;

            Rigidbody.AddRelativeForce(driftCounter, ForceMode.Acceleration);

            snap.Used = true;
        }
    }
}
