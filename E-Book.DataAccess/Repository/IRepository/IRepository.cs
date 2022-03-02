using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace E_Book.DataAccess.Repository.IRepository
{
    public interface IRepository<G> where G : class
    {
        //G - Generic Interface For CRUD
        G GetFirstorDeafult(Expression<Func<G, bool>> filter, string? includeProperties = null);
        IEnumerable<G> GetAll(string? includeProperties = null);
        void Add(G entity);
        void Remove(G entity);
        void RemoveRange(IEnumerable<G> entity);

    }
}
