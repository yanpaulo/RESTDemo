using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RESTDemo
{
    public class User
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Website { get; set; }

        public override string ToString() => $"Eu sou um Objeto, e meu nome é {Name}.";
    }

    class Program
    {
        static void Main(string[] args)
        {
            //Chama a versão assíncrona do método Main() e aguarda.
            MainAsync().Wait();
        }

        static async Task MainAsync()
        {
            //Declara uma nova array do tipo User[] e inicializa com 3 objetos.
            //Essa técnica para inicialização rápida de listas e objetos se chama de "INICIALIZADOR".
            var lista = new[]
            {
                new User { Name="Yan", Username="yanpaulo", Email="yanpaulo@hotmail.com", Website="http://yanscorp.com/youtube"  },
                new User { Name="Flávia", Username="flavia", Email="flavia@site.com" },
                new User { Name="Kaiba", Username="kaiba", Email="kaiba@kaibacorp.com", Website="http://kaibacorp.com"  }
            };

            //Converte a lista para json, que será uma string armazenada com a referência "json", do tipo string.
            //Em seguida, imprime a saída.

            //Para saída formatada/indentada, use assim:
            string json = JsonConvert.SerializeObject(lista, Formatting.Indented);

            //Para saída sem indentação/formatação, use assim:
            //var json = JsonConvert.SerializeObject(lista);

            Console.WriteLine("1) Lista convertida para json: ");
            Console.WriteLine(json);
            Console.WriteLine();

            //Converte o json de volta para uma array de User.
            //Em seguida, "imprime" os objetos.

            Console.WriteLine("2) Json convertido para lista: ");
            var listaConvetida = JsonConvert.DeserializeObject<User[]>(json);
            //Para cada item na lista
            foreach (var item in listaConvetida)
            {
                //Imprime o item.
                Console.WriteLine(item);
            }
            Console.WriteLine();

            //Consulta o servidor para obter todos os Users, e imprime somente os primeiros 5.

            Console.WriteLine("3) Lista obtida do Servidor: ");
            var listaObtida = await GetPessoasFromREST();
            //Take(5) retorna apenas os 5 primeiros objetos.
            foreach (var item in listaObtida.Take(5))
            {
                Console.WriteLine(item);
            }
            Console.WriteLine();

            //Envia para o Servidor a array de User criada no passo 1.

            Console.WriteLine("4) Enviando lista para o Servidor: ");
            PostPessoasToREST(lista);

            Console.ReadKey();
        }

        static async void PostPessoasToREST(User[] users)
        {
            //Cria um HttpClient com o endereço base especificado
            HttpClient client = new HttpClient() { BaseAddress = new Uri("http://jsonplaceholder.typicode.com/") };

            foreach (var item in users)
            {
                var json = JsonConvert.SerializeObject(item);
                //A string "users" é concatenada ao endereço base.
                //Assim, o POST irá para o endereço "http://jsonplaceholder.typicode.com/users"
                var result = await client.PostAsync("users", new StringContent(json));

                if (result.IsSuccessStatusCode)
                {
                    //Se o POST foi feito com sucesso, imprime uma mensagem de sucesso.
                    Console.WriteLine($"User {item.Name} enviado para o Servidor.");
                }
                else
                {
                    //Caso contrário, imprime uma mensagem de erro.
                    Console.WriteLine($"Erro ao enviar {item.Name} para o Servidor.");
                }
            }
        }

        static async Task<User[]> GetPessoasFromREST()
        {
            //Cria um HttpClient com o endereço base especificado
            var client = new HttpClient { BaseAddress = new Uri("http://jsonplaceholder.typicode.com") };

            //A string "users" é concatenada ao endereço base.
            //Assim, o GET irá para o endereço "http://jsonplaceholder.typicode.com/users"
            var resultado = await client.GetAsync("users");

            if (resultado.IsSuccessStatusCode)
            {
                //Se a requisição retornou status de sucesso
                //Lê a string de resposta
                var stringContent = await resultado.Content.ReadAsStringAsync();

                //Converte a string (que contem json) para uma lista de objetos
                var lista = JsonConvert.DeserializeObject<User[]>(stringContent);
                //Retorna a lista
                return lista;
            }
            else
            {
                //Se o status não foi de sucesso, retorna null.
                return null;
            }
        }
    }
}
