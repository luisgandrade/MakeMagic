using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MakeMagic.DTOs
{
    public class CharacterDTO : IValidatableObject
    {
        public string Name { get; set; }

        public string Role { get; set; }

        public string School { get; set; }

        public string House { get; set; }

        public string Patronus { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(Name))
                yield return new ValidationResult("É necessário informar o nome do personagem.");
            if (string.IsNullOrWhiteSpace(Role))
                yield return new ValidationResult("É necessário informar a função do personagem.");
            if (string.IsNullOrWhiteSpace(School))
                yield return new ValidationResult("É necessário informar a escola do personagem.");
            if (string.IsNullOrWhiteSpace(House))
                yield return new ValidationResult("É necessário informar a casa do personagem.");
            if (string.IsNullOrWhiteSpace(Patronus))
                yield return new ValidationResult("É necessário informar o patrono do personagem.");
        }
    }
}
