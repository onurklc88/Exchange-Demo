
using UnityEngine;

[CreateAssetMenu(fileName = "OperatorProperties", menuName = "ScriptableObjects/Operator", order = 1)]
public class OperatorProperties : ScriptableObject
{
    public enum OperatorType { None, Assult, Heavy, Scout }
    [Header("Movement Properties")]
    public float WalkSpeed;
    public float RunSpeed;
    public float CrouchSpeed;
    [Header("Health Properties")]
    public float TotalHealth;
}
