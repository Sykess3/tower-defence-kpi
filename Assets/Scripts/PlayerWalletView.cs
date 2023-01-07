using System.Collections;
using TMPro;
using UnityEngine;

namespace DefaultNamespace
{
    public class PlayerWalletView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _textMeshPro;
        private float _timeLeftToIncrease;

        private void Awake()
        {
            PlayerWallet.UpdatedWithValues += Count;
            _timeLeftToIncrease = 5;
            _textMeshPro.text = $"Coins: {PlayerWallet.Amount}";
        }
        

        private void OnDestroy()
        {
            PlayerWallet.UpdatedWithValues -= Count;        }

        private void Update()
        {
            if (ProjectContext.Instance.PauseManager.IsPaused)
                return;

            _timeLeftToIncrease -= Time.deltaTime;
            if (_timeLeftToIncrease <= 0)
            {
                PlayerWallet.Increase(2);
                _timeLeftToIncrease = 5;
            }
        }
        
         private IEnumerator CountNegativeAwaitable(int from, int minus,  float countingDuration = 1)
        {
            float timeSpent = 0;

            while (timeSpent < countingDuration)
            {
                float value = timeSpent / countingDuration;
                int amount = from - (int)(value * (double)minus);
                _textMeshPro.text = $"Coins: {amount.ToString()}";
                timeSpent += Time.deltaTime;

                yield return null;
            }

            _textMeshPro.text = $"Coins: {(from - minus).ToString()}";
        }

        private IEnumerator CountPositiveAwaitable(int from, int add, float countingDuration = 1)
        {
            float timeSpent = 0;

            while (timeSpent < countingDuration)
            {
                float value = timeSpent / countingDuration;
                int amount = from + (int)(value * (double)add);
                _textMeshPro.text = $"Coins: {amount.ToString()}";
                timeSpent += Time.deltaTime;

                yield return null;
            }

            _textMeshPro.text = $"Coins: {(from + add).ToString()}";
        }
        
        

        private void Count(int oldValue, int newValue)
        {
            int difference = newValue - oldValue;
            if (difference > 0)
            {
                StartCoroutine(CountPositiveAwaitable(oldValue, difference, 0.5f));
            }
            else
            {
                StartCoroutine(CountNegativeAwaitable(oldValue, -difference, 0.5f));
            }
        }

    }
}