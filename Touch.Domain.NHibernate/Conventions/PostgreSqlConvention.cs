using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;

namespace Touch.Domain.Conventions
{
    /// <summary>
    /// PostgreSQL mapping convention.
    /// </summary>
    sealed public class PostgreSqlConvention : IPropertyConvention
    {
        public void Apply(IPropertyInstance instance)
        {
            if (instance.Type == typeof(string) && ((IPropertyInspector)instance).Length > 255)
            {
                instance.CustomSqlType("text");
            }
        }
    }
}
