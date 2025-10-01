using System;
using UnityEngine;
using UnityEngine.UI;

namespace CardGame
{
    public class WinPanel : MonoBehaviour
    {
        [SerializeField] private Button continueButton;
        [SerializeField] private Animation animation;

        private const string OpenAnimName = "WinPanel_Open";
        private const string ContinueAnimName = "WinPanel_Continue";

        private Action _onContinue;

        private void Start()
        {
            continueButton.onClick.AddListener(Continue);
        }

        public void Show(Action onContinue)
        {
            _onContinue = onContinue;
            gameObject.SetActive(true);
            animation.Play(OpenAnimName);
        }

        private void Continue()
        {
            _onContinue?.Invoke();
            animation.Play(ContinueAnimName);
        }

        // This method is called at the end of the Continue animation via Animation Event
        private void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}