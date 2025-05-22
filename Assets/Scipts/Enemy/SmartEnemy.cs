using UnityEngine;

public class SmartEnemy : Enemy
{
    [SerializeField]
    private GameObject _backLaserPrefab;

    
    private float _backFireRate = 3.0f; 
    private float _canBackFire = -1f;

    protected override void Start()
    {
        base.Start();
        
        var spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red;
        }
    }

    protected override void FireLaser()
    {
        float thresholdY = Camera.main.ViewportToWorldPoint(new Vector3(0, 0.75f, 0)).y;
        
        if (_player != null && _player.transform.position.y < transform.position.y && transform.position.y < thresholdY && Time.time > _canBackFire)
        {
            Vector3 backSpawnPos = transform.position + new Vector3(0, -0.5f, 0);
            GameObject backLaser = Instantiate(_backLaserPrefab, backSpawnPos, Quaternion.identity);
            Laser laser = backLaser.GetComponent<Laser>();
            if (laser != null)
                laser.AssignEnemyLaser(true); 

            _backFireRate = Random.Range(2.5f, 4.5f); 
            _canBackFire = Time.time + _backFireRate;
        }
        
    }
}
