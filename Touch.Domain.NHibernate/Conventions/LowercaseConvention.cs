using System.Linq;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;

namespace Touch.Domain.Conventions
{
    /// <summary>
    /// Lowercase naming convention.
    /// </summary>
    sealed public class LowercaseConvention : IClassConvention, IClassConventionAcceptance, IHasManyToManyConvention, IReferenceConvention, IHasManyConvention, IPropertyConvention, IIdConvention
    {
        public void Accept(IAcceptanceCriteria<IClassInspector> criteria)
        {
            criteria.Expect(x => x.TableName, Is.Set);
        }

        public void Apply(IClassInstance instance)
        {
            instance.Table(instance.EntityType.Name.ToLower());
        }

        public void Apply(IManyToManyCollectionInstance instance)
        {
            instance.Table(instance.EntityType.Name.ToLower() + "_" + instance.ChildType.Name.ToLower());

            instance.Key.Column(instance.EntityType.Name.ToLower() + "_id");
            instance.Relationship.Column(instance.Relationship.StringIdentifierForModel.ToLower() + "_id");
        }

        public void Apply(IManyToOneInstance instance)
        {
            instance.Column(instance.Property.PropertyType.Name.ToLower() + "_id");
        }

        public void Apply(IOneToManyCollectionInstance instance)
        {
            instance.Key.Column(instance.EntityType.Name.ToLower() + "_id");
        }

        public void Apply(IPropertyInstance instance)
        {
            instance.Column(instance.Name.ToLower());
        }

        public void Apply(IIdentityInstance instance)
        {
            instance.Column("id");
        }
    }
}
