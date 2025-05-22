using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{

    [SerializeField]
    private Text _ammoText; //AMMO TEXT

    [SerializeField]
    private Text _scoreText;
    [SerializeField]
    private Image _livesImage;
    [SerializeField]
    private Sprite[] _liveSprites;
    [SerializeField]
    private Text _gameOverText;
    [SerializeField]
    private Text _restartText;
    [SerializeField]
    private GameManager _gameManager;


   
   
    void Start()
    {

       _scoreText.text = "Score: " + 0;
      _gameOverText.gameObject.SetActive(false);
      _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
      if (_gameManager == null)
      {
          Debug.LogError("GameManager is NULL.");
      }

    }
    public void UpdateScore(int playerScore)
    {
        _scoreText.text = "Score: " + playerScore.ToString();
    }
    //UPDATING AMMO
    public void UpdateAmmo(int currentAmmo, int maxAmmo)
    {
        _ammoText.text = "Ammo: " + currentAmmo;
    }

    public void UpdateLives(int currentLives)
    {
        if (currentLives < 0)
        {
            return;
        }
        _livesImage.sprite = _liveSprites[currentLives];

        if (currentLives == 0)
        {
            GameOverSequence(); 
        }
       
    }

    void GameOverSequence()
    {
        _gameManager.GameOver(); 
        _gameOverText.gameObject.SetActive(true);
        _restartText.gameObject.SetActive(true);
        StartCoroutine(GameOverFlickerRoutine());
    }
  IEnumerator GameOverFlickerRoutine()
  {
    while (true)
    {
       _gameOverText.text = "GAME OVER";
       yield return new WaitForSeconds(0.5f);
       _gameOverText.text = "";
         yield return new WaitForSeconds(0.5f);
    }
  }
}
