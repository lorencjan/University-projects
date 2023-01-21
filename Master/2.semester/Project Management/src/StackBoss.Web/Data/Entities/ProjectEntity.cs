using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StackBoss.Web.Data.Entities
{
    public class ProjectEntity
    {
        [Key]
        public int Id { get; set; }
        public string CustomId { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public string Manager { get; set; }
        public string Staff { get; set; }
        public List<RiskEntity> RiskList { get; set; }
    }
}
