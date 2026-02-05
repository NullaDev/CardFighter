using System.Threading.Tasks;
using GameLogic.Runtime;
using Registry;
using Render;
using UnityEngine;
using UnityEngine.UI;

namespace SceneControl
{
    public class LoadingControl : MonoBehaviour
    {
        public Slider progressBar;
        public Text progressText;
        
        private async void Start()
        {
            await TextureCache.PreloadWithProgress(progressBar, progressText);
            StaticDataManager.LoadAll();
            await Task.Delay(500);

            MiscData.Instance.InitSeed();
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }
    }
}