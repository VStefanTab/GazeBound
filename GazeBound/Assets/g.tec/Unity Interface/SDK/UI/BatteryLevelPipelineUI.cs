using UnityEngine;
using UnityEngine.UI;

namespace Gtec.UnityInterface
{
    public class BatteryLevelPipelineUI : MonoBehaviour
    {
        private BatteryLevelPipeline _bci;
        private Image _img;
        void Start()
        {
            try
            {
                _bci = GetComponentInParent<BatteryLevelPipeline>();
            }
            catch
            {
                _bci = null;
            }
            if (_bci != null)
            {
                _bci.OnBatteryLevelAvailable.AddListener(OnBatteryLevelAvailable);
            }

            try
            {
                Image[] images = this.GetComponentsInChildren<Image>();
                foreach (Image img in images)
                {
                    if (img.gameObject.name.Contains("img"))
                    {
                        _img = img;
                        img.gameObject.SetActive(false);
                        break;
                    }
                }
            }
            catch
            {
                _img = null;
            }
        }

        private void OnBatteryLevelAvailable(float batteryLevel)
        {
            _img.gameObject.SetActive(true);
            Texture2D tex = null;
            if (batteryLevel > 60)
            {
                tex = Resources.Load<Texture2D>("battery_full");
            }
            else if (batteryLevel <= 60 && batteryLevel >= 13)
            {
                tex = Resources.Load<Texture2D>("battery_half");
            }
            else
            {
                tex = Resources.Load<Texture2D>("battery_empty");
            }
            _img.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            _img.preserveAspect = true;
        }
    }
}
