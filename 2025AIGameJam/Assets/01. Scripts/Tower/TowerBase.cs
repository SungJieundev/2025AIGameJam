using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TowerBase : UnitBase
{
    protected override void OnEnable()
    {
        base.OnEnable();
        GetUnitStat().TowerDamageUpgrade(1);
        GetUnitStat().ApplyDamage();
    }
}
