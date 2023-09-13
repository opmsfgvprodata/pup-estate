namespace MVC_SYSTEM.MasterModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tblEmailList")]
    public partial class tblEmailList
    {
        [Key]
        public int fldID { get; set; }

        [StringLength(50)]
        public string fldEmail { get; set; }

        [StringLength(100)]
        public string fldName { get; set; }

        [StringLength(5)]
        public string fldCategory { get; set; }

        [StringLength(30)]
        public string fldDepartment { get; set; }

        public bool? fldDeleted { get; set; }

        public int? fldNegaraID { get; set; }

        public int? fldSyarikatID { get; set; }

        public int? fldWilayahID { get; set; }
    }
}
