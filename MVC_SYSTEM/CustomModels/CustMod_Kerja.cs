using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.CustomModels
{
    public partial class CustMod_Kerja
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid fld_ID { get; set; }

        [StringLength(20)]
        public string fld_Nopkj { get; set; }
        public DateTime? fld_Tarikh { get; set; }

        [StringLength(4)]
        public string fld_KodAktvt { get; set; }

        [StringLength(10)]
        public string fld_Unit { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_KadarByr { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_JumlahHasil { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_Amount { get; set; }

        public int? fld_CreatedBy { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }
        public int? fld_DivisionID { get; set; }
    }
}