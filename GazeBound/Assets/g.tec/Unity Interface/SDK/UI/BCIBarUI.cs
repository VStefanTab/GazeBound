using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static Gtec.Chain.Common.Nodes.InputNodes.ChannelQuality;
using static Gtec.Chain.Common.Templates.DataAcquisitionUnit.DataAcquisitionUnit;

namespace Gtec.UnityInterface
{
    public class BCIBarUI : MonoBehaviour
    {
        public GameObject DeviceUI;

        public Color ChannelGood;
        public Color ChannelBad;

        public UnityEvent<string> OnConnect;
        public UnityEvent OnDisconnect;

        private TMP_Dropdown _ddDevices;
        private bool _connected;
        private Device _bci;
        private Button _btnConnect;
        private TextMeshProUGUI _btnText;

        private GameObject _areaDevice;
        private GameObject _areaSQ;
        private GameObject _areaBat;
        private GameObject _areaDataLost;
        private GameObject _areaConnectionState;
        private List<GameObject> _goChannels;
        private List<Image> _imgChannels;
        private static Color ColorUnicorn = new Color(0x00, 0xC8, 0xDC, 0xFF);

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
                _bci.OnDevicesAvailable.AddListener(UpdateAvailableDevices);
                _bci.OnDeviceStateChanged.AddListener(OnDeviceStateChanged);
            }

            try
            {
                _btnConnect = this.GetComponentInChildren<Button>();
                _btnText = _btnConnect.GetComponentInChildren<TextMeshProUGUI>();

                _ddDevices = this.GetComponentInChildren<TMP_Dropdown>();
                Image[] areasTmp = this.GetComponentsInChildren<Image>();
                foreach (Image area in areasTmp)
                {
                    if (area.gameObject.name.Equals("areaDevice"))
                        _areaDevice = area.gameObject;
                    else if (area.gameObject.name.Equals("areaSQ"))
                        _areaSQ = area.gameObject;
                    else if (area.gameObject.name.Equals("areaBat"))
                        _areaBat = area.gameObject;
                    else if (area.gameObject.name.Equals("areaDataLost"))
                        _areaDataLost = area.gameObject;
                    else if (area.gameObject.name.Equals("areaConnectionState"))
                        _areaConnectionState = area.gameObject;
                }

                if (_areaDevice == null ||
                    _areaSQ == null ||
                    _areaBat == null ||
                    _areaDataLost == null ||
                    _areaConnectionState == null)
                    throw new System.Exception("Could not get area gameobject.");
            }
            catch
            {
                _ddDevices = null;
                _btnText = null;
                _btnConnect = null;
                _areaDevice = null;
                _areaSQ = null;
                _areaBat = null;
                _areaDataLost = null;
                _areaConnectionState = null;
            }

            if (_ddDevices == null)
                throw new System.Exception("Could not get dropdown UI element");

            if (_btnConnect == null)
                throw new System.Exception("Could not get button UI element");

            if (_btnText == null)
                throw new System.Exception("Could not get button text UI element");

            _btnConnect.onClick.AddListener(btnConnect_OnClick);

            _goChannels = new List<GameObject>();
            _imgChannels = new List<Image>();

            _connected = false;
        }

        public void UpdateAvailableDevices(List<string> devices)
        {
            _ddDevices.ClearOptions();
            if (devices != null && devices.Count > 0)
                _ddDevices.AddOptions(devices);
        }

        private void btnConnect_OnClick()
        {
            if (!_connected)
            {
                string serial = _ddDevices.options[_ddDevices.value].text;

                if (_bci != null)
                    _bci.Connect(serial);

                OnConnect.Invoke(serial);
            }
            else
            {
                if (_bci != null)
                    _bci.Disconnect();

                OnDisconnect.Invoke();
            }
        }

        public void OnDeviceStateChanged(States state)
        {
            if (state == States.Connected)
            {
                _ddDevices.enabled = false;
                _btnText.text = "Disconnect";
                _connected = true;

                Texture2D tex = Resources.Load<Texture2D>("connected");
                Image img = _areaConnectionState.GetComponentInChildren<Image>();
                img.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                img.color = ColorUnicorn;
            }

            if (state == States.Disconnected)
            {
                _ddDevices.enabled = true;
                _btnText.text = "Connect";
                _connected = false;

                Texture2D tex = Resources.Load<Texture2D>("disconnected");
                Image img = _areaConnectionState.GetComponentInChildren<Image>();
                img.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                img.color = Color.white;

                foreach (Image image in _imgChannels)
                    Destroy(image.gameObject);
                _imgChannels.Clear();
                _goChannels.Clear();
            }
        }

        public void OnSignalQualityAvailable(List<ChannelStates> signalQuality)
        {
            if (_goChannels.Count != signalQuality.Count)
            {
                _goChannels.Clear();
                for (int i = 0; i < signalQuality.Count; i++)
                {
                    GameObject gameObject = new GameObject();
                    gameObject.name = string.Format("ch{0}", i + 1);
                    gameObject.transform.SetParent(_areaSQ.transform);

                    float margin = 0.2f / (float)signalQuality.Count;
                    float width = (1.0f - margin * signalQuality.Count) / (float)signalQuality.Count;
                    Image image = gameObject.AddComponent<Image>();
                    Texture2D tex = Resources.Load<Texture2D>("square");

                    image.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                    image.rectTransform.anchorMin = new Vector2(i * width + i * margin + margin / 2.0f, 0.1f);
                    image.rectTransform.anchorMax = new Vector2((i + 1) * width + i * margin + margin / 2.0f, 0.9f);
                    image.rectTransform.position = Vector2.zero;
                    image.rectTransform.anchoredPosition = Vector2.zero;
                    image.rectTransform.localPosition = Vector3.zero;
                    image.rectTransform.offsetMin = Vector2.zero;
                    image.rectTransform.offsetMax = Vector2.zero;
                    image.rectTransform.SetParent(_areaSQ.transform);

                    _goChannels.Add(gameObject);
                    _imgChannels.Add(image);
                }
            }

            for (int i = 0; i < signalQuality.Count; i++)
            {
                if (signalQuality[i].Equals(ChannelStates.Good))
                    _imgChannels[i].color = ChannelGood;
                else
                    _imgChannels[i].color = ChannelBad;
            }
        }

        public void UpdateBatteryLevel(float batteryLevel)
        {
            /*if (batteryLevel >= )
            {

            }
            else if (batteryLevel >=  && batteryLevel < )
            {

            }
            else
            { 

            }*/
        }
    }
}