
using GameLogic.Option;
using GameLogic.Runtime;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Render
{
    public class OptionButtonRender : MonoBehaviour, IPointerClickHandler
    {
        private Text _title;
        private Text _text;
        private Option _option;

        public void Awake()
        {
            this._title = transform.Find("OptionTitle").GetComponent<Text>();
            this._text = transform.Find("OptionText").GetComponent<Text>();
        }

        public void SetOptionAndRender(Option option)
        {
            this._option = option;
            this._title.text = option.Title;
            this._text.text = option.Description;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if(this._option == null) return;
            
            var playerData = PlayerData.Instance;
            this._option.Actions.ForEach(a=>a.Execute(playerData));

            MiscData.Instance.OptionBundle = null;
            SceneManager.LoadScene(this._option.TargetSceneName);
        }
    }
}