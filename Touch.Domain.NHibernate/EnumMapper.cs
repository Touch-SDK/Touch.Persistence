using System;
using System.Data;
using System.Linq;
using NHibernate;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;

namespace Touch.Domain
{
    sealed class EnumMapper<T> : IUserType
    {
        #region .ctor
        public EnumMapper()
        {
            var type = typeof(T);
            _isFlag = type.GetCustomAttributes(typeof(FlagsAttribute), false).FirstOrDefault() != null;
        }
        #endregion

        #region Data
        /// <summary>
        /// Enum is a flag.
        /// </summary>
        private readonly bool _isFlag;
        #endregion

        #region IUserType implementation
        public Boolean IsMutable { get { return false; } }

        public Type ReturnedType { get { return typeof(T); } }

        public SqlType[] SqlTypes { get { return new[] { _isFlag ? SqlTypeFactory.Byte : SqlTypeFactory.Int16 }; } }

        public object NullSafeGet(IDataReader rs, string[] names, object owner)
        {
            var tmp = NHibernateUtil.Int32.NullSafeGet(rs, names[0]).ToString();
            return Enum.Parse(typeof(T), tmp);
        }

        public void NullSafeSet(IDbCommand cmd, object value, int index)
        {
            ((IDataParameter)cmd.Parameters[index]).Value = value ?? DBNull.Value;
        }

        public object DeepCopy(object value)
        {
            return value;
        }

        public object Replace(object original, object target, object owner)
        {
            return original;
        }

        public object Assemble(object cached, object owner)
        {
            return cached;
        }

        public object Disassemble(object value)
        {
            return value;
        }

        public new bool Equals(object x, object y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            return x.Equals(y);
        }

        public int GetHashCode(object x)
        {
            return x == null ? typeof(T).GetHashCode() : x.GetHashCode();
        }
        #endregion
    }
}
