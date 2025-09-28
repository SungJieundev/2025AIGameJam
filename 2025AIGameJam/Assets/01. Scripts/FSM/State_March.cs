using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_March : IState
{
    readonly UnitBase u;
    readonly StateMachine fsm;

    public State_March(UnitBase u, StateMachine fsm)
    {
        this.u = u;
        this.fsm = fsm;
    }

    public void OnEnter()
    {
        u.anim.SetTrigger("Move");
        ref float scanTimer = ref u.ScanTimerRef();
        scanTimer = 0f;          // 바로 스캔 가능하도록
        u.ScanTarget();          // 입장 즉시 1회 스캔
    }

    public void Tick()
    {
        // 주기 스캔
        ref float scanTimer = ref u.ScanTimerRef();
        scanTimer -= Time.deltaTime;
        if (scanTimer <= 0f)
        {
            // 누산기 패턴은 while로 돌리면 더 정확하지만,
            // 여기선 간단히 1회만: scanInterval이 충분히 작으므로 문제 없음
            u.ScanTarget();
            scanTimer += u.scanInterval;  // 초과분 보존
        }

        if (u.target && u.InAttackRange(u.target))
        {
            u.GoAttack();
        }
    }

    public void FixedTick()
    {
        u.rb.velocity = u.dir * u.GetUnitStat().unitMoveSpeed;
    }

    public void OnExit()
    {
        u.rb.velocity = Vector2.zero;
    }
}
