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
    public GameObject clearUI;
    public GameObject overUI;

    public string bgm;

    private bool gameClear = false;
    private bool gameOver = false;
    
    
    private int _playerStartMoney = 150;
    [HideInInspector] public int _playerMaxMoney;
    private int _playerIncomePerSecond;
    [HideInInspector] public int _playerIncomeLevel;

    public float maxProtocolPower = 1000f;
    public float curProtocolPower;

    public int damageUpgradeLevel = 0;
    public event Action<int> OnTowerDamageUpgrade;

    [SerializeField] private PoolingListSO _initPoolList;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Multiple GameManager is running");
        }
        AudioManager.Instance.PlaySystem("GameStart");
        AudioManager.Instance.PlayBGM(bgm);

        Instance = this;
        ResetGame();
        CreatePool();
        StartCoroutine(PlayerIncome());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F12))
            GameOver();

        if (Input.GetKeyDown(KeyCode.F11))
            GameClear();

        if (Input.GetKeyDown(KeyCode.F10))
        {
            Time.timeScale = 2f;
        }
        
        if (Input.GetKeyDown(KeyCode.F9))
        {
            Time.timeScale = 1f;
        }
        
        if (Input.GetKeyDown(KeyCode.F8))
        {
            Time.timeScale = 0f;
        }
    }

    public void IncreaseDamageLevel()
    {
        if (playerCurMoney < (damageUpgradeLevel + 1) * 150) return;

        UseMoney(damageUpgradeLevel * 150);
        damageUpgradeLevel++;
        AudioManager.Instance.PlaySystem("DamageUpgradeAudio");
        OnTowerDamageUpgrade?.Invoke(damageUpgradeLevel);
        inGameUI.UpdateTexts();
    }

    #region Game
    public void ResetGame()
    {
        _playerMaxMoney = 300;
        playerCurMoney = _playerStartMoney;
        _playerIncomePerSecond = 12;
        _playerIncomeLevel = 0;
        damageUpgradeLevel = 0;
        maxProtocolPower = 1000f;
        curProtocolPower = 0f;
        inGameUI.UpdateTexts();

    }

    public void GameClear()
    {
        if (gameClear) return;
        
        gameClear = true;
        Debug.Log("Game Clear");
        AudioManager.Instance.PauseBGM();
        AudioManager.Instance.PlaySystem("DestroyEnemyBase");
        Delay(2f);
        AudioManager.Instance.PlaySystem("ClearStage");
        clearUI.SetActive(true);
        
    }

    IEnumerator Delay(float i)
    {
        yield return new WaitForSeconds(i);
    }

    public void GameOver()
    {
        if (gameOver) return;
        
        gameOver = true;
        Debug.Log("Game Over");
        AudioManager.Instance.PauseBGM();
        AudioManager.Instance.PlaySystem("DefeatAudio");
        Delay(2f);
        overUI.SetActive(true);

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

    public void GainProtocolPower(float amount)
    {
        curProtocolPower += amount;
        if (curProtocolPower > maxProtocolPower)
            curProtocolPower = maxProtocolPower;

        Debug.Log("Protocol Power: " + curProtocolPower);
    }

    public void UseProtocolPower(float amount)
    {
        curProtocolPower -= amount;
        if (curProtocolPower < 0f)
            curProtocolPower = 0f;
    }

}