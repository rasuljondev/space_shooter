using UnityEngine;

public class IntelEnemy : MonoBehaviour
{
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private float _speed = 4f;
    [SerializeField]
    private int _lives = 1;
    [SerializeField]
    private AudioClip _explosionClip;
    [SerializeField]
    private GameObject _explosionPrefab;

    private GameObject _targetPowerup;
    private bool _hasFired = false;
    private AudioSource _audioSource;
    private Player _player;

    void Start()
    {
       
        _player = GameObject.Find("Player")?.GetComponent<Player>();
        

       
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            Debug.LogWarning("IntelEnemy: AudioSource missing, adding one");
            _audioSource = gameObject.AddComponent<AudioSource>();
        }
        _audioSource.clip = _explosionClip;
        _audioSource.playOnAwake = false;

        // Find closest powerup
        _targetPowerup = FindClosestPowerup();
        if (_targetPowerup != null)
        {
            Debug.Log($"IntelEnemy: Found powerup at {_targetPowerup.transform.position}");
        }
        else
        {
            Debug.Log("IntelEnemy: No powerup found, will not fire");
        }
    }

    void Update()
    {
     
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

     
        if (!_hasFired && _targetPowerup != null)
        {
            FireLaserAtPowerup();
            _hasFired = true;
        }

   
        if (transform.position.y < -8f)
        {
            Destroy(gameObject);
        }
    }

    private GameObject FindClosestPowerup()
    {
        GameObject[] powerups = GameObject.FindGameObjectsWithTag("Powerup");
        if (powerups.Length == 0)
            return null;

        GameObject closest = null;
        float minDist = float.MaxValue;
        foreach (var p in powerups)
        {
            float dist = Vector3.Distance(transform.position, p.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = p;
            }
        }
        return closest;
    }

    private void FireLaserAtPowerup()
    {
        Vector3 noseOffset = transform.up * 2.0f;
        Vector3 spawnPos = transform.position + noseOffset;
        GameObject laserObj = Instantiate(_laserPrefab, spawnPos, Quaternion.identity);
        
        
        IntelLaser laser = laserObj.GetComponent<IntelLaser>();
        if (laser != null)
        {
            laser.SetTarget(_targetPowerup.transform);
        }
        else
        {
            Debug.LogError("IntelEnemy: IntelLaser component missing on _laserPrefab");
        }

        
        Collider2D enemyCollider = GetComponent<Collider2D>();
        Collider2D laserCollider = laserObj.GetComponent<Collider2D>();
        if (enemyCollider != null && laserCollider != null)
        {
            Physics2D.IgnoreCollision(enemyCollider, laserCollider);
          
        }
        else
        {
       
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Powerup"))
        {
            return;
        }

        if (other.CompareTag("Laser"))
        {
            
            IntelLaser laser = other.GetComponent<IntelLaser>();
            if (laser == null) 
            {
                _lives--;
               
                Destroy(other.gameObject);
                if (_lives <= 0)
                {
                    DestroyEnemy();
                }
            }
           
        }
        else if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.Damage();
            }
            _lives--;

            if (_lives <= 0)
            {
                DestroyEnemy();
            }
        }
        else
        {
            
        }
    }

    private void DestroyEnemy()
    {
        if (_player != null)
        {
            _player.AddScore(10);
        }
        if (_explosionPrefab != null)
        {
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
        }
        if (_audioSource != null && _explosionClip != null)
        {
            _audioSource.PlayOneShot(_explosionClip);
        }
        Destroy(gameObject);
    }
}