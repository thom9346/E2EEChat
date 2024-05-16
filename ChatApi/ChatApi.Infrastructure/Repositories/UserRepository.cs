using ChatApi.Core.Entities;
using ChatApi.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApi.Infrastructure.Repositories
{
    public class UserRepository : IRepository<User>
    {
        private readonly ChatApiContext _db;

        public UserRepository(ChatApiContext context)
        {
            _db = context;
        }
        public void Add(User entity)
        {
            _db.Users.Add(entity);
        }

        public void Edit(User entity)
        {
            _db.Entry(entity).State = EntityState.Modified;
        }

        public User Get(Guid id)
        {
            return _db.Users.FirstOrDefault(user => user.UserId == id);
        }

        public IEnumerable<User> GetAll()
        {
            return _db.Users.ToList();
        }

        public void Remove(Guid id)
        {
            throw new NotImplementedException();
        }
        public bool Save()
        {
            return (_db.SaveChanges() >= 0);
        }
    }
}
