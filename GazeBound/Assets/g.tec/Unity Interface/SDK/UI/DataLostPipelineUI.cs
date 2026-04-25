using UnityEngine;
using UnityEngine.UI;

namespace Gtec.UnityInterface
{
    public class DataLostPipelineUI : MonoBehaviour
    {
        private DataLostPipeline _bci;
        private GameObject _go;
        private int _cnt;

        void Start()
        {
            try
            {
                _bci = GetComponentInParent<DataLostPipeline>();
            }
            catch
            {
                _bci = null;
            }
            if (_bci != null)
            {
                _bci.OnDataLost.AddListener(OnDataLost);
            }

            try
            {
                Image[] images = this.GetComponentsInChildren<Image>();
                foreach (Image img in images)
                {
                    if (img.gameObject.name.Contains("img"))
                    {
                        _go = img.gameObject;
                        break;
                    }
                }
            }
            catch
            {
                _go = null;
            }

            if (_go == null)
                throw new System.Exception("Could not find data lost image");

            _go.SetActive(false);
            _cnt = -1;
        }

        public void Update()
        {
            if (_cnt >= 0)
            {
                _cnt++;
                if (_cnt >= (int)(Screen.currentResolution.refreshRateRatio.value / 2))
                {
                    _go.SetActive(false);
                    _cnt = -1;
                }
            }
        }

        public void OnDataLost()
        {
            _go.SetActive(true);
            _cnt = 0;
        }
    }
}