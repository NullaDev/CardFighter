using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneControl
{
    public class MainMenuControl : MonoBehaviour
    {
        public void StartGame()
        {
            SceneManager.LoadScene("ClassChoose");
        }
        
        public void About()
        {
            SceneManager.LoadScene("About");
        }
        
        public void ExitGame()
        {
            Application.Quit();
        }
    }
}