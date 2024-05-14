
namespace ChatApi.Core.Interfaces
{
    public interface IRepository<T>
    {
        IEnumerable<T> GetAll();
        T Get(Guid id);
        void Add(T entity);
        void Edit(T entity);
        void Remove(int id);
        bool Save();
    }
}
