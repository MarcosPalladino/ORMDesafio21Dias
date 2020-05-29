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

            var pessoaNova = new Pessoa() { Id = 4 };
            pessoaNova.Get();

            Console.WriteLine(pessoaNova.Nome);
            //new Service(pessoa).Save();
        }
    }
}
