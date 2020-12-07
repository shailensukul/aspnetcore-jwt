using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace api.core
{
    public interface IRepository<T> where T : class, IEntity
    {

        Task Update(T item);

        Task Update(IEnumerable<T> items);

        Task<int> Insert(T item);

        Task<int> Insert(IEnumerable<T> items);

        Task Remove(T item);
        Task Remove(IEnumerable<T> items);

        Task<T> Find(params object[] keyValues);


    }
}
