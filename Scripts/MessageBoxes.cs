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
        [SerializeField] private UIAnimation customTransition;

        [Header("Background Denier")]
        [SerializeField] private GameObject background;

        [Header("Simple message box")]
        [SerializeField] private AnimatedPanel messageBoxContainer;
        [SerializeField] private Text messageBoxTitle;
        [SerializeField] private Text messageBoxText;
        [SerializeField] private Button messageBoxButton;

        [Header("Error dialog box")]
        [SerializeField] private AnimatedPanel errorBoxContainer;
        [SerializeField] private Text errorBoxTitle;
        [SerializeField] private Text errorBoxText;
        [SerializeField] private Button errorBoxButton;

        [Header("Yes/no prompt")]
        [SerializeField] private AnimatedPanel promptContainer;
        [SerializeField] private Text promptTitle;
        [SerializeField] private Text promptText;
        [SerializeField] private Button promptButtonYes;
        [SerializeField] private Button promptButtonNo;

        [Header("Text prompt")]
        [SerializeField] private AnimatedPanel textPromptContainer;
        [SerializeField] private Text textPromptTitle;
        [SerializeField] private Text textPromptText;
        [SerializeField] private InputField textPromptInput;
        [SerializeField] private Button textPromptButtonAccept;
        [SerializeField] private Button textPromptButtonCancel;

        [Header("Progress dialog")]
        [SerializeField] private AnimatedPanel progressContainer;
        [SerializeField] private Text progressTitle;
        [SerializeField] private Text progressText;
        [SerializeField] private Image progressFillImage;
        [SerializeField] private Slider progressFillSlider;

        [Header("Progress overlay")]
        [SerializeField] private GameObject progressOverlayContainer;
        [SerializeField] private Text progressOverlayText;
        [SerializeField] private Image progressOverlayFillImage;
        [SerializeField] private Slider progressOverlayFillSlider;

        [Header("Progress Infinite")]
        [SerializeField] private AnimatedPanel progressInfiniteContainer;
        [SerializeField] private Text progressInfiniteText;

#pragma warning restore 0649
        #endregion

        private bool progressModalVisible = false;
        private bool progressInfiniteVisible = false;

        // Singleton
        public static MessageBoxes Instance { get; private set; }

        public UIAnimation Transition
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
                background.SetActive(false);
                StartCoroutine(UIAnimator.AnimationRoutine(messageBoxContainer, -1, () =>
                {
                    messageBoxContainer.gameObject.SetActive(false);
                    ModalClosed();
                    if (onDismissed != null)
                        onDismissed();
                }));
            });

            background.SetActive(true);
            messageBoxContainer.gameObject.SetActive(true);
            StartCoroutine(UIAnimator.AnimationRoutine(messageBoxContainer, 1));
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
                background.SetActive(false);
                StartCoroutine(UIAnimator.AnimationRoutine(errorBoxContainer, -1, () =>
                {
                    errorBoxContainer.gameObject.SetActive(false);
                    ModalClosed();
                    if (onDismissed != null)
                        onDismissed();
                }));
            });

            background.SetActive(true);
            errorBoxContainer.gameObject.SetActive(true);
            StartCoroutine(UIAnimator.AnimationRoutine(errorBoxContainer, 1));
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
                background.SetActive(false);
                StartCoroutine(UIAnimator.AnimationRoutine(promptContainer, -1, () =>
                {
                    promptContainer.gameObject.SetActive(false);
                    ModalClosed();
                    if (onYes != null)
                        onYes();
                }));
            });

            promptButtonNo.onClick.RemoveAllListeners();
            promptButtonNo.onClick.AddListener(() =>
            {
                background.SetActive(false);
                StartCoroutine(UIAnimator.AnimationRoutine(promptContainer, -1, () =>
                {
                    promptContainer.gameObject.SetActive(false);
                    ModalClosed();
                    if (onNo != null)
                        onNo();
                })); 
            });

            background.SetActive(true);
            promptContainer.gameObject.SetActive(true);
            StartCoroutine(UIAnimator.AnimationRoutine(promptContainer, 1));
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
                background.SetActive(false);
                StartCoroutine(UIAnimator.AnimationRoutine(textPromptContainer, -1, () =>
                {
                    textPromptContainer.gameObject.SetActive(false);
                    ModalClosed();
                    if (onEntered != null)
                        onEntered(textPromptInput.text);
                }));
            });

            background.SetActive(true);
            textPromptContainer.gameObject.SetActive(true);
            StartCoroutine(UIAnimator.AnimationRoutine(textPromptContainer, 1));
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
                background.SetActive(false);
                StartCoroutine(UIAnimator.AnimationRoutine(textPromptContainer, -1, () =>
                {
                    textPromptContainer.gameObject.SetActive(false);
                    ModalClosed();
                    if (onEntered != null)
                        onEntered(textPromptInput.text);
                }));
            });
            textPromptButtonCancel.onClick.RemoveAllListeners();
            textPromptButtonCancel.onClick.AddListener(() =>
            {
                background.SetActive(false);
                StartCoroutine(UIAnimator.AnimationRoutine(textPromptContainer, -1, () =>
                {
                    textPromptContainer.gameObject.SetActive(false);
                    ModalClosed();
                    if (onCancelled != null)
                        onCancelled();
                }));
            });

            background.SetActive(true);
            textPromptContainer.gameObject.SetActive(true);
            StartCoroutine(UIAnimator.AnimationRoutine(textPromptContainer, 1));
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
            if (progressOverlayContainer != null)
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

            if (!progressContainer.gameObject.activeSelf)
            {
                ClearAll();

                background.SetActive(true);
                progressContainer.gameObject.SetActive(true);
                progressModalVisible = true;
            }
        }
        public void HideProgressModal()
        {
            background.SetActive(false);
            progressModalVisible = false;
        }

        // Infinite progress
        public void ShowProgressInfinite(string message)
        {
            if (progressInfiniteText != null)
                progressInfiniteText.text = message;

            if (!progressInfiniteContainer.gameObject.activeSelf)
            {
                ClearAll();
                background.SetActive(true);
                progressInfiniteContainer.gameObject.SetActive(true);

                progressInfiniteVisible = true;
            }
        }
        public void HideProgressInfinite()
        {
            // Note: take extra care with this; infinite progress is often interrupted
            // (for example, an error happens), so make sure we don't hide the background
            // if something else uses it.
            bool canClose =
                ((messageBoxContainer == null) || !messageBoxContainer.gameObject.activeSelf) &&
                ((errorBoxContainer == null) || !errorBoxContainer.gameObject.activeSelf) &&
                ((promptContainer == null) || !promptContainer.gameObject.activeSelf) &&
                ((textPromptContainer == null) || !textPromptContainer.gameObject.activeSelf) &&
                ((progressContainer == null) || !progressContainer.gameObject.activeSelf);
            if (canClose)
                background.SetActive(false);

            progressInfiniteVisible = false;
        }

        public void ClearAll()
        {
            lock (modalLock)
            {
                modalCloseCallbacks.Clear();
            }

            if (messageBoxContainer != null)
                messageBoxContainer.gameObject.SetActive(false);
            if (errorBoxContainer != null)
                errorBoxContainer.gameObject.SetActive(false);
            if (promptContainer != null)
                promptContainer.gameObject.SetActive(false);
            if (textPromptContainer != null)
                textPromptContainer.gameObject.SetActive(false);
            if (progressContainer != null)
                progressContainer.gameObject.SetActive(false);
            //progressOverlayContainer.SetActive(false);
            if (progressInfiniteContainer != null)
                progressInfiniteContainer.gameObject.SetActive(false);
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

        private void Update()
        {
            if (progressContainer != null)
            {
                // Show/hide progress modal
                float prev = progressContainer.AnimationProgress;
                UIAnimator.Animate(progressContainer, Time.unscaledDeltaTime * (progressModalVisible ? 1 : -1));

                // If just closed, call ModalClosed
                if ((prev > 0) && (progressContainer.AnimationProgress <= 0))
                {
                    progressContainer.gameObject.SetActive(false);
                    ModalClosed();
                }
            }

            if (progressInfiniteContainer != null)
            {
                // Show/hide infinite progress
                float prev = progressInfiniteContainer.AnimationProgress;
                UIAnimator.Animate(progressInfiniteContainer, Time.unscaledDeltaTime * (progressInfiniteVisible ? 1 : -1));

                // If just closed, call ModalClosed
                if ((prev > 0) && (progressInfiniteContainer.AnimationProgress <= 0))
                {
                    progressInfiniteContainer.gameObject.SetActive(false);
                    ModalClosed();
                }
            }
        }
    }
}