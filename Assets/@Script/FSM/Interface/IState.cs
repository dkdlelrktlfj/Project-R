public interface IState
{
    void OnChangeState();
    void OnEnter();
    void OnUpdate();
    void OnExit();
}