using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InGameUI : MonoBehaviour
{
    public TMP_Text goldText;

    public TMP_Text incomUpgradePrice;
    public TMP_Text incomLevel;
    public TMP_Text towerUpgradePrice;
    public TMP_Text towerUpgradeLevel;
    public TowerSpawner towerSpawner;

    public TowerButtonUI[] towerButtons;

    private void Start()
    {
        for (int i = 0; i < towerButtons.Length; i++)
        {
            UnitBase unit = towerSpawner.towerList[i].GetComponent<UnitBase>();
            towerButtons[i].Init(unit.unitSO);
        }
    }
    public void UpdateTexts()
    {
        goldText.text = GameManager.Instance.playerCurMoney.ToString() + " / " + GameManager.Instance._playerMaxMoney;
        incomUpgradePrice.text = ((GameManager.Instance._playerIncomeLevel + 1) * 100).ToString();
        incomLevel.text = "LEVEL " + (GameManager.Instance._playerIncomeLevel+1).ToString();

        towerUpgradePrice.text = ((GameManager.Instance.damageUpgradeLevel + 1) * 150).ToString();
        towerUpgradeLevel.text = "LEVEL " + (GameManager.Instance.damageUpgradeLevel+1).ToString();
    
        foreach (var btn in towerButtons)
        {
            bool affordable = GameManager.Instance.playerCurMoney >= int.Parse(btn.priceText.text);
            btn.SetPriceColor(affordable);
        }
    }

    public void StartTowerCooldown(int index, float duration)
    {
        towerButtons[index].StartCooldown(duration);
    }
}
