using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TLP.UI.Tests
{
    public class MessageBoxTests : MonoBehaviour
    {
        private Coroutine overlayLoadTest;

        public void TestMessageBox()
        {
            MessageBoxes.Instance.ShowMessage("Testing", "This is a simple message.", "OK", () => { Debug.Log("Message box dismissed."); });
        }

        public void TestErrorMessage()
        {
            MessageBoxes.Instance.ShowError("Testing", "FATAL ERROR!", "OK", () => { Debug.Log("Error message dismissed."); });
        }

        public void TestPrompt()
        {
            MessageBoxes.Instance.PromptYesNo("Testing", "You can select an option here:", "Option 1", "Option 2", () => { Debug.Log("Selected option 1."); }, () => { Debug.Log("Selected option 2."); });
        }

        public void TestText()
        {
            MessageBoxes.Instance.PromptText("Testing", "Enter a text:", "OK", (value) => { MessageBoxes.Instance.ShowMessage("She selected the \"" + value + "\"!"); });
        }

        public void TestLoading()
        {
            StartCoroutine(Loading());
        }

        public void TestLoading2()
        {
            if (overlayLoadTest != null)
                StopCoroutine(overlayLoadTest);

            overlayLoadTest = StartCoroutine(Loading2());
        }

        private IEnumerator Loading()
        {
            float duration = 3;
            float start = Time.time;
            while ((Time.time - start) < duration)
            {
                float t = (Time.time - start) / duration;
                MessageBoxes.Instance.ProgressModal(t, "Testing", "Loading " + (t * 100).ToString("00") + "%");

                yield return null;
            }

            MessageBoxes.Instance.HideProgressModal();
            MessageBoxes.Instance.ShowMessage("Loaded!");
        }

        private IEnumerator Loading2()
        {
            float duration = 5;
            float start = Time.time;
            while ((Time.time - start) < duration)
            {
                float t = (Time.time - start) / duration;
                MessageBoxes.Instance.ShowProgressOverlay(t, "Loading " + (t * 100).ToString("00") + "%");

                yield return null;
            }

            MessageBoxes.Instance.HideProgressOverlay();
        }
    }
}