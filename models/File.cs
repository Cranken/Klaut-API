
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FileAPI.Models
{
    public class File
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public string Id { get; set; }

        public string FileType { get; set; } = "";
        public bool ThumbnailAvailable { get; set; } = false;
        public string OriginalFileName { get; set; } = "";
    }
}