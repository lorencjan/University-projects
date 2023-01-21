using StackBoss.Web.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StackBoss.Web.Data.Entities
{
    public class RiskEntity
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]
        public Category Category { get; set; }
        public string Threat { get; set; }
        public string Starters { get; set; }
        public string Reaction { get; set; }
        public string Owner { get; set; }
        public Probability Probability { get; set; }
        public Consequences Consequences { get; set; }
        public int RiskEvaluation { get; set; }
        public State State { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedStateDate { get; set; }
        public DateTime ReactionDate { get; set; }  
        [Required]
        public int ProjectId {get; set; }
        public string CustomId {get; set; }
        public ProjectEntity Project { get; set;}
    }
}
