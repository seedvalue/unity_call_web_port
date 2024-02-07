//using GamePush;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _0_WebPort
{

    public class TextLocalize : MonoBehaviour
    {
        [TextArea]
        [SerializeField]
        private string textEng;
        [TextArea]
        [SerializeField]
        private string textRu;

        private TMP_Text _targetText;
        private Text _targetTextOld;

        void Start()
        {
            //var lang = GP_Language.Current();
            Debug.LogError("Localisation other SDK");
            //Debug.Log("GP_Language = " + lang.ToString());
            _targetText = GetComponent<TMP_Text>();
            _targetTextOld = GetComponent<Text>();
            SetText();
        }
        
        private void SetText()
        {
            /*
            var lang = GP_Language.Current();
            if (_targetText != null)
            {
                _targetText.text = lang switch
                {
                    Language.English => textEng,
                    Language.Russian => textRu,
                    _ => textEng
                };
            }
            if (_targetTextOld != null)
            {
                _targetTextOld.text = lang switch
                {
                    Language.English => textEng,
                    Language.Russian => textRu,
                    _ => textEng
                };
            }
            */
        }
    }
}

