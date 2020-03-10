using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RenosFriendsList.API.Entities
{
    public class Owner
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [MaxLength(1500)]
        public string Description { get; set; }

        public virtual ICollection<Dog> Dogs { get; set; } = new List<Dog>();
    }
}
