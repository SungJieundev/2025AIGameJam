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

    public void OnEnter() { Debug.Log("March"); }
    public void Tick()
    {
        ref float scanTimer = ref u.ScanTimerRef();
        scanTimer -= Time.deltaTime;
        if (scanTimer <= 0f)
        {
            u.ScanTarget();
            scanTimer = u.scanInterval;
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

    public void OnExit() { u.rb.velocity = Vector2.zero; }
}
