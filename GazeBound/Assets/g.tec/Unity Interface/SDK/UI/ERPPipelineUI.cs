using Gtec.Chain.Common.Templates.Utilities;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Gtec.UnityInterface
{
    public class ERPPipelineUI : MonoBehaviour
    {
        public Color ColorGood;
        public Color ColorOk;
        public Color ColorBad;

        private ERPPipeline _erpPipeline;
        private Image _imgCalibration;

        void Start()
        {
            try
            {
                _erpPipeline = GetComponentInParent<ERPPipeline>();
                _erpPipeline.OnCalibrationResult.AddListener(OnClassifierAvailable);
            }
            catch
            {
                _erpPipeline = null;
            }

            try
            {
                Image[] images = this.GetComponentsInChildren<Image>();
                foreach (Image img in images)
                {
                    if (img.gameObject.name.Contains("img"))
                    {
                        _imgCalibration = img;
                        break;
                    }
                }
            }
            catch
            {
                _imgCalibration = null;
            }

            OnClassifierAvailable(null, null);
        }

        private void OnClassifierAvailable(ERPParadigm paradigm, CalibrationResult result)
        {
            EventHandler.Instance.Enqueue(() =>
            {
                if (result != null && paradigm != null)
                {
                    string res = string.Empty;
                    res += "Calibration Result: " + result.CalibrationQuality.ToString() + "\n";
                    res += "Cross-validation:\n";
                    foreach (KeyValuePair<uint, double> kvp in result.Crossvalidation)
                        res += string.Format("{0}\t|", kvp.Key);
                    res += "\n";
                    foreach (KeyValuePair<uint, double> kvp in result.Crossvalidation)
                        res += string.Format("{0}\t|", kvp.Value);
                    res += "\n";
                    res += "Number of trials: " + result.TrialsSelected + "\n";
                    res += "Estimated selection time: " + (paradigm.OnTimeMs + paradigm.OffTimeMs) * result.TrialsSelected + "\n";
                    Debug.Log(res);

                    _imgCalibration.gameObject.SetActive(true);
                    Texture2D tex = Resources.Load<Texture2D>("circle");
                    _imgCalibration.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                    _imgCalibration.preserveAspect = true;
                    if (result.CalibrationQuality == CalibrationQuality.Good)
                    {
                        _imgCalibration.color = ColorGood;
                    }
                    else if (result.CalibrationQuality == CalibrationQuality.Ok)
                    {
                        _imgCalibration.color = ColorOk;
                    }
                    else if (result.CalibrationQuality == CalibrationQuality.Bad)
                    {
                        _imgCalibration.color = ColorBad;
                    }
                }
                else
                {
                    _imgCalibration.gameObject.SetActive(false);
                }
            });
        }
    }
}