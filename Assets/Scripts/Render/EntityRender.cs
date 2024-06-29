using Entity;
using UnityEngine;
using UnityEngine.UI;

namespace Render
{
    public class EntityRender : MonoBehaviour
    {
        public Image entityImage;
        public Image HPBack;
        public Image HPFront;
        public Text HPText;
        
        void Awake()
        {
            entityImage = transform.Find("EntityImage").GetComponent<Image>();

            var HPBar = transform.Find("HPBar").gameObject;
            if (HPBar != null)
            {
                HPBack = HPBar.transform.Find("HPBack").GetComponent<Image>();
                HPFront = HPBar.transform.Find("HPFront").GetComponent<Image>();
                HPText = HPBar.transform.Find("HPText").GetComponent<Text>();
            }
        }

        public void RenderEmpty()
        {
            entityImage.enabled = false;
            HPBack.enabled = false;
            HPFront.enabled = false;
            HPText.enabled = false;
        }

        public void Render(EntityBase entity)
        {
            entityImage.enabled = true;

            if (entity.TextureName != "")
            {
                var sprite = Resources.Load<Sprite>(entity.TextureName);
                entityImage.sprite = sprite;
            }

            RenderHPBar(entity);
        }
        
        public void RenderHPBar(EntityBase entity)
        {
            HPBack.enabled = true;
            HPFront.enabled = true;
            HPText.enabled = true;
            
            HPText.text = entity.HP + "/" + entity.MaxHP;
            var newWidth = HPBack.rectTransform.rect.width * entity.HP / entity.MaxHP;
            HPFront.rectTransform.sizeDelta = new Vector2(newWidth, HPFront.rectTransform.sizeDelta.y);
        }
    }
}