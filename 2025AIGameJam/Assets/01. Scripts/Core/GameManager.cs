using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int playerMaxHp;
    public int buildingMaxHp;
    public int playerCurMoney { get; private set; }

    public InGameUI inGameUI;

    private int _playerStartMoney = 150;
    [HideInInspector] public int _playerMaxMoney;
    private int _playerIncomePerSecond;
    [HideInInspector] public int _playerIncomeLevel;

    public int damageUpgradeLevel = 0;
    public event Action<int> OnTowerDamageUpgrade;

    [SerializeField] private PoolingListSO _initPoolList;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Multiple GameManager is running");
        }

        Instance = this;
        
        ResetGame();
        CreatePool();
        StartCoroutine(PlayerIncome());
    }

    public void IncreaseDamageLevel()
    {
        if (playerCurMoney < (damageUpgradeLevel + 1) * 150) return;

        UseMoney(damageUpgradeLevel * 150);
        damageUpgradeLevel++;
        OnTowerDamageUpgrade?.Invoke(damageUpgradeLevel);
        inGameUI.UpdateTexts();
    }

    #region Game
    public void ResetGame()
    {
        _playerMaxMoney = 150;
        playerCurMoney = _playerStartMoney;
        _playerIncomePerSecond = 12;
        _playerIncomeLevel = 0;
        damageUpgradeLevel = 0;
        inGameUI.UpdateTexts();

    }

    public void GameClear()
    {
        Debug.Log("Game Clear");
    }

    public void GameOver()
    {
        Debug.Log("Game Over");
    }
    #endregion

    #region Object Pool
    private void CreatePool()
    {
        PoolManager.Instance = new PoolManager(transform);
        _initPoolList.PoolList.ForEach(p =>
        {
            PoolManager.Instance.CreatePool(p.Prefab, p.Count);
        });
    }
    #endregion

    #region Money
    public void UseMoney(int price)
    {
        playerCurMoney -= price;
        inGameUI.UpdateTexts();
    }

    IEnumerator PlayerIncome()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            playerCurMoney += (_playerIncomePerSecond + (_playerIncomeLevel * 3));
            if (playerCurMoney > _playerMaxMoney)
            {
                playerCurMoney = _playerMaxMoney; 
            }
            inGameUI.UpdateTexts();
        }
    }

    public void PlayerIncomeLevelUp()
    {
        int price = 100 * (_playerIncomeLevel + 1);
        if (playerCurMoney < price) return;
        if (_playerIncomeLevel >= 7) return;

        playerCurMoney -= price;
        _playerIncomeLevel++;
        _playerMaxMoney += 200; 
        inGameUI.UpdateTexts();

    }
    #endregion
}