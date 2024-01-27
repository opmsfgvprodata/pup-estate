using iTextSharp.text.pdf;
using iTextSharp.text;
using MVC_SYSTEM.App_LocalResources;
using MVC_SYSTEM.Attributes;
using MVC_SYSTEM.Class;
using MVC_SYSTEM.log;
using MVC_SYSTEM.MasterModels;
using MVC_SYSTEM.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ServiceModel.Configuration;

namespace MVC_SYSTEM.Controllers
{
    [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Super Power User,Super User,Normal User")]
    public class PCBReportController : Controller
    {
        private MVC_SYSTEM_MasterModels db = new MVC_SYSTEM_MasterModels();
        private MVC_SYSTEM_Models dbp = new MVC_SYSTEM_Models();
        GetIdentity GetIdentity = new GetIdentity();
        GetNSWL GetNSWL = new GetNSWL();
        Connection Connection = new Connection();
        ChangeTimeZone timezone = new ChangeTimeZone();
        GetConfig GetConfig = new GetConfig();
        errorlog geterror = new errorlog();
        GetDivision getdivision = new GetDivision();
        private GetIdentity getidentity = new GetIdentity();
        // GET: PCBReport
        public ActionResult Index()
        {
            ViewBag.Report = "class = active";
            int month = timezone.gettimezone().AddMonths(-1).Month;
            int year = timezone.gettimezone().Year;
            int rangeyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;

            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? DivisionID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            DivisionID = GetNSWL.GetDivisionSelection(getuserid, NegaraID, SyarikatID, WilayahID, LadangID);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            var yearlist = new List<SelectListItem>();
            for (var i = rangeyear; i <= year; i++)
            {
                if (i == timezone.gettimezone().Year)
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
                }
                else
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                }
            }
            var monthList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID), "fldOptConfValue", "fldOptConfDesc", month);

            List<SelectListItem> StatusList = new List<SelectListItem>();
            StatusList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "statusaktif" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            StatusList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            List<SelectListItem> SelectionList = new List<SelectListItem>();
            //code asal
            //SelectionList = new SelectList(dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1" && x.fld_DivisionID == DivisionID).OrderBy(o => o.fld_Nopkj).Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + "-" + s.fld_Nama }), "Value", "Text").ToList();
            //edit by fitri 24-11-2020
            SelectionList = new SelectList(dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1" && x.fld_DivisionID == DivisionID).OrderBy(o => o.fld_Nama).Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + "-" + s.fld_Nama }), "Value", "Text").ToList();
            SelectionList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            List<SelectListItem> JnsPkjList = new List<SelectListItem>();
            JnsPkjList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "jnsPkj" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            JnsPkjList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            ViewBag.SelectionList = SelectionList;
            ViewBag.MonthList = monthList;
            ViewBag.YearList = yearlist;
            ViewBag.StatusList = StatusList;
            ViewBag.JnsPkjList = JnsPkjList;
            return View();
        }

        public FileStreamResult PCBPdf()
        {
            Document pdfDoc = new Document(PageSize.A4, 25, 25, 25, 25);
            MemoryStream ms = new MemoryStream();
            MemoryStream output = new MemoryStream();
            PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, ms);
            pdfWriter.PageEvent = new PDFFooter();
            Chunk chunk = new Chunk();
            Paragraph para = new Paragraph();
            pdfDoc.Open();

            pdfDoc.NewPage();

            Header(pdfDoc);

            WorkerInfo(pdfDoc, "");

            WorkerChildInfo(pdfDoc, "");

            pdfDoc.NewPage();

            WorkerRemuneration1(pdfDoc, "");

            WorkerRemuneration2(pdfDoc, "");

            WorkerRemuneration3(pdfDoc, "");

            pdfDoc.NewPage();

            WorkerDeduction(pdfDoc, "");

            pdfDoc.NewPage();

            WorkerRebate(pdfDoc, "");

            WorkerPCBCalculation(pdfDoc, "");

            WorkerPCBResultCalculation(pdfDoc, "");

            pdfWriter.CloseStream = false;
            pdfDoc.Close();
            byte[] file = ms.ToArray();
            output.Write(file, 0, file.Length);
            output.Position = 0;
            return new FileStreamResult(output, "application/pdf");
        }

        public Document Header(Document pdfDoc)
        {
            PdfPTable table = new PdfPTable(2);
            float[] widths = new float[] { 0.5f, 1 };
            table.SetWidths(widths);
            table.WidthPercentage = 100;
            Image image = Image.GetInstance(Server.MapPath("~/Asset/Images/LHDN_Logo.png"));
            PdfPCell cell = new PdfPCell(image);
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.Border = 0;
            cell.Rowspan = 2;
            image.ScaleAbsolute(80, 50);
            table.AddCell(cell);

            Chunk chunk = new Chunk("KALKULATOR PCB", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_BOTTOM;
            cell.Border = 0;
            table.AddCell(cell);

            chunk = new Chunk("LEMBAGA HASIL DALAM NEGERI MALAYSIA", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_TOP;
            cell.Border = 0;
            table.AddCell(cell);
            pdfDoc.Add(table);
            return pdfDoc;
        }

        public Document WorkerInfo(Document pdfDoc, string NoPkj)
        {
            PdfPTable table = new PdfPTable(6);
            float[] widths = new float[] { 1, 0.1f, 0.8f, 0.5f, 0.1f, 0.8f };
            table.SetWidths(widths);
            table.WidthPercentage = 100;
            table.SpacingBefore = 20f;

            Chunk chunk = new Chunk("NAMA", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK));
            PdfPCell cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, 0, 1, 15, 10);

            chunk = new Chunk(":", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, 0, 1, 15, 1);

            chunk = new Chunk("JOJO ATI MAADI", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, 0, 4, 15, 1);

            chunk = new Chunk("NOMBOR PENGENALAN DIRI", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, 0, 1, 15, 10);

            chunk = new Chunk(":", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, 0, 1, 15, 1);

            chunk = new Chunk("12345678", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, 0, 4, 15, 1);

            chunk = new Chunk("BULAN", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, 0, 1, 15, 10);

            chunk = new Chunk(":", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, 0, 1, 15, 1);

            chunk = new Chunk("JANUARI", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, 0, 1, 15, 1);

            chunk = new Chunk("TAHUN", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, 0, 1, 15, 1);

            chunk = new Chunk(":", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, 0, 1, 15, 1);

            chunk = new Chunk("2023", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, 0, 1, 15, 1);

            chunk = new Chunk("STATUS", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, 0, 1, 15, 10);

            chunk = new Chunk(":", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, 0, 1, 15, 1);

            chunk = new Chunk("Pemastautin", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, 0, 4, 15, 1);

            chunk = new Chunk("STATUS PERKAHWINAN", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, 0, 1, 15, 10);

            chunk = new Chunk(":", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, 0, 1, 15, 1);

            chunk = new Chunk("Bujang/Pasangan Tidak Menuntut Pelepasan Anak", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, 0, 4, 15, 1);

            chunk = new Chunk("INDIVIDU KURANG UPAYA", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, 0, 1, 15, 10);

            chunk = new Chunk(":", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, 0, 1, 15, 1);

            chunk = new Chunk("Tidak", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, 0, 4, 15, 1);

            chunk = new Chunk("PASANGAN KURANG UPAYA", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, 0, 1, 15, 10);

            chunk = new Chunk(":", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, 0, 1, 15, 1);

            chunk = new Chunk("Tidak", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, 0, 4, 15, 1);

            pdfDoc.Add(table);
            return pdfDoc;
        }

        public Document WorkerChildInfo(Document pdfDoc, string NoPkj)
        {
            PdfPTable table = new PdfPTable(4);
            float[] widths = new float[] { 0.3f, 3, 0.5f, 0.5f };
            table.SetWidths(widths);
            table.WidthPercentage = 100;
            table.SpacingBefore = 20f;

            Chunk chunk = new Chunk("ANAK (KANDUNG/TIRI/ANGKAT)", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK));
            PdfPCell cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, 0, 2, 10, 10);

            chunk = new Chunk("Bilangan anak yang dituntut oleh diri sendiri", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, 0, 2, 10, 1);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, 0, 2, 10, 10);

            chunk = new Chunk("Kadar kelayakan", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, 0, 2, 10, 1);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, 0, 2, 10, 10);

            chunk = new Chunk("100%", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, 0, 1, 10, 1);

            chunk = new Chunk("50%", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, 0, 1, 10, 1);

            chunk = new Chunk("(a)", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 10);

            chunk = new Chunk("Di bawah 18 tahun", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("(b)", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 10);

            chunk = new Chunk("18 tahun dan ke atas yang masih belajar (termasuk sijil/matrikulasi)", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("(c)", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 10);

            chunk = new Chunk("Lebih 18 tahun dan sedang belajar sepenuh masa di peringkat diploma ke atas (Malaysia) atau di peringkat ijazah dan ke atas (luar Malaysia)", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("(d)", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 10);

            chunk = new Chunk("Kurang upaya", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("(e)", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 10);

            chunk = new Chunk("Kurang upaya belajar di IPT", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 1);

            pdfDoc.Add(table);
            return pdfDoc;
        }

        public Document WorkerRemuneration1(Document pdfDoc, string NoPkj)
        {
            PdfPTable table = new PdfPTable(3);
            float[] widths = new float[] { 3, 1, 1 };
            table.SetWidths(widths);
            table.WidthPercentage = 100;
            table.SpacingBefore = 20f;

            Chunk chunk = new Chunk("SARAAN/PCB/REBAT/POTONGAN TERKUMPUL BULAN SEBELUM DALAM TAHUN SEMASA (TERMASUK DI MAJIKAN LAMA)", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK));
            PdfPCell cell = new PdfPCell(new Phrase(chunk));
            CellWithBgColorPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 5, 5, BaseColor.LIGHT_GRAY);

            chunk = new Chunk("(RM)", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellWithBgColorPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 5, 1, BaseColor.LIGHT_GRAY);

            chunk = new Chunk("JUMLAH(RM)", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellWithBgColorPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 5, 10, BaseColor.LIGHT_GRAY);

            chunk = new Chunk("Saraan/Manfaat Berupa Barangan (MBB)/Nilai Tempat Kediaman (NTK) terkumpul", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("KWSP dan Kumpulan Wang Lain Yang Diluluskan", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("Saraan bersih terkumpul", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("PCB bulanan terkumpul yang telah dibayar (termasuk PCB saraan tambahan)", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("Jumlah terkumpul zakat/fitrah yang telah dibayar", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("Jumlah terkumpul levi yang telah dibayar", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("Lain-lain potongan terkumpul", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("1) Perbelanjaan rawatan perubatan, keperluan khas dan penjaga untuk ibu bapa (keadaan kesihatan disahkan oleh pengamal perubatan); ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("2) Peralatan sokongan asas untuk kegunaan sendiri, suami/isteri, anak atau ibu bapa yang kurang upaya ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("3) Yuran Pengajian (sendiri) ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("a) Peringkat selain sarjana atau doktor falsafah - bidang undang-undang, perakaunan, kewangan Islam, teknikal, vokasional, industri, saintifik atau teknologi; ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("b) Peringkat sarjana atau doktor falsafah - sebarang bidang atau kursus pengajian.", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("c) Bayaran kursus peningkatan kemahiran/kemajuan diri", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("Jumlah (a) + (b) + (c)", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("4) Perbelanjaan perubatan bagi: ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("a) Penyakit serius untuk diri sendiri/ pasangan / anak ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("b) Rawatan kesuburan untuk diri sendiri/ pasangan", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("c) Pemvaksinan ke atas diri sendiri / pasangan / anak", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("d) Pemeriksaan perubatan penuh, ujian pengesanan COVID-19 termasuk pembelian kit ujian kendiri, pemeriksaan kesihatan mental atau konsultasi ke atas diri sendiri / pasangan / anak", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("e) Penilaian bagi tujuan diagnosis, program Intervensi awal atau rawatan pemulihan bagi anak kurang upaya pembelanjaran.", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("Jumlah a + b + c + d + e", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("5) Pembelian bahan bacaan, komputer, telefon pintar, tablet, alat sukan, yuran keahlian gimnasium dan langganan internet", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("6) Pembelian peralatan sukan, bayaran sewa atau fi kemasukan ke fasiliti sukan dan fi bayaran pendaftaran pertandingan", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("0.00", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("7) Pembelian peralatan penyusuan ibu untuk kegunaan diri sendiri bagi anak berumur 2 tahun dan ke bawah (Potongan dibenarkan sekali setiap 2 tahun taksiran)", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("0.00", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("8) Yuran penghantaran anak berumur 6 tahun dan ke bawah ke taman asuhan kanak-kanak / tadika yang berdaftar", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("0.00", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("9) Tabungan bersih dalam Skim Simpanan Pendidikan Nasional (Jumlah simpanan dalam tahun semasa tolak jumlah pengeluaran dalam tahun semasa)", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("0.00", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("10) Bayaran alimoni kepada bekas isteri", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("0.00", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("11) KWSP Sukarela / Insurans Nyawa", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("0.00", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("a) KWSP Sukarela;", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("b) Insuran nyawa / KWSP Sukarela", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("Jumlah (a) + (b)", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("0.00", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("12) Skim persaraan swasta dan anuiti tertangguh", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("0.00", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("13) Insurans pendidikan dan perubatan ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("0.00", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("14) Caruman kepada PERKESO mengikut Akta Keselamatan Sosial Pekerja 1969 / Akta Sistem Insurans Pekerjaan 2017", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("0.00", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("15) Perbelanjaan bayaran pemasangan/sewaan/pembelian termasuk sewa-beli peralatan / langganan bagi penggunaan kemudahan pengecasan kenderaan elektrik bagi kenderaan sendiri (Bukan untuk kegunaan perniagaan).", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("0.00", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("Jumlah potongan terkumpul", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("0.00", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            pdfDoc.Add(table);
            return pdfDoc;
        }

        public Document WorkerRemuneration2(Document pdfDoc, string NoPkj)
        {
            PdfPTable table = new PdfPTable(3);
            float[] widths = new float[] { 3, 1, 1 };
            table.SetWidths(widths);
            table.WidthPercentage = 100;
            table.SpacingBefore = 20f;

            Chunk chunk = new Chunk("SARAAN BULAN SEMASA", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK));
            PdfPCell cell = new PdfPCell(new Phrase(chunk));
            CellWithBgColorPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 5, 5, BaseColor.LIGHT_GRAY);

            chunk = new Chunk("(RM)", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellWithBgColorPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 5, 1, BaseColor.LIGHT_GRAY);

            chunk = new Chunk("JUMLAH(RM)", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellWithBgColorPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 5, 10, BaseColor.LIGHT_GRAY);

            chunk = new Chunk("Saraan bulan semasa", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("KWSP dan Kumpulan Wang Lain Yang Diluluskan", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("Jumlah saraan bersih bulan semasa", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("Manfaat Berupa Barangan (MBB)", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("Nilai Tempat Kediaman (NTK)", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            pdfDoc.Add(table);
            return pdfDoc;
        }

        public Document WorkerRemuneration3(Document pdfDoc, string NoPkj)
        {
            PdfPTable table = new PdfPTable(3);
            float[] widths = new float[] { 3, 1, 1 };
            table.SetWidths(widths);
            table.WidthPercentage = 100;
            table.SpacingBefore = 20f;

            Chunk chunk = new Chunk("SARAAN TAMBAHAN BULAN SEMASA", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK));
            PdfPCell cell = new PdfPCell(new Phrase(chunk));
            CellWithBgColorPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 5, 5, BaseColor.LIGHT_GRAY);

            chunk = new Chunk("(RM)", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellWithBgColorPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 5, 1, BaseColor.LIGHT_GRAY);

            chunk = new Chunk("JUMLAH(RM)", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellWithBgColorPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 5, 10, BaseColor.LIGHT_GRAY);

            chunk = new Chunk("(a) Bonus", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("(b) Tunggakan", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("(c) Komisen (tidak dibayar setiap bulan) ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("(d) Ganjaran", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("(e) Pampasan", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("(f) Yuran pengarah (tidak dibayar setiap bulan) ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("(g) Cukai Pendapatan yang dibayar oleh majikan bagi pihak pekerja", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("(h) Lain-lain ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("(i) Jumlah saraan tambahan (a) hingga (h)", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("(j) KWSP saraan tambahan [terhad RM4000 setahun] (a) hingga (h)", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("(k) Jumlah saraan tambahan bersih [(i)-(j)]", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            pdfDoc.Add(table);
            return pdfDoc;
        }

        public Document WorkerDeduction(Document pdfDoc, string NoPkj)
        {
            PdfPTable table = new PdfPTable(3);
            float[] widths = new float[] { 3, 1, 1 };
            table.SetWidths(widths);
            table.WidthPercentage = 100;
            table.SpacingBefore = 20f;

            Chunk chunk = new Chunk("POTONGAN BULAN SEMASA", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK));
            PdfPCell cell = new PdfPCell(new Phrase(chunk));
            CellWithBgColorPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 5, 5, BaseColor.LIGHT_GRAY);

            chunk = new Chunk("(RM)", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellWithBgColorPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 5, 1, BaseColor.LIGHT_GRAY);

            chunk = new Chunk("JUMLAH(RM)", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellWithBgColorPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 5, 10, BaseColor.LIGHT_GRAY);

            chunk = new Chunk("1) Perbelanjaan rawatan perubatan, keperluan khas dan penjaga untuk ibu bapa (keadaan kesihatan disahkan oleh pengamal perubatan); ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("2) Peralatan sokongan asas untuk kegunaan sendiri, suami/isteri, anak atau ibu bapa yang kurang upaya", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("3) Yuran Pengajian (sendiri) ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("a) Peringkat selain sarjana atau doktor falsafah - bidang undang-undang, perakaunan, kewangan Islam, teknikal, vokasional, industri, saintifik atau teknologi;", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("b) Peringkat sarjana atau doktor falsafah - sebarang bidang atau kursus pengajian.", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("c) Bayaran kursus peningkatan kemahiran/kemajuan diri", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("Jumlah (a) + (b) + (c) ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("4) Perbelanjaan perubatan bagi: ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("a) Penyakit serius untuk diri sendiri/ pasangan / anak", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("b) Rawatan kesuburan untuk diri sendiri/ pasangan", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("c) Pemvaksinan ke atas diri sendiri / pasangan / anak", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("d) Pemeriksaan perubatan penuh, ujian pengesanan COVID-19 termasuk pembelian kit ujian kendiri, pemeriksaan kesihatan mental atau konsultasi ke atas diri sendiri / pasangan / anak", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("e) Penilaian bagi tujuan diagnosis, program Intervensi awal atau rawatan pemulihan bagi anak kurang upaya pembelanjaran.", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("Jumlah a + b + c + d + e ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("5) Pembelian bahan bacaan, komputer, telefon pintar, tablet, alat sukan, yuran keahlian gimnasium dan langganan internet", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("0.00", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("6) Pembelian peralatan sukan, bayaran sewa atau fi kemasukan ke fasiliti sukan dan fi bayaran pendaftaran pertandingan", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("0.00", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("7) Pembelian peralatan penyusuan ibu untuk kegunaan diri sendiri bagi anak berumur 2 tahun dan ke bawah (Potongan dibenarkan sekali setiap 2 tahun taksiran)\r\n", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("0.00", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("8) Yuran penghantaran anak berumur 6 tahun dan ke bawah ke taman asuhan kanak-kanak / tadika yang berdafta", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("0.00", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("9) Tabungan bersih dalam Skim Simpanan Pendidikan Nasional (Jumlah simpanan dalam tahun semasa tolak jumlah pengeluaran dalam tahun semasa)", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("0.00", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("10) Bayaran alimoni kepada bekas isteri", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("0.00", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("11) KWSP Sukarela / Insurans Nyawa ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("0.00", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("a) KWSP Sukarela;", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("b) Insuran nyawa / KWSP Sukarela", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("Jumlah (a) + (b)", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("0.00", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("12) Skim persaraan swasta dan anuiti tertangguh", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("0.00", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("13) Insurans pendidikan dan perubatan", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("0.00", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("14) Caruman kepada PERKESO mengikut Akta Keselamatan Sosial Pekerja 1969 / Akta Sistem Insurans Pekerjaan 2017", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("0.00", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("15) Perbelanjaan bayaran pemasangan/sewaan/pembelian termasuk sewa-beli peralatan / langganan bagi penggunaan kemudahan pengecasan kenderaan elektrik bagi kenderaan sendiri (Bukan untuk kegunaan perniagaan).", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("0.00", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("Jumlah Potongan", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("0.00", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            pdfDoc.Add(table);
            return pdfDoc;
        }

        public Document WorkerRebate(Document pdfDoc, string NoPkj)
        {
            PdfPTable table = new PdfPTable(2);
            float[] widths = new float[] { 3, 1};
            table.SetWidths(widths);
            table.WidthPercentage = 100;
            table.SpacingBefore = 20f;

            Chunk chunk = new Chunk("MAKLUMAT REBAT BULAN SEMASA", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK));
            PdfPCell cell = new PdfPCell(new Phrase(chunk));
            CellWithBgColorPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 5, 5, BaseColor.LIGHT_GRAY);

            chunk = new Chunk("JUMLAH(RM)", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellWithBgColorPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 5, 10, BaseColor.LIGHT_GRAY);

            chunk = new Chunk("i. Zakat atau Fitrah", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("0.00", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            chunk = new Chunk("ii. Levi pelepasan bagi perjalanan umrah / perjalanan tujuan keagamaan agama lain (Terhad 2 kali tuntutan seumur hidup) ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("0.00", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            pdfDoc.Add(table);
            return pdfDoc;
        }

        public Document WorkerPCBCalculation(Document pdfDoc, string NoPkj)
        {
            PdfPTable table = new PdfPTable(4);
            float[] widths = new float[] { 0.5f, 0.3f, 3, 0.8f };
            table.SetWidths(widths);
            table.WidthPercentage = 100;
            table.SpacingBefore = 20f;

            Chunk chunk = new Chunk("PENGIRAAN PCB", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK));
            PdfPCell cell = new PdfPCell(new Phrase(chunk));
            CellWithBgColorPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 4, 5, 5, BaseColor.LIGHT_GRAY);

            chunk = new Chunk("1. PCB Ke Atas Saraan Bersih Setahun Tidak Termasuk Saraan Tambahan:", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 3, 10, 5);

            chunk = new Chunk("JUMLAH(RM)", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("PCB Bersih", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk(" = [((P-M)R + B - ( Z + X) )/n+1] - Zakat/Fi/Levi Bulan Semasa", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("P = ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("Jumlah pendapatan bercukai untuk setahun tidak termasuk saraan tambahan semasa \r\n[E(Y-K)+(Y1-K1)+[(Y2-K2)n] + (Yt-Kt)]-(D+S+Du+Su\r\n+QC+ELP+LP1) iaitu(Yt-Kt) = 0", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("E(Y-K) = Jumlah saraan bersih terkumpul termasuk saraan tambahan yang telah dibayar kepada pekerja sehingga sebelum bulan semasa termasuk saraan bersih yang dibayar oleh majikan lama (sekiranya ada);", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("0.00", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("Y = ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("Jumlah saraan kasar bulanan dan saraan tambahan yang telah dibayar termasuk saraan kasar bulanan yang telah dibayar oleh majikan lama (sekiranya ada);", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("0.00", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("K = ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("Jumlah caruman KWSP atau Kumpulan Wang Lain Yang Diluluskan yang telah dibuat ke atas semua saraan (saraan bulanan, saraan tambahan dan saraan daripada majikan terdahulu dalam tahun semasa) yang telah dibayar (termasuk premium yang dituntut di bawah penggajian terdahulu dalam tahun semasa, jika ada) tidak melebihi RM4,000.00 setahun;", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("0.00", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("Y1 = ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("Saraan biasa bulan semasa;", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("0.00", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("K1 = ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("Caruman KWSP atau Kumpulan Wang Lain Yang Diluluskan yang telah dibayar tertakluk kepada jumlah yang layak bagi saraan bulan semasa tidak melebihi RM4,000.00 setahun;", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("0.00", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("Y2 = ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("Anggaran saraan seperti Y1 untuk bulan seterusnya;", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("0.00", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("K2 = ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("Anggaran baki jumlah caruman KWSP atau Kumpulan Wang Lain Yang Diluluskan yang dibayar bagi baki bulan yang layak [[RM4,000 (Terhad) - (K + K1 + Kt)]/n] atau K1, yang mana lebih rendah;", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("0.00", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("n = ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("Baki bulan bekerja dalam setahun;", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("0.00", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("n + 1 = ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("Baki bulan bekerja dalam setahun termasuk bulan semasa;", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("0.00", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("D = ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("Potongan individu;", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("0.00", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("S = ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("Potongan pasangan;", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("0.00", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("Du = ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("Potongan individu kurang upaya.", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("0.00", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("Su = ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("Potongan pasangan kurang upaya.", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("0.00", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("Q = ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("Potongan bagi anak yang layak;", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("0.00", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("C = ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("Bilangan anak yang layak; ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("0.00", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("ELP = ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("Lain-lain potongan terkumpul yang telah dibenarkan termasuk daripada penggajian terdahulu, (sekiranya ada);", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("0.00", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("LP1 = ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("Lain-lain potongan bulan semasa yang dibenarkan;", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("0.00", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("P = ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("[E(2,607.66 - 0.00) + (2,607.66 - 0.00)+[(2,607.66 - 0.00) x 11] + (0-0)]-(9000.00+0.00 + 0.00 + 0.00+(2000 x 0)+ 0.00 + 0.00)", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("0.00", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("M = ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("Amaun pendapatan bercukai yang pertama bagi setiap banjaran pendapatan bercukai setahun; ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("0.00", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("R = ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("Kadar peratusan cukai; ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("0.00", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("B = ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("Amaun cukai atas jumlah M selepas tolak rebat cukai individu dan pasangan (sekiranya layak); ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("0.00", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("Z = ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("Zakat/Fitrah/Levi terkumpul yang telah dibayar tidak termasuk Zakat/Fitrah/Levi bulan semasa; ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("0.00", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("X = ", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("PCB terkumpul yang telah dibayar pada bulan sebelum termasuk daripada penggajian terdahulu (termasuk PCB saraan tambahan);", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("0.00", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("Cukai Setahun", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk(" =", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("0.00", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("PCB Bulan Semasa", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk(" = [( 24,899.58 - 20,000.00)x 0.03 + (-250.00 ) -(0.00 + 0.00 )]/( 11+1) = 0.00", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("0.00", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("PCB Bersih", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk(" = PCB Bulan Semasa- (0)", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("0.00", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("2. PCB Perlu dipotong:", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 3, 10, 5);

            chunk = new Chunk("JUMLAH(RM)", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("PCB", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 2, 10, 5);

            chunk = new Chunk("0.00", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            cell.BackgroundColor = BaseColor.YELLOW;
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 5);

            pdfDoc.Add(table);
            return pdfDoc;
        }

        public Document WorkerPCBResultCalculation(Document pdfDoc, string NoPkj)
        {
            PdfPTable table = new PdfPTable(2);
            float[] widths = new float[] { 3, 1 };
            table.SetWidths(widths);
            table.WidthPercentage = 100;
            table.SpacingBefore = 20f;

            Chunk chunk = new Chunk("HASIL PENGIRAAN", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK));
            PdfPCell cell = new PdfPCell(new Phrase(chunk));
            CellWithBgColorPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 5, 5, BaseColor.LIGHT_GRAY);

            chunk = new Chunk("JUMLAH(RM)", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            CellWithBgColorPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 5, 10, BaseColor.LIGHT_GRAY);

            chunk = new Chunk("PCB Bulan JANUARI Perlu Dipotong (setelah dibundarkan ke perpuluhan atas yang terdekat)", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLUE));
            cell = new PdfPCell(new Phrase(chunk));
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 1, 10, 5);

            chunk = new Chunk("0.00", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            cell.BackgroundColor = BaseColor.YELLOW;
            CellPropoties(cell, table, Element.ALIGN_RIGHT, Element.ALIGN_TOP, 0, 1, 10, 1);

            var phrase = new Phrase();
            chunk = new Chunk("Nota : PCB bagi setiap bulan bermula pada bulan FEBRUARI sehingga bulan DISEMBER adalah RM 0.00 sekiranya tiada perubahan saraan, potongan dan rebat", FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK));
            phrase.Add(chunk);
            chunk = new Chunk(" (PCB tidak dikenakan jika amaun kurang dari RM10)", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK));
            phrase.Add(chunk);

            cell = new PdfPCell(phrase);
            CellPropoties(cell, table, Element.ALIGN_LEFT, Element.ALIGN_TOP, 0, 2, 10, 5);

            pdfDoc.Add(table);
            return pdfDoc;
        }

        public void CellWithBgColorPropoties(PdfPCell cell, PdfPTable table, int HoriElement, int VertiElement, int Border, int Colspan, int PaddingBottom, int PaddingLeft, BaseColor BGColor)
        {
            cell.HorizontalAlignment = HoriElement;
            cell.VerticalAlignment = VertiElement;
            cell.BorderColorTop = BaseColor.WHITE;
            cell.BorderColorBottom = BaseColor.WHITE;
            cell.BorderColorLeft = BaseColor.WHITE;
            cell.BorderColorRight = BaseColor.WHITE;
            cell.BorderWidthLeft = 1;
            cell.BorderWidthRight = 1;
            cell.PaddingBottom = PaddingBottom;
            cell.PaddingLeft = PaddingLeft;
            cell.Colspan = Colspan;
            cell.BackgroundColor = BGColor;
            table.AddCell(cell);
        }

        public void CellPropoties(PdfPCell cell, PdfPTable table, int HoriElement, int VertiElement, int Border, int Colspan, int PaddingBottom, int PaddingLeft)
        {
            cell.HorizontalAlignment = HoriElement;
            cell.VerticalAlignment = VertiElement;
            cell.Border = Border;
            cell.PaddingBottom = PaddingBottom;
            cell.PaddingLeft = PaddingLeft;
            cell.Colspan = Colspan;
            table.AddCell(cell);
        }
    }

    public class PDFFooter : PdfPageEventHelper
    {
        // write on top of document
        public override void OnOpenDocument(PdfWriter writer, Document document)
        {
            base.OnOpenDocument(writer, document);
        }

        // write on start of each page
        public override void OnStartPage(PdfWriter writer, Document document)
        {
            base.OnStartPage(writer, document);
        }

        // write on end of each page
        public override void OnEndPage(PdfWriter writer, Document document)
        {
            base.OnEndPage(writer, document);
            PdfPTable tabFot = new PdfPTable(new float[] { 1F });
            PdfPCell cell;
            tabFot.TotalWidth = 300F;
            Chunk chunk = new Chunk(writer.PageNumber.ToString(), FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.Border = 0;
            tabFot.AddCell(cell);
            tabFot.WriteSelectedRows(0, -1, 280, 25, writer.DirectContent);
        }

        //write on close of document
        public override void OnCloseDocument(PdfWriter writer, Document document)
        {
            base.OnCloseDocument(writer, document);
        }
    }
}