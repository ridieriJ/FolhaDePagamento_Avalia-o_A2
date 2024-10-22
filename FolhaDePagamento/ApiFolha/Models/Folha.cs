using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiFolha.Models
{
public class Folha
{
    [Key]
    public int Id { get; set; }

    public decimal Valor { get; set; }

    public int Quantidade { get; set; }

    public int Mes { get; set; }

    public int Ano { get; set; }

    public int FuncionarioId { get; set; }

    [ForeignKey("FuncionarioId")]
    public Funcionario Funcionario { get; set; }

    public Folha(decimal valor, int quantidade, int mes, int ano, int funcionarioId)
    {
        Valor = valor;
        Quantidade = quantidade;
        Mes = mes;
        Ano = ano;
        FuncionarioId = funcionarioId;
    }
}
}