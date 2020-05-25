using System;
using System.Collections.Generic;
using System.Text;

namespace ORMDesafio21Dias
{
    public class IType : IConnectionString
    {
        [Table(PrimaryKey ="id")]
        public virtual int Id { get; set; }
    }
}
