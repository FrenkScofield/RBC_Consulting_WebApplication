using System.ComponentModel.DataAnnotations.Schema;

namespace RBC_Consulting_WebApplication.Models
{
    public class CustomerInfo
    {
        public int Id { get; set; }

        public string FullName { get; set; }

        public string Description { get; set; }

        public DateTime DateTime { get; set; }

        public string FileName { get; set; }

        public string ContentType { get; set; }

        public byte[] Data { get; set; }

        [NotMapped]
        public IFormFile FormFile { get; set; }
    }
}
