using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ParadiesHome.Models
{
    public class LikeCount
    {
        public LikeCount()
        {
            Count = 1;
        }
        [Key]
        public int LikesCountId { get; set; }
        public string UserId { get; set; }
        [NotMapped]
        [ForeignKey("UserId")]
        public virtual IdentityUser IdentityUser { get; set; }
        public int PostId { get; set; }
        [ForeignKey("PostId")]
        public virtual Post Post { get; set; }
        public int Count { get; set; }
    }
}
