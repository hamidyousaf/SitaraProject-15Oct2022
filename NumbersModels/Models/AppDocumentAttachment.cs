using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Numbers.Entity.Models
{
    public class AppDocumentAttachment
    {
        public int Id { get; set; }
        [MaxLength(50)]
        public string Module { get; set; }
        [MaxLength(50)]
        public string TransactionType { get; set; }
        public int TransactionId { get; set; }
        [MaxLength(100)]
        public string Tags { get; set; }
        [MaxLength(300)]
        public string FileName { get; set; }
        [MaxLength(400)]
        public string UniqueFileName { get; set; }
        [MaxLength(100)]
        public string FileType { get; set; }
        [MaxLength(300)]
        public string FilePath { get; set; }
        [MaxLength(20)]
        public string FileExtension { get; set; }
        public int FileSize { get; set; }
        public bool IsDeleted { get; set; }
        [MaxLength(450)]
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        [MaxLength(450)]
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
