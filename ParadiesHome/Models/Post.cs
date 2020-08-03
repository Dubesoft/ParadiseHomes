using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ParadiesHome.Models
{
    public class Post
    {
        [Key]
        public int PostId { get; set; }
        [Display(Name = "PostedBy")]
        public string UserId { get; set; }
        public string PosterEmail { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public byte[] Picture { get; set; }
        public byte[] Video { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
