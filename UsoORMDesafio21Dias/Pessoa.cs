using ORMDesafio21Dias;
using System;
using System.Collections.Generic;
using System.Text;

namespace UsoORMDesafio21Dias
{
    public class Pessoa : CType
    {
        [Table(IsNotOnDataBase = true)]
        public override string ConnectionString => @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=DESAFIO21DIAS;Data Source=AMP-NOTE\SQLEXPRESS";
        public string Endereco { get; set; }
        public string Nome { get; set; }
        public string Tipo { get; set; }
        public string CpfCnpj { get; set; }
    }
}
