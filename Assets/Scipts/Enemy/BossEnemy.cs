using UnityEngine;

public class BossEnemy : MonoBehaviour
{
    [SerializeField]
    private float _moveSpeed = 2f;
    [SerializeField]
    private float _moveToY = 6f;
    [SerializeField]
    private int _lives = 10;
    [SerializeField]
    private GameObject _bossShotPrefab;
    [SerializeField]
    private float _fireRate = 3f;
    [SerializeField]
    private float _spreadShotAngle = 30f;
    [SerializeField]
    private int _spreadShotCount = 5;
    [SerializeField]
    private AudioClip _fireSoundClip;
    [SerializeField]
    private AudioClip _deathSoundClip;
    [SerializeField]
    private GameObject _deathEffectPrefab;
    [SerializeField]
    private Vector3 _shotOffset = new Vector3(0, -2f, 0);

    private bool _isPositioned = false;
    private float _canFire = -1f;
    private Player _player;
    private AudioSource _audioSource;

    private void Start()
    {
        transform.position = new Vector3(0, 8f, 0);
        _player = GameObject.Find("Player")?.GetComponent<Player>();
        if (_player == null)
        {
            Debug.LogWarning("BossEnemy: Player not found.");
        }

        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            Debug.LogWarning("BossEnemy: AudioSource not found, adding one.");
            _audioSource = gameObject.AddComponent<AudioSource>();
        }
        _audioSource.playOnAwake = false;
        _audioSource.enabled = true;

        var spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red;
            spriteRenderer.sortingLayerName = "Default";
            spriteRenderer.sortingOrder = 1;
            Debug.Log($"BossEnemy: SpriteRenderer set to layer {spriteRenderer.sortingLayerName}, order {spriteRenderer.sortingOrder}");
        }
        else
        {
            Debug.LogWarning("BossEnemy: SpriteRenderer not found.");
        }

        if (_bossShotPrefab == null)
        {
            Debug.LogError("BossEnemy: _bossShotPrefab is not assigned.");
        }
        else
        {
            var shotRenderer = _bossShotPrefab.GetComponent<SpriteRenderer>();
            if (shotRenderer != null)
            {
                shotRenderer.sortingLayerName = "Default";
                shotRenderer.sortingOrder = 3;
                Debug.Log($"BossShot: SpriteRenderer set to layer {shotRenderer.sortingLayerName}, order {shotRenderer.sortingOrder}");
            }
            else
            {
                Debug.LogError("BossEnemy: _bossShotPrefab missing SpriteRenderer.");
            }
        }

        _isPositioned = false;
        _canFire = Time.time + 1f;
        Debug.Log($"BossEnemy: _spreadShotCount set to {_spreadShotCount}");
    }

    private void Update()
    {
        if (!_isPositioned)
        {
            MoveToMiddle();
        }
        else if (Time.time > _canFire)
        {
            FireSpreadShot();
        }
    }

    private void MoveToMiddle()
    {
        Vector3 targetPosition = new Vector3(0, _moveToY, 0);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, _moveSpeed * Time.deltaTime);
        if (Mathf.Abs(transform.position.y - _moveToY) < 0.1f)
        {
            _isPositioned = true;
            transform.position = targetPosition;
            Debug.Log("BossEnemy: Reached middle position.");
        }
    }

    private void FireSpreadShot()
    {
        _canFire = Time.time + _fireRate;
        Debug.Log($"BossEnemy: Firing {_spreadShotCount} spread shots at {Time.time}");

        float startAngle = -_spreadShotAngle / 2;
        float angleStep = _spreadShotAngle / (_spreadShotCount - 1);
        for (int i = 0; i < _spreadShotCount; i++)
        {
            float angle = startAngle + (i * angleStep);
            Quaternion rotation = Quaternion.Euler(0, 0, angle);
            Vector3 spawnPos = transform.position + _shotOffset;
            GameObject bossShot = Instantiate(_bossShotPrefab, spawnPos, rotation);
            Laser laser = bossShot.GetComponent<Laser>();
            if (laser != null)
            {
                laser.AssignEnemyLaser();
                var shotRenderer = bossShot.GetComponent<SpriteRenderer>();
                if (shotRenderer != null)
                {
                    shotRenderer.sortingOrder = 3;
                }
                Debug.Log($"BossEnemy: Spawned shot {i} at angle {angle}, position {spawnPos}, order {shotRenderer?.sortingOrder}");
            }
            else
            {
                Debug.LogError("BossEnemy: _bossShotPrefab missing Laser component.");
            }
        }

        if (_audioSource != null && _fireSoundClip != null && _audioSource.enabled)
        {
            _audioSource.PlayOneShot(_fireSoundClip);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Laser") && other.GetComponent<Laser>()?.IsEnemyLaser == false)
        {
            _lives--;
            Debug.Log($"BossEnemy: Hit by player laser, lives remaining: {_lives}");
            Destroy(other.gameObject);
            if (_lives <= 0)
            {
                if (_player != null)
                {
                    _player.AddScore(50);
                }
                DestroyBoss();
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
            Debug.Log($"BossEnemy: Collided with player, lives remaining: {_lives}");
            if (_lives <= 0)
            {
                if (_player != null)
                {
                    _player.AddScore(50);
                }
                DestroyBoss();
            }
        }
        else if (other.CompareTag("Laser") && other.GetComponent<Laser>()?.IsEnemyLaser == true)
        {
            Debug.Log($"BossEnemy: Ignored own laser at position {other.transform.position}");
            return;
        }
    }

    private void DestroyBoss()
    {
        if (_deathEffectPrefab != null)
        {
            Instantiate(_deathEffectPrefab, transform.position, Quaternion.identity);
        }
        if (_audioSource != null && _deathSoundClip != null && _audioSource.enabled)
        {
            _audioSource.PlayOneShot(_deathSoundClip);
        }
        Destroy(gameObject);
    }
}