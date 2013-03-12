using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Orchard.Data.Conventions;


namespace Coevery.Leads.Models
{
    /// <summary>
    /// Todo list entity
    /// </summary>
    public class TodoListRecord
    {
        [Key]
        public virtual int Id { get; set; }

        [Required]
        public virtual string UserId { get; set; }

        [Required]
        public virtual string Title { get; set; }
        [Aggregate]
        [CascadeAllDeleteOrphan]
        public virtual IList<TodoItemRecord> Todos { get; set; }
    }
}