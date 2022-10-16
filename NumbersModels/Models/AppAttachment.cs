using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Entity.Models
{
   public class AppAttachment
    {
        public int Id { get; set; }
        public string Source { get; set; }
        public int SourceId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string FileType { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public Boolean IsActive { get; set; }
        public Boolean IsDeleted { get; set; }

    }
}
