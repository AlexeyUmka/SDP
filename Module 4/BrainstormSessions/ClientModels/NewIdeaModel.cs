using System.ComponentModel.DataAnnotations;
using BrainstormSessions.Helpers;

namespace BrainstormSessions.ClientModels
{
    public class NewIdeaModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Range(1, 1000000)]
        public int SessionId { get; set; }

        public override string ToString()
        {
            return this.GetPropertiesAndFieldsStringRepresentation();
        }
    }
}
