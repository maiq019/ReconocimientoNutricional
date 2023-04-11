using System.Collections;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using WhateverDevs.Core.Runtime.Common;

namespace ITCL.VisionNutricional.Runtime.UI
{
    /// <summary>
    /// Singleton that controls the Loading Screen.
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class LoadingScreen : Singleton<LoadingScreen>
    {
        /// <summary>
        /// Duration of fading.
        /// </summary>
        [SerializeField]
        private float FadeDuration = .5f;

        /// <summary>
        /// Reference to the title text.
        /// </summary>
        [SerializeField]
        private TMP_Text TitleText;
        
        /// <summary>
        /// Reference to the debug text.
        /// </summary>
        [SerializeField]
        private TMP_Text DebugText;

        /// <summary>
        /// Reference to the canvas group to be able to show and hide the panel.
        /// </summary>
        private CanvasGroup CanvasGroup
        {
            get
            {
                if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
                return canvasGroup;
            }
        }

        /// <summary>
        /// Backfield for CanvasGroup.
        /// </summary>
        private CanvasGroup canvasGroup;

        /// <summary>
        /// Fade in the loading screen.
        /// </summary>
        public static IEnumerator FadeIn()
        {
            Instance.TitleText.SetText("");
            Instance.DebugText.SetText("");
            return Instance.FadeInRoutine();
        }

        /// <summary>
        /// Test the fade in routine.
        /// </summary>
        [Button("Fade in")]
        [HideInEditorMode]
        private void TestFadeIn() => StartCoroutine(FadeInRoutine());

        /// <summary>
        /// Routine to fade into the screen.
        /// </summary>
        private IEnumerator FadeInRoutine()
        {
            CanvasGroup.alpha = 0;

            CanvasGroup.interactable = true;

            CanvasGroup.blocksRaycasts = true;

            yield return CanvasGroup.DOFade(1, FadeDuration).SetEase(Ease.OutExpo).WaitForCompletion();
        }

        /// <summary>
        /// Fade out the loading screen.
        /// </summary>
        public static IEnumerator FadeOut() => Instance.FadeOutRoutine();

        /// <summary>
        /// Test the fade out routine.
        /// </summary>
        [Button("Fade out")]
        [HideInEditorMode]
        private void TestFadeOut() => StartCoroutine(FadeOutRoutine());

        /// <summary>
        /// Routine to fade out of the screen.
        /// </summary>
        private IEnumerator FadeOutRoutine()
        {
            CanvasGroup.alpha = 1;

            yield return CanvasGroup.DOFade(0, FadeDuration).SetEase(Ease.InExpo).WaitForCompletion();

            CanvasGroup.interactable = false;

            CanvasGroup.blocksRaycasts = false;
        }
        
        /// <summary>
        /// Sets the title text.
        /// </summary>
        /// <param name="text">Text to set.</param>
        public static void SetTitleText(string text) => Instance.TitleText.SetText(text);

        /// <summary>
        /// Sets the debug text.
        /// </summary>
        /// <param name="text">Text to set.</param>
        public static void SetDebugText(string text) => Instance.DebugText.SetText(text);
    }
}