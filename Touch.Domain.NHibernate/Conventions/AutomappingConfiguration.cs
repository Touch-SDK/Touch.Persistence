using System;
using FluentNHibernate;
using FluentNHibernate.Automapping;
using Touch.Helpers;
using Touch.Persistence;

namespace Touch.Domain.Conventions
{
    /// <summary>
    /// Automapping configuration.
    /// </summary>
    public class AutomappingConfiguration : DefaultAutomappingConfiguration
    {
        public override bool ShouldMap(Type type)
        {
            if (type.ImplementsInterface(typeof(IEntity)) && !type.IsAbstract)
                return true;

            return false;
        }

        public override bool ShouldMap(Member member)
        {
            if (member.IsProperty)
            {
                if (member.Name == "Identity") return false;
            }

            return base.ShouldMap(member);
        }

        public override bool IsId(Member member)
        {
            if (member.DeclaringType.IsAssignableFrom(typeof(Entity)) || member.DeclaringType.IsAssignableFrom(typeof(BusinessEntity)))
            {
                return member.Name == "Id";
            }

            return base.IsId(member);
        }
    }
}
