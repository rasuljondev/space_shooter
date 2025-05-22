using UnityEngine;

public class ShieldedEnemy : Enemy
{
    [SerializeField]
    private GameObject _shieldVisualizer; 
    private bool _shieldActive = true;

    protected override void Start()
    {
        base.Start();
        if (_shieldVisualizer != null)
            _shieldVisualizer.SetActive(true);
        _shieldActive = true;
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Laser")
        {
            Destroy(other.gameObject);
            if (_shieldActive)
            {
                _shieldActive = false;
                if (_shieldVisualizer != null)
                {
                   
                    _shieldVisualizer.SetActive(false);
                    
                }
                return;
            }
            else
            {
                if (_player != null)
                    _player.AddScore(10);
                DestroyEnemy();
            }
        }
        else
        {
            base.OnTriggerEnter2D(other);
        }
    }
}
