using System;
using System.Collections.Generic;
using System.Text;

namespace Smart.Data.Abstractions.Models
{
    public class EntitiesNoteInputModel
    {
        public Guid? RefId { get; set; }

        public string Date { get; set; }

        public string Note { get; set; }

        public Guid? UserId { get; set; }
    }
}
