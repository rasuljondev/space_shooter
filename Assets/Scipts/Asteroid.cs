using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField]
    private float _rotateSpeed = 15f; // Speed of the asteroid
    // Start is called before the first frame update
    [SerializeField]
    private GameObject _explosionPrefab; // Prefab for the explosion effect
    [SerializeField]
    private SpawnManager _spawnManager;

    
    private void Start()
    {
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>(); // Find the SpawnManager in the scene
       if (_spawnManager == null)
        {
            Debug.LogError("SpawnManager is NULL.");
        }
    }



    // Update is called once per frame
    void Update()
    {
        // rotate the asteroid on the z-axis
        transform.Rotate(Vector3.forward * _rotateSpeed * Time.deltaTime);

    }
   
     private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Laser")
        {
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity); // Instantiate explosion effect
            Destroy(other.gameObject); 
            _spawnManager.StartSpawning(); 
            Destroy(this.gameObject,0.25f); 
            
        }
       
    }
}
