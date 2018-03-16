using UnityEngine;
using UnityEngine.SceneManagement;

namespace AiryCat.UtilitiesForUnity.Helper
{
    public class LevelManager 
    {
        public void LoadLevel(int value)
        {
            SceneManager.LoadScene(value < 0 ? SceneManager.GetActiveScene().buildIndex : value);
        }

        public void ExitGame()
        {
            Application.Quit();
        }
    }
}