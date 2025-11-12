using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PID
{
    public float Setpoint, Current, Output;
    public float Proportional, Integral, Derivative;

    private float Error, prevError;
    private float valP, valI, valD;

    public float Update (float dt)
    {
        prevError = Error;
        Error = Setpoint - Current;

        valP = Error;
        valI += Error * dt;
        valD = (Error - prevError) / dt;

        Output = Proportional * valP + Integral * valI + Derivative * valD;
        return Output * dt;
    }
}
