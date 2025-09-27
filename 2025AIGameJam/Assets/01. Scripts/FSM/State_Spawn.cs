using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Spawn : IState
{
    readonly UnitBase u;
    readonly StateMachine fsm;

    float t, wait = 0.05f;

    public State_Spawn(UnitBase u, StateMachine fsm)
    {
        this.u = u;
        this.fsm = fsm;
    }

    public void OnEnter()
    {
        t = 0f;
        u.rb.velocity = Vector2.zero;
    }

    public void Tick()
    {
        t += Time.deltaTime;
        if (t >= wait) u.GoMarch();
    }

    public void FixedTick() { }
    public void OnExit() { }
}
