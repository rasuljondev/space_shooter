using System.Collections;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    [SerializeField]
    private float _speed = 8f;
    [SerializeField]
    private float _flyTime = 2f;
    [SerializeField]
    private GameObject _explosionPrefab; 

    private bool _hasExploded = false;

    void Start()
    {
        StartCoroutine(FlyAndExplodeRoutine());
    }

    void Update()
    {
        if (!_hasExploded)
        {
           
            transform.Translate(Vector3.up * _speed * Time.deltaTime);
        }
    }

    IEnumerator FlyAndExplodeRoutine()
    {
        yield return new WaitForSeconds(_flyTime);
        Explode();
    }

    private void Explode()
    {
        _hasExploded = true;

        
        if (_explosionPrefab != null)
        {
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
        }

        
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
        }

        
        Destroy(gameObject);
    }
}
