using Gtec.Chain.Common.Templates.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Gtec.UnityInterface
{
    public class ERPParadigmUI : MonoBehaviour
    {
        public UnityEvent<ParadigmMode> OnStartParadigm;
        public UnityEvent OnStopParadigm;

        private Button _btnStartStopTraining;
        private TextMeshProUGUI _btnStartStopTrainingText;
        private Button _btnStartApplication;
        private TextMeshProUGUI _btnStartApplicationText;
        private ERPParadigm _erpParadigm;
        private ERPPipeline _erpPipeline;

        private ParadigmMode _paradigmMode;
        private bool _paradigmRunnning;
        private PipelineState _pipelineState;

        void Start()
        {
            try
            {
                _erpParadigm = GetComponentInParent<ERPParadigm>();
                _erpParadigm.OnParadigmStarted.AddListener(ParadigmStarted);
                _erpParadigm.OnParadigmStopped.AddListener(ParadigmStopped);
            }
            catch
            {
                _erpParadigm = null;
            }

            try
            {
                _erpPipeline = GetComponentInParent<ERPPipeline>();
                _erpPipeline.OnPipelineStateChanged.AddListener(OnPipelineStateChanged);
                _erpPipeline.OnCalibrationResult.AddListener(OnClassifierAvailable);
            }
            catch
            {
                _erpPipeline = null;
            }

            Button[] buttons = this.GetComponentsInChildren<Button>();
            foreach (Button btn in buttons)
            {
                if (btn.name.Equals("btnStartStopTraining"))
                {
                    _btnStartStopTraining = btn;
                    _btnStartStopTrainingText = btn.GetComponentInChildren<TextMeshProUGUI>();
                }
                if (btn.name.Equals("btnStartStopApplication"))
                {
                    _btnStartApplication = btn;
                    _btnStartApplicationText = btn.GetComponentInChildren<TextMeshProUGUI>();
                }
            }

            if (_btnStartStopTraining == null)
                throw new System.Exception("Could not get button UI element");

            if (_btnStartStopTrainingText == null)
                throw new System.Exception("Could not get button text UI element");

            if (_btnStartApplication == null)
                throw new System.Exception("Could not get button UI element");

            if (_btnStartApplicationText == null)
                throw new System.Exception("Could not get button text UI element");

            _btnStartStopTraining.onClick.AddListener(btnStartStopTraining_OnClick);
            _btnStartApplication.onClick.AddListener(btnStartApplication_OnClick);

            OnClassifierAvailable(null, null);
            OnPipelineStateChanged(PipelineState.NotReady);
            _btnStartStopTrainingText.text = "Start Training";
            _paradigmMode = ParadigmMode.Training;
            _paradigmRunnning = false;
        }

        private void ParadigmStopped()
        {
            EventHandler.Instance.Enqueue(() =>
            {
                _paradigmRunnning = false;
                if (_paradigmMode == ParadigmMode.Training)
                {
                    _btnStartStopTrainingText.text = "Start Training";
                    _btnStartStopTraining.gameObject.SetActive(false);
                    _btnStartApplication.gameObject.SetActive(false);
                }
                else
                {
                    _btnStartStopTrainingText.text = "Start Training";
                    _btnStartStopTraining.gameObject.SetActive(true);
                    _btnStartApplication.gameObject.SetActive(false);

                }

                if (_pipelineState == PipelineState.NotReady)
                {
                    _btnStartStopTraining.gameObject.SetActive(false);
                    _btnStartApplication.gameObject.SetActive(false);
                }
            });
        }

        private void ParadigmStarted()
        {
            EventHandler.Instance.Enqueue(() =>
            {
                _paradigmRunnning = true;
                if (_paradigmMode == ParadigmMode.Training)
                {
                    _btnStartStopTrainingText.text = "Stop Training";
                }
                else
                {
                    _btnStartStopTrainingText.text = "Retrain";
                }
                _btnStartApplication.gameObject.SetActive(false);
            });
        }

        public void OnPipelineStateChanged(PipelineState state)
        {
            _pipelineState = state;
            EventHandler.Instance.Enqueue(() =>
            {
                if (state == PipelineState.NotReady)
                {
                    _paradigmMode = ParadigmMode.Training;

                    if (_paradigmRunnning)
                    {
                        if (_erpParadigm != null)
                            _erpParadigm.StopParadigm();

                        OnStopParadigm.Invoke();
                    }

                    _btnStartStopTraining.gameObject.SetActive(false);
                    _btnStartApplication.gameObject.SetActive(false);
                }

                if (state == PipelineState.Ready)
                {
                    _btnStartStopTraining.gameObject.SetActive(true);
                    if (_paradigmMode == ParadigmMode.Training)
                        _btnStartApplication.gameObject.SetActive(false);
                    else
                        _btnStartApplication.gameObject.SetActive(true);
                }
            });
        }

        private void OnClassifierAvailable(ERPParadigm paradign, CalibrationResult result)
        {
            EventHandler.Instance.Enqueue(() =>
            {
                if (result != null)
                {
                    _btnStartStopTraining.gameObject.SetActive(true);
                    _btnStartStopTrainingText.text = "Retrain";

                    _btnStartApplication.gameObject.SetActive(true);
                    _btnStartApplicationText.text = "Continue";
                }
                else
                {
                    _btnStartStopTraining.gameObject.SetActive(true);
                    _btnStartStopTrainingText.text = "Start Training";

                    _btnStartApplication.gameObject.SetActive(false);
                    _btnStartApplicationText.text = "Continue";
                }
            });
        }

        private void btnStartStopTraining_OnClick()
        {
            if (_paradigmMode == ParadigmMode.Training && !_paradigmRunnning)
            {
                _paradigmMode = ParadigmMode.Training;
                if (_erpParadigm != null)
                    _erpParadigm.StartParadigm(_paradigmMode);

                OnStartParadigm.Invoke(_paradigmMode);
            }
            else if (_paradigmMode == ParadigmMode.Training && _paradigmRunnning)
            {
                if (_erpParadigm != null)
                    _erpParadigm.StopParadigm();

                OnStopParadigm.Invoke();
            }
            else if (_paradigmMode == ParadigmMode.Application && _paradigmRunnning)
            {
                _paradigmMode = ParadigmMode.Training;

                if (_erpParadigm != null)
                    _erpParadigm.StopParadigm();

                OnStopParadigm.Invoke();
            }
        }

        private void btnStartApplication_OnClick()
        {
            _paradigmMode = ParadigmMode.Application;
            if (_erpParadigm != null)
                _erpParadigm.StartParadigm(_paradigmMode);

            OnStartParadigm.Invoke(_paradigmMode);
        }
    }
}