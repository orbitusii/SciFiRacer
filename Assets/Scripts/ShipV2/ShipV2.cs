using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipV2 : MonoBehaviour
{
    public float RideExtrapolation = 1;
    public PID RideHeightController = new()
    {
        Setpoint = 5,
        Current = 0,
        Proportional = 1,
        Integral = 0.12f,
        Derivative = 0.5f,
    };
    public Vector3 LocalUp = Vector3.up;
    public Vector3 LocalDown => -LocalUp;

    public Rigidbody Rigidbody;
    private RaycastHit hitData;
    public LayerMask LayerMask;

    public GlobalTuningData GlobalTuning;
    public SpacecraftData ShipTuning;

    public float PowerCapacity = 100;
    public float ChargeSpeed = 5;
    protected float CurrentPower
    {
        get => _power;
        set => _power = Mathf.Clamp(value, 0, PowerCapacity);
    }
    private float _power = 0;

    private MainControls.DefaultActions Input;

    // Start is called before the first frame update
    void Start()
    {
        var attachedRb = gameObject.GetComponent(typeof(Rigidbody)) as Rigidbody;

        if (Rigidbody == null)
        {
            Rigidbody = attachedRb;
        }

        var inputObject = new MainControls();
        Input = inputObject.Default;
        Input.Enable();

        CurrentPower = PowerCapacity;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Rigidbody == null) return;

        float dt = Time.fixedDeltaTime;

        bool powerRecharge = true;

        if(DoGroundInteraction())
        {
            // Don't drain power
        }
        else powerRecharge = false;

        DoLinearMotion();

        if (DoAngularMotion())
        {
            // Don't drain power
        }
        else powerRecharge = false;

        if (powerRecharge) CurrentPower += ChargeSpeed * dt;
    }

    private bool DoGroundInteraction ()
    {
        Vector3 flattenedVel = Rigidbody.velocity - Vector3.Project(Rigidbody.velocity, transform.up);
        Vector3 scanPosition = transform.position + flattenedVel * RideExtrapolation;

        Debug.DrawRay(scanPosition, LocalDown, Color.red);

        if (Physics.Raycast(new(scanPosition, LocalDown), out hitData, 15, LayerMask))
        {
            RideHeightController.Current = hitData.distance;
            float delta = RideHeightController.Update(Time.fixedDeltaTime);

            Rigidbody.AddForce(LocalUp * delta, ForceMode.VelocityChange);
            return true;
        }
        return false;
    }

    private void DoLinearMotion ()
    {
        float dt = Time.fixedDeltaTime;
        float Thrust = Input.Boost.ReadValue<float>();
        Vector3 localVelocity = transform.InverseTransformVector(Rigidbody.velocity);

        float TargetSpeed = GlobalTuning.SpeedRange.Eval(ShipTuning.Speed) * (CurrentPower / PowerCapacity) * Thrust;
        float CurrentSpeed = localVelocity.z;
        float MaxAccel = GlobalTuning.AccelRange.Eval(ShipTuning.Acceleration);

        float FwdAccel = Mathf.Min(TargetSpeed - CurrentSpeed, MaxAccel);

        float drift = localVelocity.x;

        float DriftCorrection = GlobalTuning.DriftRange.Eval(10 - ShipTuning.Drift);

        Vector3 totalVelocityChange = new(DriftCorrection * -Mathf.Sign(drift) * Mathf.Abs(drift), 0, FwdAccel);
        Debug.Log(totalVelocityChange);

        Rigidbody.AddRelativeForce(totalVelocityChange * dt, ForceMode.VelocityChange);
    }

    private bool DoAngularMotion ()
    {
        float dt = Time.fixedDeltaTime;
        float Steer = Input.Slew.ReadValue<float>();
        Vector3 localAngularVel = transform.InverseTransformVector(Rigidbody.angularVelocity);
        bool powerRecharge = true;

        if (Mathf.Abs(Steer) > float.Epsilon)
        {
            powerRecharge = false;
            CurrentPower -= 5 * dt;
        }

        float TargetTurn = GlobalTuning.TurnRange.Eval(ShipTuning.TurnRate) * Steer;
        float CurrentTurn = localAngularVel.y;
        float TurnAccel = TargetTurn - CurrentTurn;

        Vector3 totalAngVelChange = new(0, TurnAccel, 0);

        Rigidbody.AddRelativeTorque(totalAngVelChange, ForceMode.VelocityChange);

        return powerRecharge;
    }
}
