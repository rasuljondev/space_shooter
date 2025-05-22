using UnityEngine;

public class AggressiveEnemy : Enemy
{
    [SerializeField]
    private float _ramDistance = 3.0f;
    [SerializeField]
    private float _ramSpeedMultiplier = 2.0f;

    protected override void CalculateMovement()
    {
        if (_player != null && Vector3.Distance(transform.position, _player.transform.position) < _ramDistance)
        {
            
            Vector3 direction = (_player.transform.position - transform.position).normalized;
            transform.position += direction * _speed * _ramSpeedMultiplier * Time.deltaTime;
        }
        else
        {
            base.CalculateMovement();
        }
    }
}
