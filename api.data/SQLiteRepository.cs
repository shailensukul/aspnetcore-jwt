using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.core;

namespace api.Data
{
    public class SQLiteRepository<T> : IRepository<T> where T : class, IEntity
    {

        public SQLiteRepository()
        {
        }

        public async Task Remove(T item)
        {
            using (var db = new DBContext())
            {
                db.Remove(item);
                await db.SaveChangesAsync();
            }
        }

        public async Task Remove(IEnumerable<T> items)
        {
            using (var db = new DBContext())
            {
                db.RemoveRange(items);
                await db.SaveChangesAsync();
            }
        }

        public async Task<int> Insert(T item)
        {
            using (var db = new DBContext())
            {
                db.Add(item);
                return await db.SaveChangesAsync();
            }
        }

        public async Task<int> Insert(IEnumerable<T> items)
        {
            using (var db = new DBContext())
            {
                db.AddRange(items);
                return await db.SaveChangesAsync();
            }
        }

        public async Task Update(T item)
        {
            using (var db = new DBContext())
            {
                db.Update(item);
                await db.SaveChangesAsync();
            }
        }

        public async Task Update(IEnumerable<T> items)
        {
            using (var db = new DBContext())
            {
                db.UpdateRange(items);
                await db.SaveChangesAsync();
            }
        }

        public async Task<T> Find(params object[] keyValues)
        {
            using (var db = new DBContext())
            {
                return (T)await db.FindAsync(typeof(T), keyValues);
            }
        }
    }
}
