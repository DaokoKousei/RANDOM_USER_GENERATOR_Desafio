namespace RANDOM_USER_GENERATOR.Models
{
    public class Result
    {
        public Name Name { get; set; }
        public Location Location { get; set; }

        public Registered Registered { get; set; }

        public string Gender { get; set; }

        public string Email { get; set; }


        public string Phone { get; set; }

        public Picture Picture { get; set; }
        // Adicione outras propriedades conforme necessário para representar os dados do usuário
    }
}
