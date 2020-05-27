using ORMDesafio21Dias;
using System;

namespace UsoORMDesafio21Dias
{
    class Program
    {
        static void Main(string[] args)
        {
            var pessoa = new Pessoa();

            pessoa.Nome = "Marcos";
            pessoa.Endereco = "Endereco Marcos";

            pessoa.Save();

            //new Service(pessoa).Save();
            
            

        }
    }
}
