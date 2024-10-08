namespace MVC_SYSTEM.ViewingModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class vw_KumpulanKerja
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int fld_KumpulanID { get; set; }

        [StringLength(50)]
        public string fld_KodKumpulan { get; set; }

        public int? bilangan_ahli { get; set; }

        [StringLength(50)]
        public string fld_KodKerja { get; set; }

        [StringLength(50)]
        public string fld_Keterangan { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public int? fld_DivisionID { get; set; }

        public bool? fld_deleted { get; set; }
        [StringLength(20)]
        public string fld_SupervisorID { get; set; }
        [StringLength(150)]
        public string fld_SupervisorName { get; set; }
    }
}
