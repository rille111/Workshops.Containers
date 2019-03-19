using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;

namespace frontend.values.web.Models
{
    public class TraceModel
    {
        [Required]
        public string Message { get; set; }
        public LogLevel Level { get; set; }
    }
}
