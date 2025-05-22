using UnityEngine;

public class SuperEnemy : Enemy
{
    private bool _isDodging = false;
    [SerializeField]
    private float _dodgeDistance = 3f; 
    [SerializeField]
    private float _detectionRangeX = 8f; 
    [SerializeField]
    private float _detectionRangeY = 8f; 
    [SerializeField]
    private float _dodgeCooldown = 1f;
    [SerializeField]
    private float _dodgeDuration = 0.1f; 
    private float _lastDodgeTime = -1f;
    private Vector3 _dodgeTarget;
    private Vector3 _dodgeStart;
    private float _dodgeProgress = 0f;

    protected override void Start()
    {
        base.Start();

       
        var spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;
        }
        else
        {
           
        }
    }

    protected override void Update()
    {
        
        if (_isDodging)
        {
            _dodgeProgress += Time.deltaTime / _dodgeDuration;
            transform.position = Vector3.Lerp(_dodgeStart, _dodgeTarget, _dodgeProgress);
            if (_dodgeProgress >= 1f)
            {
                _isDodging = false;
                _startX = transform.position.x; 
            }
        }
        else
        {
            base.Update(); 
        }

        
        if (!_isDodging && Time.time > _lastDodgeTime + _dodgeCooldown)
        {
            GameObject[] lasers = GameObject.FindGameObjectsWithTag("Laser");
            foreach (var laserObj in lasers)
            {
                Laser laserScript = laserObj.GetComponent<Laser>();
                if (laserScript != null && !laserScript.IsEnemyLaser)
                {
                    float distanceX = Mathf.Abs(laserObj.transform.position.x - transform.position.x);
                    float distanceY = laserObj.transform.position.y - transform.position.y;

                    
                    if (distanceY > 0 && distanceY < _detectionRangeY && distanceX < _detectionRangeX)
                    {
                        // Start dodge
                        float dodgeDir = Random.value > 0.5f ? 1f : -1f;
                        _dodgeStart = transform.position;
                        _dodgeTarget = _dodgeStart + new Vector3(dodgeDir * _dodgeDistance, 0, 0);
                        _dodgeTarget.x = Mathf.Clamp(_dodgeTarget.x, -11f, 11f); 
                        _isDodging = true;
                        _dodgeProgress = 0f;
                        _lastDodgeTime = Time.time;

                     
                        break;
                    }
                }
            }
        }
    }

   
    protected override void CalculateMovement()
    {
        if (!_isDodging)
        {
            base.CalculateMovement(); 
        }
    }

    
    public void ResetDodge()
    {
        _isDodging = false;
        _lastDodgeTime = -1f;
        _dodgeProgress = 0f;
    }
}