using System;
using DefaultNamespace;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildButton : MonoBehaviour, IPointerDownHandler
{
    [SerializeField]
    private GameTileContentType _type;

    [SerializeField] private TextMeshProUGUI _cost;
    [SerializeField] private GameObject _overlay;
    

    private Action<GameTileContentType> _listenerAction;

    private void Awake()
    {
        _cost.text = GetCost(_type).ToString();
        PlayerWallet.Updated += CheckBuyOpportunity;
        CheckBuyOpportunity();
    }

    private void OnDestroy()
    {
        PlayerWallet.Updated -= CheckBuyOpportunity;
    }

    private void CheckBuyOpportunity()
    {
        _overlay.SetActive(GetCost(_type) > PlayerWallet.Amount);
    }

    public void AddListener(Action<GameTileContentType> listenerAction)
    {
        _listenerAction = listenerAction;
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        if (_overlay.gameObject.activeSelf)
            return;
        
        _listenerAction?.Invoke(_type);
    }

    public static int GetCost(GameTileContentType tile)
    {
        switch (tile)
        {
            case GameTileContentType.Ice:
                return 35;
            case GameTileContentType.Wall:
                return 15;
            case GameTileContentType.LaserTower:
                return 40;
            case GameTileContentType.MortarTower:
                return 50;
            default:
                throw new ArgumentOutOfRangeException(nameof(tile), tile, null);
        }
    }
}