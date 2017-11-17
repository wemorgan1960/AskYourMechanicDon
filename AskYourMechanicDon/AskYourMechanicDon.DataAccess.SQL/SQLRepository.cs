using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using AskYourMechanicDon.Core.Contracts;
using AskYourMechanicDon.Core.Models;
using AskYourMechanicDon.DataAccess.InMemory;

namespace AskYourMechanicDon.DataAccess.SQL
{
    public class SQLRepository<T> : IRepository<T> where T : BaseEntity
    {
        internal DataContext context;
        internal DbSet<T> DbSet;

        public SQLRepository(DataContext context)
        {
            this.context = context;
            this.DbSet = context.Set<T>();
        }

        public IQueryable<T> Collection()
        {
            return DbSet;
        }

        public void Commit()
        {
            context.SaveChanges();
        }

        public void Delete(string Id)
        {
            var t = Find(Id);
            if (context.Entry(t).State == EntityState.Detached)
                DbSet.Attach(t);

            DbSet.Remove(t);
        }

        public T Find(string Id)
        {
            return DbSet.Find(Id);
        }

        public void Insert(T t)
        {
            DbSet.Add(t);
        }

        public void Update(T t)
        {
            DbSet.Attach(t);
            context.Entry(t).State = EntityState.Modified;
        }
    }
}
