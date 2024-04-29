using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Npgsql;
using RANDOM_USER_GENERATOR.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace RANDOM_USER_GENERATOR.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        



        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            RandomUserResponse userData = await pegarAPI();



            string nome = userData.Results[0].Name.First.ToString();
            string genero = userData.Results[0].Gender.ToString();
            string dataNascimento = userData.Results[0].Registered.Date.ToString();
            string estado = userData.Results[0].Location.State.ToString();
            string pais = userData.Results[0].Location.Country.ToString();
            string cidade = userData.Results[0].Location.City.ToString();
            int idade =  Convert.ToInt16(userData.Results[0].Registered.Age.ToString());
            string email = userData.Results[0].Email.ToString();
            string telefone = userData.Results[0].Phone.ToString();
            string foto = userData.Results[0].Picture.Thumbnail.ToString();
       
            gravarTabela(nome, genero,dataNascimento, estado, pais,cidade,idade,email,telefone, foto);
            return View(userData);
        }




        public async Task<RandomUserResponse> pegarAPI()
        {
            HttpClient client = new HttpClient
            {
                BaseAddress = new Uri("https://api.randomuser.me")
            };

            HttpResponseMessage response = await client.GetAsync("https://randomuser.me/api/");
            response.EnsureSuccessStatusCode();

            string stringResult = await response.Content.ReadAsStringAsync();
            var root = JsonConvert.DeserializeObject<RandomUserResponse>(stringResult);

           
        

            return root;
        }

   

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Lista()
        {
            pegarUsuarios();
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        private void gravarTabela(String nome, String genero, String dataNascimento, String estado, String pais, String cidade, int idade, String email, String telefone, String foto)
        {

            string connString = "Server=localhost;Port=5432;Database=postgres;User Id=postgres;Password=root;";

            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();

                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        
                        using (var xp = new NpgsqlCommand())
                        {
                            xp.Connection = conn;
                            xp.Transaction = transaction;

                            xp.CommandText = "INSERT INTO usuarios (nome,genero,data_nascimento,estado,pais,idade,cidade,email,telefone,foto) VALUES (@nome, @genero,@data,@estado,@pais,@idade,@cidade,@email,@telefone,@foto)";
                            xp.Parameters.AddWithValue("nome", nome);
                            xp.Parameters.AddWithValue("genero", genero);
                            xp.Parameters.AddWithValue("data", dataNascimento);
                            xp.Parameters.AddWithValue("estado", estado);
                            xp.Parameters.AddWithValue("pais", pais);
                            xp.Parameters.AddWithValue("idade", idade);
                            xp.Parameters.AddWithValue("cidade", cidade);
                            xp.Parameters.AddWithValue("email", email);
                            xp.Parameters.AddWithValue("telefone", telefone);
                            xp.Parameters.AddWithValue("foto", foto);
                            xp.ExecuteNonQuery();

                           
                        }

                        
                        transaction.Commit();
                       
                    }
                    catch (Exception ex)
                    {
                       
                        transaction.Rollback();

                        string erro = ex.Message;
                    }
                }
            }
        }










        [HttpGet]
        public IActionResult pegarUsuarios()
        {
            string connString = "Server=localhost;Port=5432;Database=postgres;User Id=postgres;Password=root;";

           
            List<Usuario> usuarios = new List<Usuario>();

            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand("SELECT * FROM usuarios", conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string _Data_nascimento = reader.GetString(reader.GetOrdinal("data_nascimento"));
                            Usuario usuario = new Usuario
                            { //nome,genero,data_nascimento,estado,pais,idade,cidade,email,telefone,foto
                                Nome = reader.GetString(reader.GetOrdinal("nome")),
                                Foto = reader.GetString(reader.GetOrdinal("foto")),
                                Idade =  reader.GetInt16(reader.GetOrdinal("idade")),
                                Genero = reader.GetString(reader.GetOrdinal("genero")),
                                Data_nascimento = reader.GetString(reader.GetOrdinal("data_nascimento")).Substring(0, Math.Min(10, _Data_nascimento.Length)),
                                Estado = reader.GetString(reader.GetOrdinal("estado")),
                                Pais = reader.GetString(reader.GetOrdinal("pais")),
                                Cidade = reader.GetString(reader.GetOrdinal("cidade")),
                                Email = reader.GetString(reader.GetOrdinal("email")),
                                Telefone = reader.GetString(reader.GetOrdinal("telefone"))

                            };
                            usuarios.Add(usuario);
                        }
                    }
                }
            }

            return View(usuarios); 
        }



    }
}
