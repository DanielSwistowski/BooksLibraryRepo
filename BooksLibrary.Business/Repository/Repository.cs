using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace BooksLibrary.Business.Repository
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        public BookLibraryDataContext context;

        public Repository(BookLibraryDataContext ctx)
        {
            context = ctx;
        }


        public IQueryable<TEntity> Get()
        {
            return context.Set<TEntity>();
        }

        public TEntity GetById(int id)
        {
            return context.Set<TEntity>().Find(id);
        }

        public virtual void Add(TEntity entity)
        {
            context.Set<TEntity>().Add(entity);
        }

        public virtual void Update(TEntity entity)
        {
            context.Set<TEntity>().Attach(entity);
            context.Entry(entity).State = EntityState.Modified;
        }

        public virtual void Delete(TEntity entity)
        {
            if (context.Entry(entity).State == EntityState.Detached)
            {
                context.Set<TEntity>().Attach(entity);
            }
            context.Set<TEntity>().Remove(entity);
        }
    }
}
