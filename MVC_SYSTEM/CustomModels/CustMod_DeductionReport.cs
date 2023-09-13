using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.CustomModels
{
    public partial class CustMod_DeductionWorkerDetailReport
    {
        [Key]
        public int ID { get; set; }

        public string WorkerID { get; set; }

        public string WorkerName { get; set; }

        public decimal TotalDeductionAmount { get; set; }

        public List<CustMod_DeductionDetails> DeductionDetail { get; set; }
    }

    public partial class CustMod_DeductionDetails
    {
        [Key]
        public int ID { get; set; }

        public string DeductionCode { get; set; }

        public string DeductionDesc { get; set; }

        public decimal TotalAmount { get; set; }
    }
}