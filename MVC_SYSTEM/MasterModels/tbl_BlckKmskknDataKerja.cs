namespace MVC_SYSTEM.MasterModels
{
    using MVC_SYSTEM.App_LocalResources;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_BlckKmskknDataKerja
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid fld_ID { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fld_ValidDT { get; set; }

        
        public bool? fld_BlokStatus { get; set; }

        public int? fld_Month { get; set; }

        public int? fld_Year { get; set; }

        [StringLength(300)]
        public string fld_Reason { get; set; }

        public short? fld_BilHariXKyIn { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public int? fld_UnBlockAppBy { get; set; }

        public DateTime? fld_UnBlockAppDT { get; set; }

        [StringLength(300)]
        public string fld_Remark { get; set; }
        public int? fld_DivisionID { get; set; }  //add by wani 18.6.2021

        [StringLength(50)]
        public string fld_Purpose { get; set; }

    }

    [Table("tbl_BlckKmskknDataKerja")]
    public partial class tbl_BlckKmskknDataKerjaUpdate
    {
        
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid fld_ID { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fld_ValidDT { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResEstate), ErrorMessageResourceName = "msgModelValidation")]
        public bool? fld_BlokStatus { get; set; }

        public int? fld_Month { get; set; }

        public int? fld_Year { get; set; }

        [StringLength(300)]
        public string fld_Reason { get; set; }

        public short? fld_BilHariXKyIn { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public int? fld_UnBlockAppBy { get; set; }

        public DateTime? fld_UnBlockAppDT { get; set; }

        [StringLength(300)]
        public string fld_Remark { get; set; }
    }
}
