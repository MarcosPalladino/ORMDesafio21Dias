using ORMDesafio21Dias;
using System;
using System.Collections.Generic;
using System.Text;

namespace UsoORMDesafio21Dias
{
    [Table(Name = "PessoasMarcos") ]
    public class Marco : CType
    {
        [Table(IsNotOnDataBase = true)]
        public override string ConnectionString => @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=DESAFIO21DIAS;Data Source=AMP-NOTE\SQLEXPRESS";
        
        [Table(PrimaryKey = "Codigo")]
        public override int Id { get => base.Id; set => base.Id = value; }

        public string Endereco { get; set; }
        public string Nome { get; set; }
        public string Fone { get; set; }
        public int Cep { get; set; }
        public string Email { get; set; }

    }
}
