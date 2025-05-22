using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private float _speed = 8f;
    private bool _isEnemyLaser = false;
    private bool _isBackLaser = false;
    public bool IsEnemyLaser { get; set; } = false;

    void Update()
    {
        if (_isEnemyLaser)
        {
            if (_isBackLaser)
                MoveUp();
            else
                MoveDown();
        }
        else
        {
            MoveUp();
        }
    }

    void MoveUp()
    {
        transform.Translate(Vector3.up * _speed * Time.deltaTime);
        if (transform.position.y > 8f)
        {
            if (transform.parent != null)
                Destroy(transform.parent.gameObject);
            Destroy(gameObject);
        }
    }

    void MoveDown()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
        if (transform.position.y < -8f)
        {
            if (transform.parent != null)
                Destroy(transform.parent.gameObject);
            Destroy(gameObject);
        }
    }

    public void AssignEnemyLaser(bool isBackLaser = false)
    {
        _isEnemyLaser = true;
        _isBackLaser = isBackLaser;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isEnemyLaser && !_isBackLaser && other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.Damage();
                Destroy(gameObject);
            }
        }
        
        else if (other.CompareTag("Enemy") || other.CompareTag("Boss"))
        {
            return;
        }
    }
}