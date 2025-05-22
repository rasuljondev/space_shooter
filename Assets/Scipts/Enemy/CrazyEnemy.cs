using UnityEngine;

public class CrazyEnemy : Enemy
{
    [SerializeField]
    private float _crazyAngle = 45f; 
    protected override void Start()
    {
        base.Start();
        _movementType = MovementType.Angle;
        _angle = _crazyAngle;
    }
}