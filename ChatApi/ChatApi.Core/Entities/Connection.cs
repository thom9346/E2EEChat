﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApi.Core.Entities
{
    public class Connection
    {
        public Guid ConnectionId { get; set; }
        public string ConnectionString { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}
