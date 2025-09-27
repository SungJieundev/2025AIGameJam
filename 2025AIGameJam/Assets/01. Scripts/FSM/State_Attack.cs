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
        if (u.isAttacking) return; // 공격 중이면 타겟 유무와 상관없이 끝날 때까지 대기

        if (!u.target || !u.InAttackRange(u.target))
        {
            u.GoMarch();
            return;
        }
        else if (u.attack != null && u.attack.CanExecute(u))
            u.attack.Begin(u);
        else
            u.anim.SetTrigger("Idle");
    }

    public void FixedTick() { u.rb.velocity = Vector2.zero; }
    public void OnExit() { }
}
