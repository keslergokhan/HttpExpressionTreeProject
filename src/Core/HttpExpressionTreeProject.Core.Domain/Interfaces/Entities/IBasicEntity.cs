﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpExpressionTreeProject.Core.Domain.Interfaces.Entities
{
    public interface IBasicEntity
    {
        public Guid Id { get; set; }
        public byte State { get; set; }
    }
}
