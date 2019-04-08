using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace TLP.UI
{
    public class MessageBoxes : MonoBehaviour
    {
        private static MessageBoxes instance;
        public static MessageBoxes Instance { get { return instance; } }

        [SerializeField]
        private string DefaultButtonText = "OK";

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
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
            messageBoxButton.onClick.AddListener(() => {
                background.SetActive(false);
                messageBoxContainer.SetActive(false);
                ModalClosed();
                if (onDismissed != null)
                    onDismissed();
            });

            background.SetActive(true);
            messageBoxContainer.SetActive(true);
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
            errorBoxButton.onClick.AddListener(() => {
                background.SetActive(false);
                errorBoxContainer.SetActive(false);
                ModalClosed();
                if (onDismissed != null)
                    onDismissed();
            });

            background.SetActive(true);
            errorBoxContainer.SetActive(true);
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
            promptButtonYes.onClick.AddListener(() => {
                background.SetActive(false);
                promptContainer.SetActive(false);
                ModalClosed();
                if (onYes != null)
                    onYes();
            });

            promptButtonNo.onClick.RemoveAllListeners();
            promptButtonNo.onClick.AddListener(() => {
                background.SetActive(false);
                promptContainer.SetActive(false);
                ModalClosed();
                if (onNo != null)
                    onNo();
            });

            background.SetActive(true);
            promptContainer.SetActive(true);
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
            textPromptButtonAccept.onClick.AddListener(() => {
                background.SetActive(false);
                textPromptContainer.gameObject.SetActive(false);
                ModalClosed();
                if (onEntered != null)
                    onEntered(textPromptInput.text);
            });

            background.SetActive(true);
            textPromptContainer.gameObject.SetActive(true);
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
            textPromptButtonAccept.onClick.AddListener(() => {
                background.SetActive(false);
                textPromptContainer.gameObject.SetActive(false);
                ModalClosed();
                if (onEntered != null)
                    onEntered(textPromptInput.text);
            });
            textPromptButtonCancel.onClick.RemoveAllListeners();
            textPromptButtonCancel.onClick.AddListener(() => {
                background.SetActive(false);
                textPromptContainer.gameObject.SetActive(false);
                ModalClosed();
                if (onCancelled != null)
                    onCancelled();
            });

            background.SetActive(true);
            textPromptContainer.gameObject.SetActive(true);
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
            ClearAll();
            
            if (progressTitle != null)
                progressTitle.text = title;

            if (progressText != null)
                progressText.text = text;

            if (progressFillImage != null)
                progressFillImage.fillAmount = progress;

            if (progressFillSlider != null)
                progressFillSlider.normalizedValue = progress;

            background.SetActive(true);
            progressContainer.SetActive(true);
        }
        public void HideProgressModal()
        {
            background.SetActive(false);
            progressContainer.SetActive(false);
            ModalClosed();
            progressContainer.SetActive(false);
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

        private object modalLock = new object();
        private List<Action> modalCloseCallbacks = new List<Action>();

        #region Editor-assigned fields

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

        #endregion
    }
}