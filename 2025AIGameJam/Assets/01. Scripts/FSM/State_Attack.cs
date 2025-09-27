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
        Debug.Log("Attack");
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

        if (!u.isAttacking && u.attack != null && u.attack.CanExecute(u))
        {
            u.attack.Begin(u);
        }
    }

    public void FixedTick() { u.rb.velocity = Vector2.zero; }
    public void OnExit() { }
}
