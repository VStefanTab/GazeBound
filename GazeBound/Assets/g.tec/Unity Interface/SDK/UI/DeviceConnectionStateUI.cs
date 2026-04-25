using UnityEngine;
using UnityEngine.UI;
using static Gtec.Chain.Common.Templates.DataAcquisitionUnit.DataAcquisitionUnit;

namespace Gtec.UnityInterface
{
    public class DeviceConnectionStateUI : MonoBehaviour
    {
        public Color ColorHighlighted;
        public Color ColorDefault;

        private Device _bci;
        private Image _img;
        void Start()
        {
            try
            {
                _bci = GetComponentInParent<Device>();
            }
            catch
            {
                _bci = null;
            }
            if (_bci != null)
            {
                _bci.OnDeviceStateChanged.AddListener(OnDeviceStateChanged);
            }

            try
            {
                Image[] images = this.GetComponentsInChildren<Image>();
                foreach (Image img in images)
                {
                    if (img.gameObject.name.Contains("img"))
                    {
                        _img = img;
                        break;
                    }
                }
            }
            catch
            {
                _img = null;
            }
        }

        public void OnDeviceStateChanged(States state)
        {
            if (state == States.Connected)
            {
                Texture2D tex = Resources.Load<Texture2D>("connected");
                _img.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                _img.color = ColorHighlighted;
                _img.preserveAspect = true;
            }

            if (state == States.Disconnected)
            {
                Texture2D tex = Resources.Load<Texture2D>("disconnected");
                _img.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                _img.color = ColorDefault;
                _img.preserveAspect = true;
            }
        }
    }
}