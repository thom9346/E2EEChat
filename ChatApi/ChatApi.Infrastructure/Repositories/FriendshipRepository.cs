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
    public class FriendshipRepository : IRepository<Friendship>
    {
        private readonly ChatApiContext _db;

        public FriendshipRepository(ChatApiContext context)
        {
            _db = context;
        }

        public void Add(Friendship entity)
        {
            _db.Friendships.Add(entity);
        }

        public void Edit(Friendship entity)
        {
            _db.Entry(entity).State = EntityState.Modified;
        }

        public Friendship Get(Guid id)
        {
            return _db.Friendships.FirstOrDefault(f => f.Id == id);
        }

        public IEnumerable<Friendship> GetAll()
        {
            return _db.Friendships.ToList();
        }

        public void Remove(int id)
        {
            throw new NotImplementedException();
        }

        public bool Save()
        {
            return (_db.SaveChanges() >= 0);
        }
    }
}
