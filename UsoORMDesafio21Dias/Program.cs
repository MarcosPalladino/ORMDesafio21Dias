using ORMDesafio21Dias;
using System;
using System.Collections.Generic;

namespace UsoORMDesafio21Dias
{
    class Program
    {
        static void Main(string[] args)
        {
            var pessoas2 = Service.All<List<Pessoa>>();
            //var pessoas2 = Service

            var pessoas = new Service(new Pessoa()).All();
            foreach(Pessoa pessoa in pessoas)
            {
                Console.WriteLine(pessoa.Nome);
            }

            Console.ReadLine();

            /*
            var pessoa = new Pessoa();

            pessoa.Nome = "Marcos";
            pessoa.Endereco = "Endereco Marcos";
            pessoa.CpfCnpj = "990.009.000-10";
            pessoa.Tipo = "F";
            pessoa.Save();

            pessoa.Id = 4;
            pessoa.Get();

            pessoa.Id = 7;
            pessoa.Destroy();

            pessoa.Id = 8;
            pessoa.Destroy();

            pessoa.Id = 9;
            pessoa.Destroy();

            pessoa.Id = 10;
            pessoa.Destroy();

            pessoa.Id = 11;
            pessoa.Destroy();

            pessoa.Id = 12;
            pessoa.Destroy();

            pessoa.Id = 13;
            pessoa.Destroy();

            pessoa.Id = 14;
            pessoa.Destroy();
            */
            //new Service(pessoa).Save();
        }
    }
}
