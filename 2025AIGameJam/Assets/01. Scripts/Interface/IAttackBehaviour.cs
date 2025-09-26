using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttackBehaviour
{
    bool CanExecute(UnitBase u); // 사거리/쿨타임 충족?
    void Begin(UnitBase u);      // 애니 트리거/사전 준비
    void OnHit(UnitBase u);      // 실제 피해/투사체/AOE
    void End(UnitBase u);
}
