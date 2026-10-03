using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerButtonUI : MonoBehaviour
{
    public int towerIndex;
    public TMP_Text priceText;
    public Image cooldownMask;

    private UnitSO unitSO;
    private float cooldownEndTime = -1f;

    public void Init(UnitSO so)
    {
        unitSO = so;
        priceText.text = so.unitPrice.ToString();
        cooldownMask.fillAmount = 0f;
    }
    public void SetPriceColor(bool affordable)
    {
        priceText.color = affordable ? Color.white : Color.red;
    }

    public void StartCooldown(float duration)
    {
        cooldownEndTime = Time.time + duration;
        cooldownMask.fillAmount = 1f;
    }

    void Update()
    {
        if (cooldownEndTime > 0)
        {
            float remain = cooldownEndTime - Time.time;
            if (remain <= 0)
            {
                cooldownMask.fillAmount = 0;
                cooldownEndTime = -1f;
                AudioManager.Instance.PlaySFX("CanSpawnAudio", AudioManager.Instance.sfxPlayer);
            }
            else
            {
                cooldownMask.fillAmount = remain / unitSO.cooldown;
            }
        }
    }
}
