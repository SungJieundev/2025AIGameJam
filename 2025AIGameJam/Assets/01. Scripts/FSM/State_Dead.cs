using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Dead : IState
{
    readonly UnitBase u;
    readonly StateMachine fsm;
    Vector2 start, end;
    float t;

    const float knockDist = 0.5f, knockTime = 0.2f;

    public State_Dead(UnitBase u, StateMachine fsm)
    {
        this.u = u;
        this.fsm = fsm;
    }

    public void OnEnter()
    {
        u.rb.velocity = Vector2.zero;
        t = 0f;

        float dist = u.dathKnockDist;
        float dur = Mathf.Max(0.01f, u.deathKnockTime);

        start = u.transform.position;
        end = start - u.dir * dist;
        // 넉백 애니메이션 재생

    }

    public void Tick()
    {
        t += Time.deltaTime;
        float a = Mathf.Clamp01(t / u.deathKnockTime);
        u.transform.position = Vector2.Lerp(start, end, a);

        if (a >= 1f)
        {
            u.pendingDeath = false;
            PoolManager.Instance.Push(u);
        }
    }

    public void FixedTick() { }
    public void OnExit() { }
}
