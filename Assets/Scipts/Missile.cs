using UnityEngine;

public class Missile : MonoBehaviour
{
    [SerializeField]
    private float _speed = 8f;
    [SerializeField]
    private float _rotateSpeed = 200f;
    [SerializeField]
    private GameObject _explosionPrefab; 

    private Transform _target;

    void Start()
    {
        
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float minDist = Mathf.Infinity;
        Transform closest = null;
        foreach (GameObject enemy in enemies)
        {
            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = enemy.transform;
            }
        }
        _target = closest;

        
        if (_target == null)
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (_target == null)
        {
            Destroy(gameObject);
            return;
        }

        // Move towards the target
        Vector3 direction = (_target.position - transform.position).normalized;
        float rotateAmount = Vector3.Cross(direction, transform.up).z;
        transform.Rotate(0, 0, -rotateAmount * _rotateSpeed * Time.deltaTime);
        transform.position += transform.up * _speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_target != null && other.transform == _target)
        {
            
            if (_explosionPrefab != null)
                Instantiate(_explosionPrefab, transform.position, Quaternion.identity);

            Destroy(_target.gameObject);
            Destroy(gameObject);
        }
    }
}
