using MVC_SYSTEM.App_LocalResources;

namespace MVC_SYSTEM.MasterModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_SubCarumanTambahan
    {
        [Key]
        public int fld_JenisSubCarumanID { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResEstate), ErrorMessageResourceName = "msgModelValidation")]
        [StringLength(10)]
        public string fld_KodCaruman { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResEstate), ErrorMessageResourceName = "msgModelValidation")]
        [StringLength(10)]
        public string fld_KodSubCaruman { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResEstate), ErrorMessageResourceName = "msgModelValidation")]
        [StringLength(30)]
        public string fld_KeteranganSubCaruman { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResEstate), ErrorMessageResourceName = "msgModelValidation")]
        [Range(0, 99, ErrorMessageResourceType = typeof(GlobalResEstate), ErrorMessageResourceName = "msgMaxAgeModelValidation")]
        [RegularExpression("^[0-9][0-9]*$", ErrorMessageResourceType = typeof(GlobalResEstate), ErrorMessageResourceName = "msgNumberModelValidation")]
        public int fld_UmurLower { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResEstate), ErrorMessageResourceName = "msgModelValidation")]
        [Range(0, 99, ErrorMessageResourceType = typeof(GlobalResEstate), ErrorMessageResourceName = "msgMaxAgeModelValidation")]
        [RegularExpression("^[0-9][0-9]*$", ErrorMessageResourceType = typeof(GlobalResEstate), ErrorMessageResourceName = "msgNumberModelValidation")]
        public int fld_UmurUpper { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResEstate), ErrorMessageResourceName = "msgModelValidation")]
        [Range(0, 100.00, ErrorMessageResourceType = typeof(GlobalResEstate), ErrorMessageResourceName = "msgMaxPercentageModelValidation")]
        [RegularExpression("^\\d+(?:\\.\\d{1,2})?$", ErrorMessageResourceType = typeof(GlobalResEstate), ErrorMessageResourceName = "msgNumberModelValidation")]
        public decimal fld_KadarMajikan { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResEstate), ErrorMessageResourceName = "msgModelValidation")]
        [Range(0, 100.00, ErrorMessageResourceType = typeof(GlobalResEstate), ErrorMessageResourceName = "msgMaxPercentageModelValidation")]
        [RegularExpression("^\\d+(?:\\.\\d{1,2})?$", ErrorMessageResourceType = typeof(GlobalResEstate), ErrorMessageResourceName = "msgNumberModelValidation")]
        public decimal fld_KadarPekerja { get; set; }

        public int fld_NegaraID { get; set; }

        public int fld_SyarikatID { get; set; }

        public bool fld_Deleted { get; set; }
    }
}
