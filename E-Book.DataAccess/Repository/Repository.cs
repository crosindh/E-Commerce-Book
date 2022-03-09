using E_Book.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace E_Book.DataAccess.Repository
{
    public class Repository<G> : IRepository<G> where G : class
    {
        private readonly ApplicationDbContext _db;
        internal DbSet<G> dbset;
        public Repository(ApplicationDbContext db)
        {
            _db = db;
            this.dbset = _db.Set<G>();
        }
        public void Add(G entity)
        {
            dbset.Add(entity);
        }
        // IncludeProp = "Category,CoverTypes it should work with multiple commas aswell"
        public IEnumerable<G> GetAll(Expression<Func<G, bool>>? filter=null, string? includeProperties = null)
        {
            IQueryable<G> query = dbset;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (includeProperties != null)
            {
                foreach(var IncludeProp in includeProperties.Split(new char[] {','},StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(IncludeProp);
                }
            }
            return query.ToList();
        }

        public G GetFirstorDeafult(Expression<Func<G, bool>> filter, string? includeProperties = null)
        {
            IQueryable<G> query = dbset;
            query = query.Where(filter);
            if (includeProperties != null)
            {
                foreach (var IncludeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(IncludeProp);
                }
            }
            return query.FirstOrDefault();
        }

        public void Remove(G entity)
        {
            dbset.Remove(entity);
        }

        public void RemoveRange(IEnumerable<G> entity)
        {
            dbset.RemoveRange(entity);
        }
    }
}
