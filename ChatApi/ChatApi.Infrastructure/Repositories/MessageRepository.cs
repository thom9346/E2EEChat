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
    public class MessageRepository : IRepository<Message>
    {
        private readonly ChatApiContext _db;

        public MessageRepository(ChatApiContext context)
        {
            _db = context;
        }
        public void Add(Message entity)
        {
            _db.Messages.Add(entity);
        }

        public void Edit(Message entity)
        {
            _db.Entry(entity).State = EntityState.Modified;
        }

        public Message Get(Guid id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Message> GetAll()
        {
            return _db.Messages.ToList();
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
