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
    public class ConnectionRepository : IRepository<Connection>
    {
        private readonly ChatApiContext _db;

        public ConnectionRepository(ChatApiContext context)
        {
            _db = context;
        }

        public void Add(Connection entity)
        {
            _db.Connections.Add(entity);
        }

        public void Edit(Connection entity)
        {
            _db.Entry(entity).State = EntityState.Modified;
        }

        public Connection Get(Guid id)
        {
            return _db.Connections.FirstOrDefault(conn => conn.ConnectionId == id);
        }

        public IEnumerable<Connection> GetAll()
        {
            return _db.Connections.ToList();
        }

        public void Remove(Guid id)
        {
            var connection = _db.Connections.FirstOrDefault(conn => conn.ConnectionId == id);
            if (connection != null)
            {
                _db.Connections.Remove(connection);
            }
        }


        public bool Save()
        {
            return (_db.SaveChanges() >= 0);
        }
    }
}

