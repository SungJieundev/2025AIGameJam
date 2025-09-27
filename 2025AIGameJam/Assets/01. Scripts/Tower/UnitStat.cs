using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UnitStat
{
    public UnitSO unitSO;
    public int unitId;
    public float unitMaxHp;
    public float unitCurHp;
    public float unitBaseDamage;
    public float unitAddDamage;
    public float unitTotalDamage;
    public float unitRange;
    public float unitMoveSpeed;
    public float unitAttackSpeed;

    public UnitStat(UnitSO unitSO)
    {
        UnitReset(unitSO);
    }

    public void UnitReset(UnitSO unitSO)
    {
        this.unitId = unitSO.unitId;
        this.unitMaxHp = unitSO.unitHp;
        this.unitCurHp = unitMaxHp;
        this.unitBaseDamage = unitSO.unitDamage;
        this.unitAddDamage = 0;
        this.unitTotalDamage = unitSO.unitDamage;
        this.unitRange = unitSO.unitRange;
        this.unitMoveSpeed = unitSO.unitMoveSpeed;
        this.unitAttackSpeed = unitSO.unitAttackSpeed;
    }

    public void TowerDamageUpgrade(int level)
    {
        unitAddDamage = unitBaseDamage * (level * 0.1f);
    }

    public void ApplyDamage()
    {
        float f = this.unitBaseDamage + this.unitAddDamage;
        this.unitTotalDamage = (int)Mathf.Floor(f);
        Debug.Log(unitTotalDamage);
    }
}
