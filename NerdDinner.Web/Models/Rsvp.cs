using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace NerdDinner.Web.Models
{
    public class Rsvp
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long RsvpId { get; set; }

        [Required]
        public long DinnerId { get; set; }

        [Required]
        [MaxLength(64)]
        public string UserName { get; set; }

        public Dinner Dinner { get; set; }
    }
}
