using System;
using UnityEngine;
using UnityEngine.UI;

namespace GameResult
{
    [RequireComponent(typeof(Canvas))]
    public class GameResultWindow : MonoBehaviour
    {
        //[SerializeField]
        //private GameResultIntroAnimation _introAnimation;
        [SerializeField]
        private Button _restartButton;
        [SerializeField]
        private Button _quitButton;
        [SerializeField] private Image _resultImage;
        [SerializeField] private Sprite _win;
        [SerializeField] private Sprite _lose;

        private Canvas _canvas;

        private Action _onRestart;
        private Action _onQuit;
        
        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
            _canvas.enabled = false;
            _restartButton.onClick.AddListener(OnRestartClicked);
            _quitButton.onClick.AddListener(OnQuitClicked);
        }
        
        public async void Show(GameResultType result, Action onRestart, Action onQuit)
        {
            _onRestart = onRestart;
            _onQuit = onQuit;
            
            _restartButton.interactable = false;
            _quitButton.interactable = false;
            
            _canvas.enabled = true;
            EnableResult(result);
            //await _introAnimation.Play(result);

            _restartButton.interactable = true;
            _quitButton.interactable = true;
        }

        private void EnableResult(GameResultType result)
        {
            _resultImage.gameObject.SetActive(true);
            _resultImage.sprite = result == GameResultType.Defeat
                ? _lose
                : _win;
        }

        private void OnRestartClicked()
        {
            _onRestart?.Invoke();
            _canvas.enabled = false;
            DisableResult();
        }

        private void OnQuitClicked()
        {
            _onQuit?.Invoke();
            _canvas.enabled = false;
            DisableResult();
        }

        private void DisableResult() => 
            _resultImage.gameObject.SetActive(false);
    }
}
