using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static Gtec.Chain.Common.Templates.DataAcquisitionUnit.DataAcquisitionUnit;

namespace Gtec.UnityInterface
{
    public class DeviceDialogUI : MonoBehaviour
    {
        public UnityEvent<string> OnConnect;
        public UnityEvent OnDisconnect;

        private TMP_Dropdown _ddDevices;
        private Button _btnConnect;
        private TextMeshProUGUI _btnText;
        private bool _connected;
        private Device _bci;
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

            _ddDevices = this.GetComponentInChildren<TMP_Dropdown>();
            _btnConnect = this.GetComponentInChildren<Button>();
            _btnText = _btnConnect.GetComponentInChildren<TextMeshProUGUI>();

            if (_ddDevices == null)
                throw new System.Exception("Could not get dropdown UI element");

            if (_btnConnect == null)
                throw new System.Exception("Could not get button UI element");

            if (_btnText == null)
                throw new System.Exception("Could not get button text UI element");

            _connected = false;

            _btnConnect.onClick.AddListener(btnConnect_OnClick);
        }

        public void UpdateAvailableDevices(List<string> devices)
        {
            _ddDevices.ClearOptions();
            if (devices != null && devices.Count > 0)
                _ddDevices.AddOptions(devices);
        }

        public void OnDeviceStateChanged(States state)
        {
            if (state == States.Connected)
            {
                _ddDevices.enabled = false;
                _btnText.text = "Disconnect";
                _connected = true;
            }

            if (state == States.Disconnected)
            {
                _ddDevices.enabled = true;
                _btnText.text = "Connect";
                _connected = false;
            }
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
    }
}