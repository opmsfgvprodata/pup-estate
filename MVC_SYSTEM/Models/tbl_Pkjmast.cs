using MVC_SYSTEM.App_LocalResources; //add by wani 22.9.2020

namespace MVC_SYSTEM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_Pkjmast
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid fld_UniqueID { get; set; }

        [StringLength(20)]
        public string fld_Nopkj { get; set; }

        //[Required(ErrorMessageResourceType = typeof(GlobalResEstate), ErrorMessageResourceName = "msgModelValidation")] //add by wani 22.9.2020
        [StringLength(15)]
        public string fld_Nokp { get; set; }

        //[Required(ErrorMessageResourceType = typeof(GlobalResEstate), ErrorMessageResourceName = "msgModelValidation")] //add by wani 22.9.2020
        [StringLength(100)]
        public string fld_Nama { get; set; }

        [StringLength(100)]
        public string fld_Almt1 { get; set; }

        [StringLength(30)]
        public string fld_Daerah { get; set; }

        [StringLength(5)]
        public string fld_Neg { get; set; }

        [StringLength(5)]
        public string fld_Negara { get; set; }

        [StringLength(5)]
        public string fld_Poskod { get; set; }

        [StringLength(15)]
        public string fld_Notel { get; set; }

        [StringLength(15)]
        public string fld_Nofax { get; set; }

        [StringLength(1)]
        public string fld_Kdjnt { get; set; }

        [StringLength(2)]
        public string fld_Kdbgsa { get; set; }

        [StringLength(1)]
        public string fld_Kdagma { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Column(TypeName = "date")]
        public DateTime? fld_Trlhr { get; set; }

        [StringLength(2)]
        public string fld_Kdrkyt { get; set; }

        [StringLength(1)]
        public string fld_Kdkwn { get; set; }

        [StringLength(1)]
        public string fld_Kpenrka { get; set; }

        [StringLength(1)]
        public string fld_Kdaktf { get; set; }

        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        public DateTime? fld_Trtakf { get; set; }

        [StringLength(60)]
        public string fld_Sbtakf { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Column(TypeName = "date")]
        public DateTime? fld_Trmlkj { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Column(TypeName = "date")]
        public DateTime? fld_Trshjw { get; set; }

        [StringLength(10)]
        public string fld_Ktgpkj { get; set; }

        [StringLength(2)]
        public string fld_Jenispekerja { get; set; }

        [StringLength(3)]
        public string fld_Kodbkl { get; set; }

        [StringLength(5)]
        public string fld_KodSocso { get; set; }

        [StringLength(12)]
        public string fld_Noperkeso { get; set; }

        [StringLength(5)]
        public string fld_KodKWSP { get; set; }

        [StringLength(15)]
        public string fld_Nokwsp { get; set; }

        [StringLength(50)]
        public string fld_Kdbank { get; set; }

        [StringLength(50)]
        public string fld_NoAkaun { get; set; }

        [StringLength(15)]
        public string fld_Visano { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fld_T1visa { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Column(TypeName = "date")]
        public DateTime? fld_T2visa { get; set; }

        [StringLength(15)]
        public string fld_Nogilr { get; set; }

        [StringLength(20)]
        public string fld_Prmtno { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fld_T1prmt { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Column(TypeName = "date")]
        public DateTime? fld_T2prmt { get; set; }

        [StringLength(20)]
        public string fld_Psptno { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fld_T1pspt { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Column(TypeName = "date")]
        public DateTime? fld_T2pspt { get; set; }

        [StringLength(5)]
        public string fld_Kdldg { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public int? fld_DivisionID { get; set; }

        public DateTime? fld_DateApply { get; set; }

        [StringLength(50)]
        public string fld_AppliedBy { get; set; }

        public int? fld_StatusApproved { get; set; }

        [StringLength(50)]
        public string fld_ActionBy { get; set; }

        public DateTime? fld_ActionDate { get; set; }

        [StringLength(50)]
        public string fld_Batch { get; set; }

        [StringLength(15)]
        public string fld_IDpkj { get; set; }

        public int? fld_KumpulanID { get; set; }

        [StringLength(1)]
        public string fld_StatusKwspSocso { get; set; }

        [StringLength(1)]
        public string fld_StatusAkaun { get; set; }

        public string fld_Remarks { get; set; }

        [StringLength(50)]
        public string fld_KodSAPPekerja { get; set; }

        [StringLength(200)]
        public string fld_Almt2 { get; set; }

        [StringLength(5)]
        public string fld_Negara2 { get; set; }

        [StringLength(5)]
        public string fld_PurposeRequest { get; set; }

        //added by faeza 22.09.2021
        [StringLength(4)]
        [Display(Name = "Last 4 PAN")]
        public string fld_Last4Pan { get; set; }

        [StringLength(15)]
        public string fld_PaymentMode { get; set; }

        [StringLength(10)]
        public string fld_PassportStatus { get; set; }

        [StringLength(50)]
        public string fld_PassportRenewalStatus { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fld_PassportRenewalStartDate { get; set; }

        [StringLength(10)]
        public string fld_PermitStatus { get; set; }

        [StringLength(50)]
        public string fld_PermitRenewalStatus { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fld_PermitRenewalStartDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fld_ContractStartDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fld_ContractExpiryDate { get; set; }

        [StringLength(20)]
        public string fld_NopkjPermanent { get; set; }
    }

    [Table("tbl_Pkjmast")]
    public partial class tbl_PkjmastEdit
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid fld_UniqueID { get; set; }

        [StringLength(20)]
        public string fld_Nopkj { get; set; }

        [StringLength(15)]
        public string fld_Nokp { get; set; }

        [StringLength(40)]
        public string fld_Nama { get; set; }

        [StringLength(100)]
        public string fld_Almt1 { get; set; }

        [StringLength(30)]
        public string fld_Daerah { get; set; }

        [StringLength(5)]
        public string fld_Neg { get; set; }

        [StringLength(5)]
        public string fld_Negara { get; set; }

        [StringLength(5)]
        public string fld_Poskod { get; set; }

        [StringLength(15)]
        public string fld_Notel { get; set; }

        [StringLength(15)]
        public string fld_Nofax { get; set; }

        [StringLength(1)]
        public string fld_Kdjnt { get; set; }

        [StringLength(2)]
        public string fld_Kdbgsa { get; set; }

        [StringLength(1)]
        public string fld_Kdagma { get; set; }

        [DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Column(TypeName = "date")]
        public DateTime? fld_Trlhr { get; set; }

        [StringLength(2)]
        public string fld_Kdrkyt { get; set; }

        [StringLength(1)]
        public string fld_Kdkwn { get; set; }

        [StringLength(1)]
        public string fld_Kpenrka { get; set; }

        [StringLength(5)]
        public string fld_Kdaktf { get; set; }

        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        public DateTime? fld_Trtakf { get; set; }

        [StringLength(60)]
        public string fld_Sbtakf { get; set; }

        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        public DateTime? fld_Trmlkj { get; set; }

        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        public DateTime? fld_Trshjw { get; set; }

        [StringLength(2)]
        public string fld_Ktgpkj { get; set; }

        [StringLength(2)]
        public string fld_Jenispekerja { get; set; }

        [StringLength(3)]
        public string fld_Kodbkl { get; set; }

        [StringLength(5)]
        public string fld_KodSocso { get; set; }

        [StringLength(12)]
        public string fld_Noperkeso { get; set; }

        [StringLength(5)]
        public string fld_KodKWSP { get; set; }

        [StringLength(15)]
        public string fld_Nokwsp { get; set; }

        [StringLength(50)]
        [Required]
        public string fld_Kdbank { get; set; }

        [StringLength(50)]
        public string fld_NoAkaun { get; set; }

        [StringLength(15)]
        public string fld_Visano { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fld_T1visa { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fld_T2visa { get; set; }

        [StringLength(15)]
        public string fld_Nogilr { get; set; }

        [StringLength(20)]
        public string fld_Prmtno { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fld_T1prmt { get; set; }

        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        public DateTime? fld_T2prmt { get; set; }

        [StringLength(20)]
        public string fld_Psptno { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fld_T1pspt { get; set; }

        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        public DateTime? fld_T2pspt { get; set; }

        [StringLength(5)]
        public string fld_Kdldg { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public int? fld_DivisionID { get; set; }

        public DateTime? fld_DateApply { get; set; }

        [StringLength(50)]
        public string fld_AppliedBy { get; set; }

        public int? fld_StatusApproved { get; set; }

        [StringLength(50)]
        public string fld_ActionBy { get; set; }

        public DateTime? fld_ActionDate { get; set; }

        [StringLength(50)]
        public string fld_Batch { get; set; }

        [StringLength(15)]
        public string fld_IDpkj { get; set; }

        public int? fld_KumpulanID { get; set; }

        [StringLength(1)]
        public string fld_StatusKwspSocso { get; set; }

        [StringLength(1)]
        public string fld_StatusAkaun { get; set; }

        public string fld_Remarks { get; set; }

        [StringLength(200)]
        public string fld_Almt2 { get; set; }

        [StringLength(5)]
        public string fld_Negara2 { get; set; }

        //added by faeza 22.09.2021
        [StringLength(4)]
        [Display(Name = "Last 4 PAN")]
        public string fld_Last4Pan { get; set; }

        [StringLength(15)]
        public string fld_PaymentMode { get; set; }

    }

    [Table("tbl_Pkjmast")]
    public partial class tbl_PkjmastEditKwsp
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid fld_UniqueID { get; set; }

        [StringLength(20)]
        public string fld_Nopkj { get; set; }

        [StringLength(15)]
        public string fld_Nokp { get; set; }

        [StringLength(40)]
        public string fld_Nama { get; set; }

        [StringLength(100)]
        public string fld_Almt1 { get; set; }

        [StringLength(30)]
        public string fld_Daerah { get; set; }

        [StringLength(5)]
        public string fld_Neg { get; set; }

        [StringLength(5)]
        public string fld_Negara { get; set; }

        [StringLength(5)]
        public string fld_Poskod { get; set; }

        [StringLength(15)]
        public string fld_Notel { get; set; }

        [StringLength(15)]
        public string fld_Nofax { get; set; }

        [StringLength(1)]
        public string fld_Kdjnt { get; set; }

        [StringLength(2)]
        public string fld_Kdbgsa { get; set; }

        [StringLength(1)]
        public string fld_Kdagma { get; set; }

        [DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Column(TypeName = "date")]
        public DateTime? fld_Trlhr { get; set; }

        [StringLength(2)]
        public string fld_Kdrkyt { get; set; }

        [StringLength(1)]
        public string fld_Kdkwn { get; set; }

        [StringLength(1)]
        public string fld_Kpenrka { get; set; }

        [StringLength(5)]
        public string fld_Kdaktf { get; set; }

        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        public DateTime? fld_Trtakf { get; set; }

        [StringLength(60)]
        public string fld_Sbtakf { get; set; }

        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        public DateTime? fld_Trmlkj { get; set; }

        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        public DateTime? fld_Trshjw { get; set; }

        [StringLength(2)]
        public string fld_Ktgpkj { get; set; }

        [StringLength(2)]
        public string fld_Jenispekerja { get; set; }

        [StringLength(3)]
        public string fld_Kodbkl { get; set; }

        [StringLength(5)]
        public string fld_KodSocso { get; set; }

        [StringLength(12)]
        [Required]
        public string fld_Noperkeso { get; set; }

        [StringLength(5)]
        public string fld_KodKWSP { get; set; }

        [StringLength(15)]
        [Required]
        public string fld_Nokwsp { get; set; }

        [StringLength(50)]
        public string fld_Kdbank { get; set; }

        [StringLength(50)]
        //[Required]
        public string fld_NoAkaun { get; set; }

        [StringLength(15)]
        public string fld_Visano { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fld_T1visa { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fld_T2visa { get; set; }

        [StringLength(15)]
        public string fld_Nogilr { get; set; }

        [StringLength(20)]
        public string fld_Prmtno { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fld_T1prmt { get; set; }

        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        public DateTime? fld_T2prmt { get; set; }

        [StringLength(20)]
        public string fld_Psptno { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fld_T1pspt { get; set; }

        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        public DateTime? fld_T2pspt { get; set; }

        [StringLength(5)]
        public string fld_Kdldg { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public int? fld_DivisionID { get; set; }

        public DateTime? fld_DateApply { get; set; }

        [StringLength(50)]
        public string fld_AppliedBy { get; set; }

        public int? fld_StatusApproved { get; set; }

        [StringLength(50)]
        public string fld_ActionBy { get; set; }

        public DateTime? fld_ActionDate { get; set; }

        [StringLength(50)]
        public string fld_Batch { get; set; }

        [StringLength(15)]
        public string fld_IDpkj { get; set; }

        public int? fld_KumpulanID { get; set; }

        [StringLength(1)]
        public string fld_StatusKwspSocso { get; set; }

        [StringLength(1)]
        public string fld_StatusAkaun { get; set; }

        public string fld_Remarks { get; set; }

        [StringLength(200)]
        public string fld_Almt2 { get; set; }

        [StringLength(5)]
        public string fld_Negara2 { get; set; }

        //added by faeza 22.09.2021
        [StringLength(4)]
        [Display(Name = "Last 4 PAN")]
        public string fld_Last4Pan { get; set; }

        [StringLength(15)]
        public string fld_PaymentMode { get; set; }

    }
}
