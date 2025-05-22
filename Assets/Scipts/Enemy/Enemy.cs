using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Base Enemy class. Inherit from this for custom enemy behaviors.
public class Enemy : MonoBehaviour
{
    // Enum for movement types. Set in Inspector or by derived classes.
    public enum MovementType { Down, SideToSide, Angle }
    [SerializeField]
    protected MovementType _movementType = MovementType.Down;

    [SerializeField]
    protected float _speed = 3.0f;
    [SerializeField]
    protected GameObject _laserPrefab;

    [SerializeField]
    protected Player _player;
    [SerializeField]
    protected Animator _animator;
    protected AudioSource _audioSource;
    protected float _fireRate = 2.0f;
    protected float _canFire = -1f;
    protected bool _isDestroyed = false; // Track destruction state

    // Side-to-side movement settings
    [Header("Side-to-Side Settings")]
    [SerializeField]
    protected float _sideAmplitude = 3f;
    [SerializeField]
    protected float _sideFrequency = 2f;
    protected float _startX;

    // Angle movement settings
    [Header("Angle Settings")]
    [SerializeField]
    protected float _angle = 30f; // degrees

    protected virtual void Start()
    {
        _player = GameObject.Find("Player")?.GetComponent<Player>();
        _audioSource = GetComponent<AudioSource>();
        if (_player == null)
        {
            Debug.LogError("Enemy: Player is NULL.");
        }
        _animator = GetComponent<Animator>();
        if (_animator == null)
        {
            Debug.LogError("Enemy: Animator is NULL.");
        }

        _startX = transform.position.x;
    }

    protected virtual void Update()
    {
        if (_isDestroyed)
        {
            return; // Skip movement and firing if destroyed
        }

        CalculateMovement();
        if (Time.time > _canFire)
        {
            FireLaser();
        }
    }

    protected virtual void FireLaser()
    {
        _fireRate = Random.Range(3.0f, 7.0f);
        _canFire = Time.time + _fireRate;
        GameObject enemyLaser = Instantiate(_laserPrefab, transform.position, Quaternion.identity);
        Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();
        for (int i = 0; i < lasers.Length; i++)
        {
            lasers[i].AssignEnemyLaser();
        }
        Debug.Log($"Enemy {gameObject.name}: Fired laser at {transform.position}");
    }

    protected virtual void CalculateMovement()
    {
        switch (_movementType)
        {
            case MovementType.Down:
                MoveDown();
                break;
            case MovementType.SideToSide:
                SideToSideMove();
                break;
            case MovementType.Angle:
                AngleMove();
                break;
        }
    }

    protected void MoveDown()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
        if (transform.position.y < -5.0f)
        {
            float randomX = Random.Range(-8f, 8f);
            transform.position = new Vector3(randomX, 7, 0);
        }
    }

    protected void SideToSideMove()
    {
        float newX = _startX + Mathf.Sin(Time.time * _sideFrequency) * _sideAmplitude;
        transform.position += Vector3.down * _speed * Time.deltaTime;
        transform.position = new Vector3(newX, transform.position.y, 0);

        if (transform.position.y < -5.0f)
        {
            float randomX = Random.Range(-8f, 8f);
            _startX = randomX;
            transform.position = new Vector3(randomX, 7, 0);
        }
    }

    protected void AngleMove()
    {
        float rad = _angle * Mathf.Deg2Rad;
        Vector3 direction = new Vector3(Mathf.Sin(rad), -Mathf.Cos(rad), 0);
        transform.position += direction * _speed * Time.deltaTime;

        if (transform.position.y < -5.0f || Mathf.Abs(transform.position.x) > 10f)
        {
            float randomX = Random.Range(-8f, 8f);
            transform.position = new Vector3(randomX, 7, 0);
        }
    }

    protected void DestroyEnemy()
    {
        _isDestroyed = true; // Mark as destroyed
        if (_animator != null)
        {
            _animator.SetTrigger("OnEnemyDeath");
        }
        else
        {
            Debug.LogWarning($"Enemy {gameObject.name}: Animator is null, skipping death animation.");
        }
        _speed = 0;
        GetComponent<Collider2D>().enabled = false;
        if (_audioSource != null && _audioSource.enabled && _audioSource.clip != null)
        {
            _audioSource.Play();
        }
        else
        {
            Debug.LogWarning($"Enemy {gameObject.name}: AudioSource is disabled or has no clip, skipping sound.");
        }
        Debug.Log($"Enemy {gameObject.name}: Destroyed, waiting 2.8s for animation");
        Destroy(this.gameObject, 2.8f);
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.Damage();
            }
            Debug.Log($"Enemy {gameObject.name}: Collided with player");
            DestroyEnemy();
        }
        else if (other.CompareTag("Laser"))
        {
            Laser laser = other.GetComponent<Laser>();
            if (laser != null && !laser.IsEnemyLaser)
            {
                if (_player != null)
                {
                    _player.AddScore(10);
                }
                Destroy(other.gameObject);
                Debug.Log($"Enemy {gameObject.name}: Hit by player laser");
                DestroyEnemy();
            }
            else
            {
                Debug.Log($"Enemy {gameObject.name}: Ignored enemy laser from {other.gameObject.name}");
            }
        }
        else
        {
            Debug.Log($"Enemy {gameObject.name}: Ignored collision with {other.tag} at {other.transform.position}");
        }
    }
}