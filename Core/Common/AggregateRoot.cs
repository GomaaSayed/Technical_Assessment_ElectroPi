namespace Technical_Assessment_ElectroPi.Core.Common;

public abstract class AggregateRoot<TId> : Entity<TId>
{
    protected AggregateRoot()
    {
    }

    protected AggregateRoot(TId id)
        : base(id)
    {
    }
}