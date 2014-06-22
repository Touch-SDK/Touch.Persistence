using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;

namespace Touch.Domain.Conventions
{
    /// <summary>
    /// SQL Server mapping convention.
    /// </summary>
    sealed public class MsSqlConvention : IPropertyConvention
    {
        public void Apply(IPropertyInstance instance)
        {
            if (instance.Type == typeof(string) && ((IPropertyInspector)instance).Length > 255)
            {
                instance.CustomSqlType("ntext");
            }
        }
    }
}
