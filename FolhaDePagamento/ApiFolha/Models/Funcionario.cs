using System.ComponentModel.DataAnnotations;

namespace ApiFolha.Models
{
    public class Funcionario
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Nome { get; set; }

        [Required]
        [StringLength(11)] 
        public string Cpf { get; set; }

        public Funcionario(string nome, string cpf)
        {
            Nome = nome;
            Cpf = cpf;
        }
    }
}
