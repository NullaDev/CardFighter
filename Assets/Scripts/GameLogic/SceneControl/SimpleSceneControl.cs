using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameLogic.SceneControl
{
    public class SimpleSceneControl: MonoBehaviour
    {
        public void ReturnToMainMenu()
        {
            SceneManager.LoadScene("MainMenu");
        }
        
        public void ReturnToRogueMap()
        {
            SceneManager.LoadScene("RogueMap");
        }
    }
}