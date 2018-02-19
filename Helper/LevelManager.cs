using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AiryCat.Utilities.Helper
{
    public class LevelManager : MonoBehaviour
    {

        public void LoadLevel(int value)
        {
            SceneManager.LoadScene(value == -1 ? SceneManager.GetActiveScene().buildIndex : value);
        }

        public void ExitGame()
        {
            Application.Quit();
        }
    }
}