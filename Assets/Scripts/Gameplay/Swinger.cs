using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Swinger", menuName = "SwingyRopes/Swinger")]
public class Swinger : ScriptableObject {

    public float SwingForce;
    public float Duration;
    public SwingFrequencies Frequency;
    public AnimationCurve ForceCurve;

    public enum SwingFrequencies
    {
        Continuous,
        TwicePerCycle,
        OncePerCycle
    }

    public float GetSwingForce(float t)
    {
        return ForceCurve.Evaluate(t) * SwingForce;
    }
}
