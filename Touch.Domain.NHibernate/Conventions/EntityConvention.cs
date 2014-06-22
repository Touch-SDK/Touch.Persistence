using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;
using Touch.Helpers;
using Touch.Persistence;

namespace Touch.Domain.Conventions
{
    /// <summary>
    /// Entity mapping convention.
    /// </summary>
    sealed public class EntityConvention : IPropertyConvention, IIdConvention, IHasManyConvention, IHasManyToManyConvention, IHasOneConvention, IReferenceConvention
    {
        public void Apply(IPropertyInstance instance)
        {
            if (instance.Type.GetUnderlyingSystemType().IsNullable())
                instance.Nullable();
            else if (typeof (IEntity).IsAssignableFrom(instance.Property.PropertyType))
                instance.Nullable();
            else
                instance.Not.Nullable();

            if (instance.EntityType.ImplementsInterface(typeof(IBusinessEntity)))
            {
                if (instance.Name == "IsRemoved")
                {
                    instance.Index(string.Format("IX_{0}_IsRemoved", instance.EntityType.Name).ToUpper());
                }
            }

            if (instance.Type.IsEnum)
            {
                var generic = typeof (EnumMapper<>);
                var typeArgs = new[] {instance.Property.PropertyType};
                var constructed = generic.MakeGenericType(typeArgs);

                instance.CustomType(constructed);
            }
        }

        public void Apply(IOneToManyCollectionInstance instance)
        {
            instance.Cascade.All();
            instance.Inverse();
            instance.LazyLoad();
        }

        public void Apply(IManyToManyCollectionInstance instance)
        {
            instance.Relationship.ForeignKey(string.Format("FK_M_{0}_{1}", instance.EntityType.Name, instance.ChildType.Name).ToUpper());
            instance.Key.ForeignKey(string.Format("FK_M_{0}_{1}", instance.ChildType.Name, instance.EntityType.Name).ToUpper());

            instance.Cascade.All();
            instance.LazyLoad();
        }

        public void Apply(IIdentityInstance instance)
        {
            if (typeof(IBusinessEntity).IsAssignableFrom(instance.EntityType))
                instance.GeneratedBy.GuidComb();
            else
                instance.GeneratedBy.Native();
        }

        public void Apply(IManyToOneInstance instance)
        {
            instance.ForeignKey(string.Format("FK_{0}_{1}", instance.EntityType.Name, instance.Name).ToUpper());
            instance.Index(string.Format("IX_{0}_{1}", instance.EntityType.Name, instance.Name).ToUpper());

            instance.Nullable();
            instance.LazyLoad();
        }

        public void Apply(IOneToOneInstance instance)
        {
            instance.ForeignKey(string.Format("FK_{0}_{1}", instance.EntityType.Name, instance.Name).ToUpper());
        }
    }
}
