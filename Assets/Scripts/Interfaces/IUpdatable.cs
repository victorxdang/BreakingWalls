
public interface IUpdatable
{
    bool ShouldUpdate { get; }

    void OnUpdate();
}
