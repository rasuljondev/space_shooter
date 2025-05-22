using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyPrefab;
    [SerializeField]
    private GameObject _enemyContainer;
    private bool _stopSpawning = false;
    [SerializeField]
    private GameObject[] _powerups;
    private int _currentLevel = 1;
    private float _enemySpawnRate = 5.0f;
    [SerializeField]
    private GameObject[] _enemyPrefabs;
    private bool _bossSpawned = false;
    [SerializeField]
    private GameObject _bossPrefab;

    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerupRoutine());
    }

    IEnumerator SpawnEnemyRoutine()
    {
        yield return new WaitForSeconds(3.0f);

        while (!_stopSpawning && !_bossSpawned)
        {
            Vector3 spawnPosition = new Vector3(Random.Range(-8f, 8f), 7, 0);
            int randomEnemy = Random.Range(0, _enemyPrefabs.Length);
            GameObject newEnemy = Instantiate(_enemyPrefabs[randomEnemy], spawnPosition, Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;
            yield return new WaitForSeconds(_enemySpawnRate);
        }
    }

    IEnumerator SpawnPowerupRoutine()
    {
        yield return new WaitForSeconds(3.0f);

        while (!_stopSpawning)
        {
            Vector3 spawnPosition = new Vector3(Random.Range(-8f, 8f), 7, 0);
            int randomValue = Random.Range(0, 100);
            int randomPowerup;

            if (randomValue < 9) // 0-8: 9%
                randomPowerup = 0; // triple
            else if (randomValue < 14) // 9-13: 5%
                randomPowerup = 1; // speed
            else if (randomValue < 39) // 14-38: 25%
                randomPowerup = 2; // shield
            else if (randomValue < 76) // 39-75: 37%
                randomPowerup = 3; // ammo
            else if (randomValue < 81) // 76-80: 5%
                randomPowerup = 4; // health
            else if (randomValue < 86) // 81-85: 5%
                randomPowerup = 5; // negative
            else if (randomValue < 95) // 86-94: 9%
                randomPowerup = 6; // rocket
            else // 95-99: 5%
                randomPowerup = 7; // missile

            if (randomPowerup >= 0 && randomPowerup < _powerups.Length)
            {
                GameObject newPowerup = Instantiate(_powerups[randomPowerup], spawnPosition, Quaternion.identity);
                yield return new WaitForSeconds(Random.Range(3f, 7f));
            }
            else
            {
                Debug.LogWarning($"Invalid powerup index: {randomPowerup}. Check _powerups array length.");
            }
        }


    }

    void OnEnable()
    {
        Player.OnLevelUp += HandleLevelUp;
    }

    void OnDisable()
    {
        Player.OnLevelUp -= HandleLevelUp;
    }

    private void HandleLevelUp(int newLevel)
    {
        _currentLevel = newLevel;
        if (_currentLevel == 2)
        {
            _enemySpawnRate = 4f;
            StartCoroutine(Level2Sequence());
        }
        else if (_currentLevel == 3 && !_bossSpawned)
        {
            _stopSpawning = true;
            StartCoroutine(Level3Sequence());
            _bossSpawned = true;
        }
    }

    private IEnumerator Level3Sequence()
    {

        yield return new WaitForSeconds(2f);
        // Spawn 5 
        int waveCount = 5;
        for (int i = 0; i < waveCount; i++)
        {
            Vector3 spawnPosition = new Vector3(Random.Range(-8f, 8f), 7, 0);
            int randomEnemy = Random.Range(0, _enemyPrefabs.Length);
            GameObject newEnemy = Instantiate(_enemyPrefabs[randomEnemy], spawnPosition, Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;
            yield return new WaitForSeconds(0.5f);
        }

        yield return new WaitForSeconds(2f);

        // Spawn boss
        Vector3 bossSpawnPos = new Vector3(0, 7, 0);
        GameObject boss = Instantiate(_bossPrefab, bossSpawnPos, Quaternion.identity);
        boss.transform.parent = _enemyContainer.transform;
        Debug.Log("SpawnManager: Boss spawned.");
    }
    private IEnumerator Level2Sequence()
    {

        yield return new WaitForSeconds(2f);
        // Spawn 5 
        int waveCount = 5;
        for (int i = 0; i < waveCount; i++)
        {
            Vector3 spawnPosition = new Vector3(Random.Range(-8f, 8f), 7, 0);
            int randomEnemy = Random.Range(0, _enemyPrefabs.Length);
            GameObject newEnemy = Instantiate(_enemyPrefabs[randomEnemy], spawnPosition, Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;
            yield return new WaitForSeconds(0.5f);
        }

      
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }
}