using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FiniteStateMachine<TState> : IStateMachine<TState> where TState : class, IState
{
    protected TState currentState;

    public FiniteStateMachine(TState _defaultState)
    {
        if (_defaultState == null)
        {
            Utility.Log($"기본 스테이트는 null일 수 없음", Color.red);
            var exception = new System.NullReferenceException("기본 스테이트는 null일 수 없음");
            ExceptionManager.SetCriticalException(exception);
            throw exception;
        }

        this.currentState = _defaultState;
    }

    public virtual void OnChangeState(TState _newState)
    {
        if(_newState == null)
        {
            Utility.Log($"변경하고자 하는 스테이트가 Null 임", Color.red);
            return;
        }

        this.currentState.OnExit();
        this.currentState = _newState;
        this.currentState.OnEnter();
    }

    public virtual void OnEnter()
    {
        currentState.OnEnter();
    }

    public virtual void OnUpdate()
    {
        currentState.OnUpdate();
    }

    public virtual void OnExit()
    {
        currentState.OnExit();
    }
}
