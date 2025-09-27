using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Attack : IState
{
    readonly UnitBase u;
    readonly StateMachine fsm;

    public State_Attack(UnitBase u, StateMachine fsm)
    {
        this.u = u;
        this.fsm = fsm;
    }

    public void OnEnter()
    {
        
        u.rb.velocity = Vector2.zero;
        u.AttackEnterFromFSM();
    }

    public void Tick()
    {
        if (!u.target || !u.InAttackRange(u.target))
        {
            u.GoMarch();
            return;
        }

        if (!u.isAttacking && u.attack != null)
        {
            if (u.attack.CanExecute(u))
            {
                // 공격 가능 → 공격 실행
                u.attack.Begin(u);
            }
            else
            {
                // 사거리 안인데 쿨타임 중 → Idle 애니 출력
                u.anim.SetTrigger("Idle");
            }
        }
    }

    public void FixedTick() { u.rb.velocity = Vector2.zero; }
    public void OnExit() { }
}
