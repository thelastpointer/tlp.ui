using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace TLP.UI
{
    public class MessageBoxes : MonoBehaviour
    {
        #region Editor-assigned fields

        // Disable "never assigned to, and will always have its default value null" (these are always assigned in the editor)
        #pragma warning disable 0649

        [Header("Default Texts")]
        [SerializeField] private string DefaultButtonText = "OK";

        [Header("Transition")]
        [SerializeField] private bool useDefaultTransition = true;
        [SerializeField] private WindowTransition customTransition;

        [Header("Background Denier")]
        [SerializeField] private GameObject background;

        [Header("Simple message box")]
        [SerializeField] private GameObject messageBoxContainer;
        [SerializeField] private Text messageBoxTitle;
        [SerializeField] private Text messageBoxText;
        [SerializeField] private Button messageBoxButton;

        [Header("Error dialog box")]
        [SerializeField] private GameObject errorBoxContainer;
        [SerializeField] private Text errorBoxTitle;
        [SerializeField] private Text errorBoxText;
        [SerializeField] private Button errorBoxButton;

        [Header("Yes/no prompt")]
        [SerializeField] private GameObject promptContainer;
        [SerializeField] private Text promptTitle;
        [SerializeField] private Text promptText;
        [SerializeField] private Button promptButtonYes;
        [SerializeField] private Button promptButtonNo;

        [Header("Text prompt")]
        [SerializeField] private GameObject textPromptContainer;
        [SerializeField] private Text textPromptTitle;
        [SerializeField] private Text textPromptText;
        [SerializeField] private InputField textPromptInput;
        [SerializeField] private Button textPromptButtonAccept;
        [SerializeField] private Button textPromptButtonCancel;

        [Header("Progress dialog")]
        [SerializeField] private GameObject progressContainer;
        [SerializeField] private Text progressTitle;
        [SerializeField] private Text progressText;
        [SerializeField] private Image progressFillImage;
        [SerializeField] private Slider progressFillSlider;

        [Header("Progress overlay")]
        [SerializeField] private GameObject progressOverlayContainer;
        [SerializeField] private Text progressOverlayText;
        [SerializeField] private Image progressOverlayFillImage;
        [SerializeField] private Slider progressOverlayFillSlider;

#pragma warning restore 0649
        #endregion

        // Singleton
        public static MessageBoxes Instance { get; private set; }

        public WindowTransition Transition
        {
            get
            {
                if (useDefaultTransition && (WindowManager.Instance != null))
                    return WindowManager.Instance.DefaultTransition;
                return customTransition;
            }
        }

        private object modalLock = new object();
        private List<Action> modalCloseCallbacks = new List<Action>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);

                ClearAll();
                HideProgressOverlay();
            }
            else
                Destroy(gameObject);
        }

        // Generic message
        public void ShowMessage(string title, string message, string buttonText, Action onDismissed)
        {
            ClearAll();

            if (messageBoxTitle != null)
                messageBoxTitle.text = title;

            messageBoxText.text = message;

            var btnText = messageBoxButton.GetComponentInChildren<Text>();
            if (btnText != null)
                btnText.text = buttonText;

            messageBoxButton.onClick.RemoveAllListeners();
            messageBoxButton.onClick.AddListener(() =>
            {
                StartCoroutine(WindowAnimator.Animate(messageBoxContainer, Transition, true, () =>
                {
                    background.SetActive(false);
                    messageBoxContainer.SetActive(false);
                    ModalClosed();
                    if (onDismissed != null)
                        onDismissed();
                }));
            });

            background.SetActive(true);
            messageBoxContainer.SetActive(true);
            StartCoroutine(WindowAnimator.Animate(messageBoxContainer, Transition, false));
        }
        public void ShowMessage(string message)
        {
            ShowMessage("", message, DefaultButtonText, null);
        }
        public void ShowMessage(string message, Action onDismissed)
        {
            ShowMessage("", message, DefaultButtonText, onDismissed);
        }
        public void ShowMessage(string title, string message, Action onDismissed)
        {
            ShowMessage(title, message, DefaultButtonText, onDismissed);
        }

        // Generic error
        public void ShowError(string title, string message, string buttonText, Action onDismissed)
        {
            ClearAll();

            if (errorBoxTitle != null)
                errorBoxTitle.text = title;

            errorBoxText.text = message;

            var btnText = errorBoxButton.GetComponentInChildren<Text>();
            if (btnText != null)
                btnText.text = buttonText;

            errorBoxButton.onClick.RemoveAllListeners();
            errorBoxButton.onClick.AddListener(() =>
            {
                StartCoroutine(WindowAnimator.Animate(errorBoxContainer, Transition, true, () =>
                {
                    background.SetActive(false);
                    errorBoxContainer.SetActive(false);
                    ModalClosed();
                    if (onDismissed != null)
                        onDismissed();
                }));
            });

            background.SetActive(true);
            errorBoxContainer.SetActive(true);
            StartCoroutine(WindowAnimator.Animate(errorBoxContainer, Transition, false));
        }
        public void ShowError(string title, string message, Action onDismissed)
        {
            ShowError(title, message, DefaultButtonText, onDismissed);
        }
        public void ShowError(string message, Action onDismissed)
        {
            ShowError("", message, DefaultButtonText, onDismissed);
        }

        // Yes/no promt
        public void PromptYesNo(string title, string message, string yesButtonText, string noButtonText, Action onYes, Action onNo)
        {
            ClearAll();

            if (promptTitle != null)
                promptTitle.text = title;

            promptText.text = message;

            var btnText = promptButtonYes.GetComponentInChildren<Text>();
            if (btnText != null)
                btnText.text = yesButtonText;
            btnText = promptButtonNo.GetComponentInChildren<Text>();
            if (btnText != null)
                btnText.text = noButtonText;

            promptButtonYes.onClick.RemoveAllListeners();
            promptButtonYes.onClick.AddListener(() =>
            {
                StartCoroutine(WindowAnimator.Animate(promptContainer, Transition, true, () =>
                {
                    background.SetActive(false);
                    promptContainer.SetActive(false);
                    ModalClosed();
                    if (onYes != null)
                        onYes();
                }));
            });

            promptButtonNo.onClick.RemoveAllListeners();
            promptButtonNo.onClick.AddListener(() =>
            {
                StartCoroutine(WindowAnimator.Animate(promptContainer, Transition, true, () =>
                {
                    background.SetActive(false);
                    promptContainer.SetActive(false);
                    ModalClosed();
                    if (onNo != null)
                        onNo();
                })); 
            });

            background.SetActive(true);
            promptContainer.SetActive(true);
            StartCoroutine(WindowAnimator.Animate(promptContainer, Transition, false));
        }
        public void PromptYesNo(string message, string yesButtonText, string noButtonText, Action onYes, Action onNo)
        {
            PromptYesNo("", message, yesButtonText, noButtonText, onYes, onNo);
        }
        public void PromptYesNo(string message, Action onYes, Action onNo)
        {
            PromptYesNo("", message, "Yes", "No", onYes, onNo);
        }

        // Text prompt
        public void PromptText(string title, string message, string buttonText, Action<string> onEntered)
        {
            ClearAll();

            if (textPromptTitle != null)
                textPromptTitle.text = title;

            textPromptText.text = message;
            textPromptInput.text = "";

            textPromptButtonAccept.gameObject.SetActive(true);
            textPromptButtonCancel.gameObject.SetActive(false);

            var btnText = textPromptButtonAccept.GetComponentInChildren<Text>();
            if (btnText != null)
                btnText.text = buttonText;

            textPromptButtonAccept.onClick.RemoveAllListeners();
            textPromptButtonAccept.onClick.AddListener(() =>
            {
                StartCoroutine(WindowAnimator.Animate(textPromptContainer, Transition, true, () =>
                {
                    background.SetActive(false);
                    textPromptContainer.SetActive(false);
                    ModalClosed();
                    if (onEntered != null)
                        onEntered(textPromptInput.text);
                }));
            });

            background.SetActive(true);
            textPromptContainer.SetActive(true);
            StartCoroutine(WindowAnimator.Animate(textPromptContainer, Transition, false));
        }
        public void PromptText(string title, string message, Action<string> onEntered)
        {
            PromptText(title, message, DefaultButtonText, onEntered);
        }
        public void PromptText(string message, Action<string> onEntered)
        {
            PromptText("", message, DefaultButtonText, onEntered);
        }
        public void PromptText(string title, string message, string acceptButtonText, string cancelButtonText, Action<string> onEntered, Action onCancelled)
        {
            ClearAll();

            if (textPromptTitle != null)
                textPromptTitle.text = title;

            textPromptText.text = message;
            textPromptInput.text = "";

            textPromptButtonAccept.gameObject.SetActive(true);
            textPromptButtonCancel.gameObject.SetActive(true);

            var btnText = textPromptButtonAccept.GetComponentInChildren<Text>();
            if (btnText != null)
                btnText.text = acceptButtonText;
            btnText = textPromptButtonCancel.GetComponentInChildren<Text>();
            if (btnText != null)
                btnText.text = cancelButtonText;

            textPromptButtonAccept.onClick.RemoveAllListeners();
            textPromptButtonAccept.onClick.AddListener(() =>
            {
                StartCoroutine(WindowAnimator.Animate(textPromptContainer, Transition, true, () =>
                {
                    background.SetActive(false);
                    textPromptContainer.SetActive(false);
                    ModalClosed();
                    if (onEntered != null)
                        onEntered(textPromptInput.text);
                }));
            });
            textPromptButtonCancel.onClick.RemoveAllListeners();
            textPromptButtonCancel.onClick.AddListener(() =>
            {
                StartCoroutine(WindowAnimator.Animate(textPromptContainer, Transition, true, () =>
                {
                    background.SetActive(false);
                    textPromptContainer.SetActive(false);
                    ModalClosed();
                    if (onCancelled != null)
                        onCancelled();
                }));
            });

            background.SetActive(true);
            textPromptContainer.SetActive(true);
            StartCoroutine(WindowAnimator.Animate(textPromptContainer, Transition, false));
        }
        public void PromptText(string message, string acceptButtonText, string cancelButtonText, Action<string> onEntered, Action onCancelled)
        {
            PromptText("", message, acceptButtonText, cancelButtonText, onEntered, onCancelled);
        }
        public void PromptText(string message, Action<string> onEntered, Action onCancelled)
        {
            PromptText("", message, "OK", "Cancel", onEntered, onCancelled);
        }

        // Progress overlay
        public void ShowProgressOverlay(float progress, string text)
        {
            if (progressOverlayText != null)
                progressOverlayText.text = text;
            if (progressOverlayFillImage != null)
                progressOverlayFillImage.fillAmount = progress;
            if (progressOverlayFillSlider != null)
                progressOverlayFillSlider.normalizedValue = progress;

            progressOverlayContainer.SetActive(true);
        }
        public void HideProgressOverlay()
        {
            progressOverlayContainer.SetActive(false);
        }

        // Modal progress
        public void ProgressModal(float progress, string title, string text)
        {
            if (progressTitle != null)
                progressTitle.text = title;

            if (progressText != null)
                progressText.text = text;

            if (progressFillImage != null)
                progressFillImage.fillAmount = progress;

            if (progressFillSlider != null)
                progressFillSlider.normalizedValue = progress;

            if (!progressContainer.activeSelf)
            {
                ClearAll();

                background.SetActive(true);
                progressContainer.SetActive(true);
                StartCoroutine(WindowAnimator.Animate(progressContainer, Transition, false));
            }
        }
        public void HideProgressModal()
        {
            background.SetActive(false);
            StartCoroutine(WindowAnimator.Animate(progressContainer, Transition, true, () =>
            {
                progressContainer.SetActive(false);
                ModalClosed();
            }));
        }

        public void ClearAll()
        {
            lock (modalLock)
            {
                modalCloseCallbacks.Clear();
            }

            messageBoxContainer.SetActive(false);
            errorBoxContainer.SetActive(false);
            promptContainer.SetActive(false);
            textPromptContainer.SetActive(false);
            progressContainer.SetActive(false);
            //progressOverlayContainer.SetActive(false);
        }

        public void WaitForModals(Action onCleared)
        {
            lock (modalLock)
            {
                modalCloseCallbacks.Add(onCleared);
            }
        }

        private void ModalClosed()
        {
            lock (modalLock)
            {
                foreach (var callback in modalCloseCallbacks)
                {
                    if (callback != null)
                        callback();
                }

                modalCloseCallbacks.Clear();
            }
        }
    }
}