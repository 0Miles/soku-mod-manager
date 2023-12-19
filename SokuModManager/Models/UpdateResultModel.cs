using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SokuModManager.Models
{
    public class UpdateResultModel
    {
        public string? Name { get; set; }
        public bool Success { get; set; }
        public string? Message { get; set; }
    }
}
