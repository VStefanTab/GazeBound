using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Gtec.UnityInterface
{
    public class BCIUI : MonoBehaviour
    {
        public DeviceDialogUI DeviceDialogUI;
        public SignalQualityUI SignalQualityUI;
        public ERPPipelineUI ERPPipelineUI;
        public ERPParadigmUI ERPParadigmUI;
        public DeviceConnectionStateUI DeviceConnectionStateUI;
        public BatteryLevelPipelineUI BatteryLevelPipelineUI;
        public DataLostPipelineUI DataLostPipelineUI;

        private class UIElement
        {
            public MonoBehaviour UI;
            public RectTransform Rect;
            public GameObject Area;
            public bool Shifted;
            public UIElement(MonoBehaviour ui, RectTransform rect, GameObject area, bool shifted)
            {
                UI = ui;
                Area = area;
                Shifted = shifted;
                Rect = rect;
            }
        }

        private List<UIElement> _uiElements;

        void Start()
        {
            _uiElements = new List<UIElement>();
            Image[] areasTmp = this.GetComponentsInChildren<Image>();

            GameObject areaDevice = GetArea(areasTmp, "areaDevice");
            AddUIElement(areaDevice, DeviceDialogUI, "bg");

            GameObject areaSQ = GetArea(areasTmp, "areaSQ");
            AddUIElement(areaSQ, SignalQualityUI, "bg");

            GameObject areaPdgm = GetArea(areasTmp, "areaPdgm");
            AddUIElement(areaPdgm, ERPParadigmUI, "bg");

            GameObject areaConnectionState = GetArea(areasTmp, "areaConnectionState");
            AddUIElement(areaConnectionState, DeviceConnectionStateUI, "bg");

            GameObject areaClassifier = GetArea(areasTmp, "areaClassifier");
            AddUIElement(areaClassifier, ERPPipelineUI, "bg");

            GameObject areaBattery = GetArea(areasTmp, "areaBattery");
            AddUIElement(areaBattery, BatteryLevelPipelineUI, "bg");

            GameObject areaDataLost = GetArea(areasTmp, "areaDataLost");
            AddUIElement(areaDataLost, DataLostPipelineUI, "bg");
        }

        private void AddUIElement(GameObject area, MonoBehaviour ui, string uiName)
        {
            RectTransform rect = null;
            if (ui != null)
                rect = GetRectTransform(ui.GetComponentsInChildren<RectTransform>(), uiName);
            if (area != null && ui != null && rect != null)
                _uiElements.Add(new UIElement(ui, rect, area, false));
        }

        private GameObject GetArea(Image[] areas, string name)
        {
            try
            {
                foreach (Image area in areas)
                    if (area.gameObject.name.Equals(name))
                        return area.gameObject;
                return null;
            }
            catch
            {
                return null;
            }
        }

        private RectTransform GetRectTransform(RectTransform[] rects, string name)
        {
            try
            {
                foreach (RectTransform rect in rects)
                    if (rect.gameObject.name.Equals(name))
                        return rect;
                return null;
            }
            catch
            {
                return null;
            }
        }

        void Update()
        {
            foreach (UIElement uie in _uiElements)
            {
                if (!uie.Shifted)
                {
                    uie.UI.transform.SetParent(uie.Area.transform);
                    uie.UI.transform.localPosition = Vector3.zero;
                    uie.Rect.SetParent(uie.Area.transform);
                    uie.Rect.anchorMin = new Vector2(0, 0);
                    uie.Rect.anchorMax = new Vector2(1, 1);
                    uie.Rect.position = Vector2.zero;
                    uie.Rect.anchoredPosition = Vector2.zero;
                    uie.Rect.localPosition = Vector3.zero;
                    uie.Rect.offsetMin = Vector2.zero;
                    uie.Rect.offsetMax = Vector2.zero;
                }
                uie.Shifted = true;
            }
        }
    }
}