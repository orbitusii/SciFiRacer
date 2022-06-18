using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SpacecraftData : ScriptableObject
{
    [Range(0,10), Tooltip("How fast this craft can go. Higher is faster")]
    public float Speed = 5;

    [Range(0, 10), Tooltip("How quickly this craft accelerates to top speed. Higher is quicker")]
    public float Acceleration = 5;

    [Range(0, 10), Tooltip("How much boost this craft gets.")]
    public float Boost = 5;

    [Range(0, 10), Tooltip("How fast this craft turns. Higher = faster turns")]
    public float TurnRate = 5;

    [Range(0, 10), Tooltip("How floaty this craft is. 1 = tight, 10 = floaty")]
    public float Drift = 5;
}
