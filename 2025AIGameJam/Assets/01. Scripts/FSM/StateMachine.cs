using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    public IState Current { get; private set; }
    public event Action<IState> OnStateChanged;

    public void SetState(IState next)
    {
        if (Current == next) return;
        Current?.OnExit();
        Current = next;
        Current?.OnEnter();
        OnStateChanged?.Invoke(Current);
    }

    public void Tick() => Current?.Tick();
    public void FixedTick() => Current?.FixedTick();
}
