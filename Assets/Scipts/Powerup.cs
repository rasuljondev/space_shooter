using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3f;
    [SerializeField]
    private int _powerupID;
    [SerializeField]
    private AudioClip _clip;

    [SerializeField]
    private float _homingSpeed = 3f; 

    private bool _isHoming = false;
    private Transform _player;

    void Start()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
            _player = playerObj.transform;
    }

    void Update()
    {
        
        if (!_isHoming && Input.GetKeyDown(KeyCode.C) && _player != null)
        {
            _isHoming = true;
        }

        if (_isHoming && _player != null)
        {
            
            Vector3 direction = (_player.position - transform.position).normalized;
            transform.position += direction * _homingSpeed * Time.deltaTime;
        }
        else
        {
            
            transform.Translate(Vector3.down * _speed * Time.deltaTime);
        }

        if (transform.position.y < -6f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.transform.GetComponent<Player>();
            AudioSource.PlayClipAtPoint(_clip, transform.position);

            if (player != null)
            {
                switch (_powerupID)
                {
                    case 0:
                        player.TripleShotPowerupOn();
                        break;
                    case 1:
                        player.SpeedBoostPowerupOn();
                        break;
                    case 2:
                        player.ShieldsPowerupOn();
                        break;
                    case 3:
                        player.RefillAmmo();
                        break;
                    case 4:
                        player.AddLife(); 
                        break;
                    case 5:
                        player.QuickBoostPowerupOn();
                        break;
                    case 6:
                        player.RocketShotPowerupOn();
                        break;
                    case 7:
                        player.GrantMissilePowerup();
                        break;
                    default:
                        Debug.Log("Invalid Powerup ID");
                        break;
                }
            }

            Destroy(gameObject);
        }
    }
}
