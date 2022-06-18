using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GlobalTuningData : ScriptableObject
{
    public float MinSpeed = 35;
    public MinMax SpeedRange = new MinMax(100, 150);
    public MinMax AccelRange = new MinMax(75, 125);
    public MinMax BoostRange = new MinMax(50, 75);

    public MinMax TurnRange = new MinMax(2, 3);
    public float RollCorrection = 5;
    public float IgnoreRollAngle = 30;

    public MinMax DriftRange = new MinMax(0.75f, 0.25f);

    public int InputCacheSize = 64;
}

[System.Serializable]
public class MinMax
{
    public float Min;
    public float Max;

    public MinMax (float min, float max)
    {
        this.Min = min;
        this.Max = max;
    }

    public float Eval (float value)
    {
        float lerpval = value / 10;

        return Mathf.Lerp(Min, Max, lerpval);
    }
}