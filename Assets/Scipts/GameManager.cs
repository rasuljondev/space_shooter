using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class GameManager : MonoBehaviour
{
   [SerializeField]
   private bool _isGameOver;

   public void Update()
   {
    if (_isGameOver == true)
    {
        if (Input.GetKeyDown(KeyCode.R) && _isGameOver == true)
        {
            SceneManager.LoadScene(1); // Current Game Scene
        }
    }
    // if the escape key is pressed, quit the game
    if (Input.GetKeyDown(KeyCode.Escape))
    {
        Application.Quit();
        Debug.Log("Game is exiting...");
    }
   }

   public void GameOver()
   {
       _isGameOver = true;
   }




}
