using System;
using System.Runtime.CompilerServices;

namespace ORMDesafio21Dias
{
    public sealed class Service
    {
        private string connectionString;
        private IType iType;

        public Service(IType _iType)
        {
            this.iType = _iType;
            this.connectionString = this.iType.ConnectionString;
        }

        public void Save()
        {
            throw new NotImplementedException();
        }

        public void Destroy()
        {
            throw new NotImplementedException();
        }

        public void Get()
        {
            throw new NotImplementedException();
        }

        public static T All<T>()
        {
            throw new NotImplementedException();
        }
    }
}
