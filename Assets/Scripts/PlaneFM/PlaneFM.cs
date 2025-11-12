using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A flight model similar to <see cref="Spacecraft"/> but tailored for more modern aircraft rather than for cool sci-fi racers.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class PlaneFM : MonoBehaviour, IInputSink
{
    public SpacecraftData Tuning;
    public int player { get; set; }

    public float MinSpeed;
    public float TopSpeed;
    public float Accel;
    public float Boost;

    public float Turn;
    public float Roll;
    public float IRA;

    public float Drift;

    // Overwriting default Rigidbody field because it no longer works. Thanks Unity.
#pragma warning disable CS0108
    public Rigidbody Rigidbody { get; private set; }
#pragma warning restore CS0108

    private List<InputSnapshot> inputCache = new List<InputSnapshot>();
    private int cacheSize;

    void Start()
    {
        RaceManager.RegisterRacer(this);

        var td = RaceManager.GetTuningData();

        MinSpeed = td.MinSpeed;
        TopSpeed = td.SpeedRange.Eval(Tuning.Speed);
        Accel = td.AccelRange.Eval(Tuning.Acceleration);
        Boost = td.BoostRange.Eval(Tuning.Boost);

        Turn = td.TurnRange.Eval(Tuning.TurnRate);
        Roll = td.RollCorrection;
        IRA = td.IgnoreRollAngle;

        Drift = td.DriftRange.Eval(Tuning.Drift);

        Rigidbody = GetComponent(typeof(Rigidbody)) as Rigidbody;

        cacheSize = td.InputCacheSize;
        if (cacheSize < 1) cacheSize = 1;
    }

    public void SetFrameInput(InputSnapshot snap)
    {
        if (player == snap.Player)
        {
            inputCache.Insert(0, snap);

            if (inputCache.Count > cacheSize)
            {
                inputCache.RemoveRange(cacheSize - 1, inputCache.Count - cacheSize);
            }
        }
    }

    void FixedUpdate()
    {
        if (inputCache.Count > 0 && !inputCache[0].Used)
        {
            InputSnapshot snap = inputCache[0];
            // Rotations
            Vector3 euler = transform.localEulerAngles;

            float dt = Time.fixedDeltaTime;
            float pitch = Mathf.Min(Mathf.Abs(Turn * snap.Pitch), Turn) * Mathf.Sign(snap.Pitch);
            float roll = Mathf.Min(Mathf.Abs(Turn * snap.Yaw), Turn) * Mathf.Sign(snap.Yaw);

            Rigidbody.MoveRotation(transform.rotation * Quaternion.Euler(pitch * dt * Mathf.Rad2Deg, 0, -roll * dt * Mathf.Rad2Deg) );
            //Rigidbody.AddRelativeTorque(pitch, 0, roll, ForceMode.Acceleration);

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
