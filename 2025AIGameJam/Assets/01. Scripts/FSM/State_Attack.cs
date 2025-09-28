using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Attack : IState
{
    readonly UnitBase u;
    readonly StateMachine fsm;
    float _enterGraceUntil; // 진입 유예 시간

    public State_Attack(UnitBase u, StateMachine fsm) { this.u = u; this.fsm = fsm; }

    public void OnEnter()
    {
        u.rb.velocity = Vector2.zero;
        _enterGraceUntil = Time.time + 0.1f; // 0.1초 유예

        // 입장 즉시 1회 스캔 + 다음 스캔도 바로 가능하게
        ref float scanTimer = ref u.ScanTimerRef();
        scanTimer = 0f;
        u.ScanTarget();
    }

    public void Tick()
    {
        // 공격 애니 중이면 입력/스캔/전이 정지 (중복 Begin 방지)
        if (u.isAttacking) return;

        // 공격 상태에서도 주기적으로 스캔 (March와 동일 규칙)
        ref float scanTimer = ref u.ScanTimerRef();
        scanTimer -= Time.deltaTime;
        if (scanTimer <= 0f)
        {
            u.ScanTarget();
            scanTimer += u.scanInterval;  // 초과분 보존
        }

        bool outOfRangeOrNoTarget = (!u.target || !u.InAttackRange(u.target));

        // 유예 시간 동안은 March로 튀지 말고 타깃 기회를 더 준다
        if (outOfRangeOrNoTarget)
        {
            if (Time.time < _enterGraceUntil)
            {
                u.anim.SetTrigger("Idle"); // 대기
                return;
            }
            else
            {
                // 유예 끝나고도 여전히 타깃이 없거나 사거리 밖이면 March
                u.GoMarch();
                return;
            }
        }

        // 여기까지 왔으면 사거리 내 타깃 보유 → 쿨다운만 통과하면 공격
        if (u.attack != null && u.attack.CanExecute(u))
        {
            u.attack.Begin(u);
        }
        else
        {
            u.anim.SetTrigger("Idle");
        }
    }

    public void FixedTick() { u.rb.velocity = Vector2.zero; }
    public void OnExit() { }
}
