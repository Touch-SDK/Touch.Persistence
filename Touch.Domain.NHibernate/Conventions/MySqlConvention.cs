﻿using System;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace Touch.Domain.Conventions
{
    /// <summary>
    /// MySQL mapping convention.
    /// </summary>
    sealed public class MySqlConvention : IPropertyConvention
    {
        public void Apply(IPropertyInstance instance)
        {
            if (instance.Type == typeof(Guid) || instance.Type == typeof(Guid?))
            {
                instance.CustomSqlType("CHAR(36)");
            }
        }
    }
}
