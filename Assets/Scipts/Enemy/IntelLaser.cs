using UnityEngine;

public class IntelLaser : MonoBehaviour
{
    [SerializeField]
    private float _speed = 6f;
    private Transform _targetPowerup;

    public void SetTarget(Transform powerup)
    {
        _targetPowerup = powerup;
        
    }

    void Start()
    {
        if (_targetPowerup == null)
        {
            Destroy(gameObject);
           
        }
    }

    void Update()
    {
        if (_targetPowerup == null)
        {
            Destroy(gameObject);
            
            return;
        }

        // Move towards powerup
        float step = _speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, _targetPowerup.position, step);

        // Rotate to face target
        Vector3 direction = _targetPowerup.position - transform.position;
        if (direction != Vector3.zero)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle - 90f, Vector3.forward);
        }

        // Destroy if close
        if (Vector3.Distance(transform.position, _targetPowerup.position) < 0.1f)
        {
            if (_targetPowerup != null)
            {
                Destroy(_targetPowerup.gameObject);
               
            }
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("IntelEnemy") || other.CompareTag("Enemy"))
        {
            return;
        }
        if (other.CompareTag("Powerup"))
        {
            Destroy(other.gameObject);
            Destroy(gameObject);

        }
    }
}