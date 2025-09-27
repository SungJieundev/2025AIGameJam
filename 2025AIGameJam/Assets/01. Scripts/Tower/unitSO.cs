using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New unitSO", menuName = "SO/unitSO")]
public class UnitSO : ScriptableObject
{
    [Header("unit Stat")]
    public int      unitId;
    public string   unitName;
    [TextArea]public string   unitDesc;
    public int      unitPrice;
    public float    cooldown;
    public Sprite   unitRealImage;
    public float    unitHp;
    public float    unitDamage;
    public float    unitRange;
    public float    unitMoveSpeed;
    public float    unitAttackSpeed;


    [Header("Attack")]
    public Enums.AttackKind attackKind;
    [Tooltip("원거리일 때 투사체 프리팹(선택)")]
    public GameObject projectile;
    [Tooltip("원거리 범위공격일 때 공격 거리(선택)")]
    public float aoeRange;
    [Tooltip("범위공격일 때 반경(선택)")]
    public float aoeRadius;
    [Tooltip("원거리 (범위)공격 타이밍 지연(선택)")]
    public float rangeHitDelay;
    [Tooltip("근접 히트 타이밍 지연(선택)")]
    public float meleeHitDelay;

    [Header("Protocol Attack")]
    [Tooltip("첫번째 공격 딜레이")]
    public float Hit1Deley;
    [Tooltip("두번째 공격 딜레이")]
    public float Hit2Deley;
    [Tooltip("세번째 공격 딜레이")]
    public float Hit3Deley;
    [Tooltip("네번째 공격 딜레이")]
    public float Hit4Deley;

}
