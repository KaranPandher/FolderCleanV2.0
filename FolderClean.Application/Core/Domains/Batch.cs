using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FolderClean.Application.Core.Domains
{
    //https://docs.microsoft.com/en-us/ef/core/modeling/entity-types?tabs=data-annotations
    // [Table("Batches")]
    public class Batch
    {
        [Key]
        public double Batch_ID { get; set; }
        public string BatchName { get; set; }
        public string AlarisToSBStatsStatus { get; set; }
        public string JobName { get; set; }
    }

    public class RenalDocumentStat
    {
        public int Year { get; set; }
        public string DocumentType { get; set; }
        public string BatchReference { get; set; }
        public string PrepperBarcode { get; set; }
        [Key]
        public int DocumentID { get; set; }
        public int Document_ID { get; set; }
    }

    public class RenalBatchStat
    {
        public string Unit { get; set; }
        [Key]
        public string ProcessingBatchName { get; set; }
    }
}