using E_Book.DataAccess.Repository.IRepository;
using E_Book.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Book.DataAccess.Repository
{
    public class CategoryRespository : Repository<Category>, ICategoryRepository
    {
        private ApplicationDbContext _db;
        public CategoryRespository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(Category obj)
        {
            _db.Update(obj);
        }
    }
}
