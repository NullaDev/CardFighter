using Entity;
using UnityEngine;
using UnityEngine.UI;

namespace Render
{
    public class CoinRender : MonoBehaviour
    {
        public Text coinText;
        private readonly Player _currentPlayer;

        public CoinRender(Player currentPlayer)
        {
            _currentPlayer = currentPlayer;
        }

        void Start()
        {
            coinText.text = _currentPlayer.GetCurrentCoins().ToString();
            _currentPlayer.OnCoinCountChanged += UpdateCoinRender;
        }

        void UpdateCoinRender(int coinNumber)
        {
            coinText.text = coinNumber.ToString();
        }

        private void OnDestroy()
        {
            _currentPlayer.OnCoinCountChanged -= UpdateCoinRender;
        }
    }
}
