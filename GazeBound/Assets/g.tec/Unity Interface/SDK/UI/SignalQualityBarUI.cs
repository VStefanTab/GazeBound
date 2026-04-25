using Gtec.Chain.Common.Templates.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Gtec.Chain.Common.Nodes.InputNodes.ChannelQuality;

namespace Gtec.UnityInterface
{
    public class SignalQualityUI : MonoBehaviour
    {
        public Color ChannelGood;
        public Color ChannelBad;

        private List<GameObject> _goChannels;
        private List<Image> _imgChannels;
        private GameObject _bgSQ;
        private SignalQualityPipeline _sqPipeline;
        public void Start()
        {
            try
            {
                _sqPipeline = GetComponentInParent<SignalQualityPipeline>();
            }
            catch
            {
                _sqPipeline = null;
            }
            if (_sqPipeline != null)
            {
                _sqPipeline.OnSignalQualityAvailable.AddListener(OnSignalQualityAvailable);
                _sqPipeline.OnPipelineStateChanged.AddListener(OnPipelineStateChanged);
            }

            try
            {
                Image[] areasTmp = this.GetComponentsInChildren<Image>();
                foreach (Image area in areasTmp)
                {
                    if (area.gameObject.name.Equals("bg"))
                        _bgSQ = area.gameObject;
                }
            }
            catch
            {
                _bgSQ = null;
            }

            if (_bgSQ == null)
                throw new Exception("Could not find SQ area");

            _goChannels = new List<GameObject>();
            _imgChannels = new List<Image>();
        }

        private void OnPipelineStateChanged(PipelineState state)
        {
            if (state == PipelineState.NotReady)
            {
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
                    gameObject.transform.SetParent(_bgSQ.transform);

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
                    image.rectTransform.SetParent(_bgSQ.transform);

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
    }
}