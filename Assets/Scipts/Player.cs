using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3.5f;
    [SerializeField]
    private float _speedMultiplier = 2f;
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _tripleShotPrefab;
    private float _fireRate = 0.5f;
    private float _canFire = -1f;
    [SerializeField]
    private int _lives = 3;
    private SpawnManager _spawnManager;

    private bool _isTripleShotActive = false;
    private bool _isShieldActive = false;
    public bool _isSpeedBoostActive = false;


    private int _shieldHits = 0; //Shield hits

    [SerializeField]
    private int _maxAmmo = 15; //AMMO
    private int _currentAmmo;
    [SerializeField]
    private AudioClip _noAmmoSoundClip;

    [SerializeField]
    private GameObject _shieldVisualizer;
    [SerializeField]
    private GameObject _rightEngine;
    [SerializeField]
    private GameObject _leftEngine;
    [SerializeField]
    private int _score = 0;
    [SerializeField]
    private UIManager _uiManager;
    [SerializeField]
    private AudioClip _laserSoundClip;
    private AudioSource _audioSource;


    [SerializeField]
    private GameObject _rocketShotPrefab; 
    private bool _isRocketShotActive = false;

    private bool _hasFiredRocket = false;

    private CameraShake _cameraShake;

    public delegate void LevelUpAction(int newLevel);
    public static event LevelUpAction OnLevelUp;
    private int _level = 1;


    [SerializeField]
    private Slider _fuelSlider; 
 
    [SerializeField]
    private float _maxFuel = 100f;
    private float _currentFuel;
    [SerializeField]
    private float _boostDrainRate = 20f; 
    [SerializeField]
    private float _regenRate = 20f;


    [SerializeField]
    private GameObject _missilePrefab; 
    private bool _hasMissilePowerup = false;
    void Start()
    {
        _cameraShake = Camera.main.GetComponent<CameraShake>();

        //AMMO 15
        _currentAmmo = _maxAmmo;

        transform.position = new Vector3(0, 0, 0);
        _spawnManager = GameObject.Find("Spawn_Manager")?.GetComponent<SpawnManager>();
        _uiManager = GameObject.Find("Canvas")?.GetComponent<UIManager>();
        _audioSource = GetComponent<AudioSource>();

        if (_spawnManager == null)
        {
            Debug.LogError("The Spawn Manager is NULL.");
        }
        
        if (_uiManager == null)
        {
            Debug.LogError("The UI Manager is NULL.");
        }

        if (_audioSource == null)
        {
            Debug.LogError("Audio Source is NULL.");
        }
        else
        {
            _audioSource.clip = _laserSoundClip;
        }
        _currentFuel = _maxFuel;
        if (_fuelSlider != null)
        {
            _fuelSlider.maxValue = _maxFuel;
            _fuelSlider.value = _currentFuel;
        }
        else
        {
            Debug.LogError("Fuel Slider is NULL.");
        }
    }

    void Update()
    {
        CalculateMovement();

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
        {
            FireLaser();
        }

        // Fire missile with 'M' key if powerup is active
        if (_hasMissilePowerup && Input.GetKeyDown(KeyCode.M))
        {
            FireMissile();
        }
    }


    public void GrantMissilePowerup()
    {
        _hasMissilePowerup = true;
    }
    private void FireMissile()
    {
        Instantiate(_missilePrefab, transform.position, Quaternion.identity);
        _hasMissilePowerup = false; // Single use
    }

    public void QuickBoostPowerupOn()
    {
        _isSpeedBoostActive = true; 
        float originalSpeed = _speed; 
        _speed *= 0.5f; 
        StartCoroutine(QuickBoostPowerDownRoutine(originalSpeed));
    }

    IEnumerator QuickBoostPowerDownRoutine(float originalSpeed)
    {
        yield return new WaitForSeconds(3f); // 3-second duration
        _isSpeedBoostActive = false;
        _speed = originalSpeed; // Restore original speed
    }

    public void RocketShotPowerupOn()
    {
        _isRocketShotActive = true;
        _hasFiredRocket = false;
        StartCoroutine(RocketShotPowerDownRoutine());
    }

    IEnumerator RocketShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(5f);
        _isRocketShotActive = false;
    }

    public void AddLife()
    {
        _lives++;
        if (_lives > 3) // Assuming 3 is the max number of lives
        {
            _lives = 3;
        }

        // Reset engine visuals based on lives
        if (_lives == 3)
        {
            _rightEngine.SetActive(false);
            _leftEngine.SetActive(false);
        }
        else if (_lives == 2)
        {
            _rightEngine.SetActive(true);
            _leftEngine.SetActive(false);
        }
        else if (_lives == 1)
        {
            _rightEngine.SetActive(true);
            _leftEngine.SetActive(true);
        }

        _uiManager.UpdateLives(_lives);
    }



    public void RefillAmmo()
    {
        _currentAmmo = _maxAmmo;
        _uiManager.UpdateAmmo(_currentAmmo, _maxAmmo);

    }

    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // We are checking the SHIFT key
        float currentSpeed = _speed;
        bool isBoosting = (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && _currentFuel > 0f;

        if (isBoosting)
        {
            currentSpeed *= _speedMultiplier; // We are boosting speed here
            _currentFuel -= _boostDrainRate * Time.deltaTime;
            if (_currentFuel < 0f) _currentFuel = 0f;
        }
        else if (_currentFuel < _maxFuel)
        {
            _currentFuel += _regenRate * Time.deltaTime;
            if (_currentFuel > _maxFuel) _currentFuel = _maxFuel;
        }

        // Update the fuel slider
        if (_fuelSlider != null)
        {
            _fuelSlider.value = _currentFuel;
        }

        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);
        transform.Translate(direction * currentSpeed * Time.deltaTime);

        if (transform.position.y >= 0)
        {
            transform.position = new Vector3(transform.position.x, 0, 0);
        }
        else if (transform.position.y <= -3.8f)
        {
            transform.position = new Vector3(transform.position.x, -3.8f, 0);
        }

        if (transform.position.x >= 11)
        {
            transform.position = new Vector3(-11, transform.position.y, 0);
        }
        else if (transform.position.x <= -11)
        {
            transform.position = new Vector3(11, transform.position.y, 0);
        }
    }

    void FireLaser()
    {
        // If rocket powerup is active and not yet fired
        if (_isRocketShotActive && !_hasFiredRocket)
        {
            if (_currentAmmo <= 0)
            {
                if (_noAmmoSoundClip != null)
                    _audioSource.PlayOneShot(_noAmmoSoundClip);
                return;
            }

            _canFire = Time.time + 3f; // 3 seconds cooldown after rocket
            _currentAmmo--;
            Instantiate(_rocketShotPrefab, transform.position, Quaternion.identity);
            _audioSource.Play();
            _uiManager.UpdateAmmo(_currentAmmo, _maxAmmo);

            _hasFiredRocket = true; // Only one rocket per powerup
            return;
        }

        // Prevent firing during 3s cooldown after rocket
        if (Time.time < _canFire)
            return;

        if (_isTripleShotActive)
        {
            if (_currentAmmo < 3)
            {
                if (_noAmmoSoundClip != null)
                    _audioSource.PlayOneShot(_noAmmoSoundClip);
                return;
            }

            _canFire = Time.time + _fireRate;
            _currentAmmo -= 3;
            Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);
            _audioSource.Play();
        }
        else
        {
            if (_currentAmmo <= 0)
            {
                if (_noAmmoSoundClip != null)
                    _audioSource.PlayOneShot(_noAmmoSoundClip);
                return;
            }

            _canFire = Time.time + _fireRate;
            _currentAmmo--;
            Instantiate(_laserPrefab, transform.position + new Vector3(0, 1.05f, 0), Quaternion.identity);
            _audioSource.Play();
        }

        _uiManager.UpdateAmmo(_currentAmmo, _maxAmmo);
    }




    public void Damage()
    {
        if (_isShieldActive)
        {
            _shieldHits--;
            UpdateShieldColor(); // Update shield color after hit
            if (_shieldHits <= 0)
            {
                _isShieldActive = false;
                _shieldVisualizer.SetActive(false);
                StopCoroutine("ShieldsPowerDownRoutine"); // Stop timer if shield is broken by hits
            }
            return;
        }

        // Camera shake on actual damage
        if (_cameraShake != null)
        {
            StartCoroutine(_cameraShake.Shake(0.15f, 0.15f)); // Subtle shake
        }

        _lives--;
        if (_lives == 2)
        {
            _rightEngine.SetActive(true);
        }
        else if (_lives == 1)
        {
            _leftEngine.SetActive(true);
        }

        _uiManager.UpdateLives(_lives);

        if (_lives < 1)
        {
            _spawnManager?.OnPlayerDeath();
            Destroy(this.gameObject);
        }
    }




    public void TripleShotPowerupOn()
    {
        _isTripleShotActive = true;
        StartCoroutine(TripleShotPowerDownRoutine());
    }

    IEnumerator TripleShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(5f);
        _isTripleShotActive = false;
    }

    public void SpeedBoostPowerupOn()
    {
        _isSpeedBoostActive = true;
        _speed *= _speedMultiplier;
        StartCoroutine(SpeedBoostPowerDownRoutine());
    }

    IEnumerator SpeedBoostPowerDownRoutine()
    {
        yield return new WaitForSeconds(5f);
        _isSpeedBoostActive = false;
        _speed /= _speedMultiplier;
    }

    private void UpdateShieldColor()
    {
        if (_shieldVisualizer == null) return;

        Color color;
        switch (_shieldHits)
        {
            case 3:
                color = Color.cyan; // Full strength
                break;
            case 2:
                color = Color.yellow; // Medium strength
                break;
            case 1:
                color = Color.red; // Low strength
                break;
            default:
                color = Color.clear; // Invisible or off
                break;
        }

        // For SpriteRenderer (2D object)
        var spriteRenderer = _shieldVisualizer.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = color;
        }
    }

    public void ShieldsPowerupOn()
    {
        _isShieldActive = true;
        _shieldHits = 3; // Shield can take 3 hits
        _shieldVisualizer.SetActive(true);
        UpdateShieldColor(); // Set initial color
        StopCoroutine("ShieldsPowerDownRoutine"); // Stop any running shield timer
        StartCoroutine(ShieldsPowerDownRoutine());
    }


    IEnumerator ShieldsPowerDownRoutine()
    {
        yield return new WaitForSeconds(5f);
        if (_isShieldActive) // Only turn off if still active (not already broken by hits)
        {
            _isShieldActive = false;
            _shieldVisualizer.SetActive(false);
        }
    }

    public void AddScore(int points)
    {
        _score += points;
        _uiManager.UpdateScore(_score);

        if (_level == 1 && _score >= 50)
        {
            _level = 2;
            if (OnLevelUp != null)
                OnLevelUp(_level);
            _uiManager.ShowLevelMessage("LVL 2 Started");
        }
        else if (_level == 2 && _score >= 100)
        {
            _level = 3;
            if (OnLevelUp != null)
                OnLevelUp(_level);
            _uiManager.ShowLevelMessage("LVL 3 Started");
        }
    }

}
