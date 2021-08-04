using LuxAsp.Notations;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LuxAsp.Internals
{
    internal class Settings : DataModel
    {
        [MultiPrimaryKey(0)][Required(AllowEmptyStrings = false)][MaxLength(255)]
        public string Type { get; set; }

        [MultiPrimaryKey(1)][Required(AllowEmptyStrings = true)][MaxLength(255)]
        public string Key { get; set; } = "";

        [UniversalDateTime]
        public DateTime LastWriteTime { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Json { get; set; }

        [NotMapped]
        public SettingsModel Instance { get; set; }
    }
}
