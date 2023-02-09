public interface IStateMachine<TState> where TState : class
{
    void OnChangeState(TState _newState);
    void OnEnter();
    void OnUpdate();
    void OnExit();
}