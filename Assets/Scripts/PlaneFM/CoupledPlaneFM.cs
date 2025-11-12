using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CoupledPlaneFM : MonoBehaviour, IInputSink
{
    public SpacecraftData Tuning;
    public int player { get; set; }

    public float MinSpeed;
    public float TopSpeed;
    public float Accel;
    public float Boost;

    public float Turn;
    public float Roll;
    public float RollCorrection;
    public float IRA;

    public float Drift;

    public float RollSnappiness = 1;

    // Overwriting default Rigidbody field because it no longer works. Thanks Unity.
#pragma warning disable CS0108
    public Rigidbody Rigidbody { get; private set; }
#pragma warning restore CS0108

    private List<InputSnapshot> inputCache = new List<InputSnapshot>();
    private int cacheSize;

    public float TargetRoll = 0;

    void Start()
    {
        RaceManager.RegisterRacer(this);

        var td = RaceManager.GetTuningData();

        MinSpeed = td.MinSpeed;
        TopSpeed = td.SpeedRange.Eval(Tuning.Speed);
        Accel = td.AccelRange.Eval(Tuning.Acceleration);
        Boost = td.BoostRange.Eval(Tuning.Boost);

        Turn = td.TurnRange.Eval(Tuning.TurnRate);
        Roll = td.RollRate.Eval(Tuning.Snappiness);
        RollCorrection = td.RollCorrection;
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
            Vector3 euler = transform.eulerAngles;
            Vector3 currentAngVel = Rigidbody.angularVelocity;

            float dt = Time.fixedDeltaTime;

            //Rigidbody.MoveRotation(transform.rotation * Quaternion.Euler(pitch * dt * Mathf.Rad2Deg, 0, -roll * dt * Mathf.Rad2Deg));
            //Rigidbody.AddRelativeTorque(pitch, 0, roll, ForceMode.Acceleration);

            // Roll
            TargetRoll = Mathf.Clamp(TargetRoll - snap.Yaw * RollCorrection * dt, -135, 135);
            float currentRoll = euler.z;
            currentRoll = currentRoll > 180 ? currentRoll - 360 : currentRoll;

            float rollError = (TargetRoll - currentRoll) * Mathf.Deg2Rad;

            float rr = Mathf.Min(Mathf.Abs(rollError)*10, Roll) * Mathf.Sign(rollError);

            Rigidbody.AddRelativeTorque(Vector3.forward * 6*(rr - currentAngVel.z), ForceMode.Acceleration);

            // Pitch
            float pitch = Mathf.Min(Mathf.Abs(Turn * snap.Pitch), Turn) * Mathf.Sign(snap.Pitch);
        }
    }
}
