﻿namespace MVC_SYSTEM.ViewingModels
{
    //using AuthModels;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity;
    using System.Linq;
    using System.Web;

    [Table("tbl_TaxWorkerInfo")]
    public partial class tbl_TaxWorkerInfo
    {
        [Key]
        public Guid fld_UniqueID { get; set; }
       
        [StringLength(20)]
        public string fld_NopkjPermanent { get; set; }

        [StringLength(20)]
        public string fld_Nopkj { get; set; }

        [StringLength(30)]
        public string fld_TaxNo { get; set; }

        public int? fld_Year { get; set; }

        [StringLength(5)]
        public string fld_TaxResidency { get; set; }

        [StringLength(5)]
        public string fld_TaxMaritalStatus { get; set; }

        [StringLength(5)]
        public string fld_IsIndividuOKU { get; set; }

        [StringLength(5)]
        public string fld_IsSpouseOKU { get; set; }

        public int? fld_ChildBelow18Full { get; set; }

        public int? fld_ChildBelow18Half { get; set; }

        public int? fld_ChildAbove18CertFull { get; set; }

        public int? fld_ChildAbove18CertHalf { get; set; }

        public int? fld_ChildAbove18HigherFull { get; set; }

        public int? fld_ChildAbove18HigherHalf { get; set; }
        public int? fld_DisabledChildFull { get; set; }

        public int? fld_DisabledChildHalf { get; set; }

        public int? fld_DisabledChildStudyFull { get; set; }

        public int? fld_DisabledChildStudyHalf { get; set; }

        public DateTime? fld_CreatedDate { get; set; }

        public int? fld_CreatedBy { get; set; }

        public DateTime? fld_ModifiedDate { get; set; }

        public int? fld_ModifiedBy { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public int? fld_DivisionID { get; set; }

    }
}