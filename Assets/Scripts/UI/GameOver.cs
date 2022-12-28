using UnityEngine;
using UnityEngine.SceneManagement;

namespace Kolman_Freecss.Krodun
{
    public class GameOver : MonoBehaviour
    {
        public void OnExitButtonPressed()
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}