using System.ComponentModel.DataAnnotations;

namespace FinPulse.Validations;

public class DataNaoFuturaAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null)
            return ValidationResult.Success; // Se for nulo, deixa o [Required] cuidar

        if (value is DateTime data)
        {
            // Não permite datas maiores que o dia de hoje (permitindo uma margem de segurança de minutos)
            if (data.Date > DateTime.Today)
            {
                return new ValidationResult(ErrorMessage ?? "A data da transação não pode ser uma data futura.");
            }
        }

        return ValidationResult.Success;
    }
}