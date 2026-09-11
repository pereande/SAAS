namespace ERP.Shared.Entities;

public interface IEntity<TKey>
{
    TKey Id { get; set; }
}
