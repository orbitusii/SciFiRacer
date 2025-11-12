using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SpacecraftData : ScriptableObject
{
    [Range(0,10), Tooltip("How fast this craft can go. Higher is faster")]
    public int Speed = 5;

    [Range(0, 10), Tooltip("How quickly this craft accelerates to top speed. Higher is quicker")]
    public int Acceleration = 5;

    [Range(0, 10), Tooltip("How much boost this craft gets.")]
    public int Boost = 5;

    [Range(0, 10), Tooltip("How fast this craft turns. Higher = faster turns")]
    public int TurnRate = 5;

    [Range(0, 10), Tooltip("How snappy this craft's roll response is. 1 = tight, 10 = floaty")]
    public int Snappiness = 5;

    [Range(0, 10), Tooltip("How floaty this craft is. 1 = tight, 10 = floaty")]
    public int Drift = 5;

}
