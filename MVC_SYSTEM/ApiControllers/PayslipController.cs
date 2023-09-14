﻿using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using MVC_SYSTEM.Class;
using MVC_SYSTEM.MasterModels;
using MVC_SYSTEM.Models;
using MVC_SYSTEM.ViewingModels;
using MVC_SYSTEM.App_LocalResources;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using MVC_SYSTEM.CustomModels;
using Org.BouncyCastle.Utilities.Collections;
using System.Web.Security;
using System.Web.Script.Serialization;
using MVC_SYSTEM.log;
using MVC_SYSTEM.Attributes;

namespace MVC_SYSTEM.ApiControllers
{
    public class PayslipController : Controller
    {

        private MVC_SYSTEM_MasterModels db = new MVC_SYSTEM_MasterModels();
        GetIdentity GetIdentity = new GetIdentity();
        GetNSWL GetNSWL = new GetNSWL();
        Connection Connection = new Connection();
        ChangeTimeZone timezone = new ChangeTimeZone();
        GetConfig GetConfig = new GetConfig();
        errorlog geterror = new errorlog();
        GetTriager GetTriager = new GetTriager();
        // GET: Payslip
        public FileStreamResult Index(string request_type, int year, int period, string worker_id, string estate_code, string mode)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            string host, catalog, user, pass = "";
            var estateDetail = GetNSWL.GetLadangDetail(estate_code);
            NegaraID = estateDetail.fld_NegaraID;
            SyarikatID = estateDetail.fld_SyarikatID;
            WilayahID = estateDetail.fld_WilayahID;
            LadangID = estateDetail.fld_LadangID;
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            MVC_SYSTEM_SP_Models dbsp = MVC_SYSTEM_SP_Models.ConnectToSqlServer(host, catalog, user, pass);

            Document pdfDoc = new Document(PageSize.A4, 10, 10, 10, 5);
            MemoryStream ms = new MemoryStream();
            MemoryStream output = new MemoryStream();
            PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, ms);
            Chunk chunk = new Chunk();
            Paragraph para = new Paragraph();
            pdfDoc.Open();
            var pkjList = new List<Models.tbl_Pkjmast>();

            pkjList = dbr.tbl_Pkjmast.Where(x => x.fld_Nopkj == worker_id && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).OrderBy(o => o.fld_Nama).ToList();
            
            if (pkjList.Count() > 0)
            {
                foreach (var pkj in pkjList.Select(s => s.fld_Nopkj).Distinct())
                {
                    var getpkjInfo = pkjList.Where(x => x.fld_Nopkj == pkj);

                    var result = dbsp.sp_Payslip(NegaraID, SyarikatID, WilayahID, LadangID, period, year, pkj).ToList();
                    if (result.Count() > 0)
                    {
                        var NamaPkj = getpkjInfo.Select(s => s.fld_Nama).FirstOrDefault();
                        var NoKwsp = getpkjInfo.Select(s => s.fld_Nokwsp).FirstOrDefault();
                        var NoSocso = getpkjInfo.Select(s => s.fld_Noperkeso).FirstOrDefault();
                        var NoKp = getpkjInfo.Select(s => s.fld_Nokp).FirstOrDefault();

                        int? kumpID = getpkjInfo.Select(s => s.fld_KumpulanID).FirstOrDefault();//desc
                        string ktgrPkj = getpkjInfo.Select(s => s.fld_Ktgpkj).FirstOrDefault();//desc
                        string jntnaPkj = getpkjInfo.Select(s => s.fld_Kdjnt).FirstOrDefault();//desc

                        var Kump = dbr.tbl_KumpulanKerja.Where(x => x.fld_KumpulanID == kumpID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false).Select(s => s.fld_Keterangan).FirstOrDefault();
                        var Kategori = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "designation" && x.fldOptConfValue == ktgrPkj && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).Select(s => s.fldOptConfDesc).FirstOrDefault();
                        var Jantina = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "jantina" && x.fldOptConfValue == jntnaPkj && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).Select(s => s.fldOptConfDesc).FirstOrDefault();

                        var NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
                        var NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false).Select(s => s.fld_NoSyarikat).FirstOrDefault();
                        //added by faeza 26.02.2023
                        var NamaLadang = db.tbl_Ladang.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_ID == LadangID).Select(s => s.fld_LdgName).FirstOrDefault();

                        pdfDoc.NewPage();
                        //Header
                        pdfDoc = Header(pdfDoc, NamaSyarikat, "(" + NoSyarikat + ")", NamaLadang, "Worker Payslip Report For Month " + period + "/" + year + "");
                        //Header
                        PdfPTable table = new PdfPTable(6);
                        table.WidthPercentage = 100;
                        table.SpacingBefore = 10f;
                        float[] widths = new float[] { 0.5f, 1, 0.5f, 1, 0.5f, 1 };
                        table.SetWidths(widths);
                        PdfPCell cell = new PdfPCell();
                        chunk = new Chunk("Worker ID: ", FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = 0;
                        table.AddCell(cell);

                        chunk = new Chunk(pkj, FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = 0;
                        table.AddCell(cell);

                        chunk = new Chunk("Worker Name: ", FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = 0;
                        table.AddCell(cell);

                        chunk = new Chunk(NamaPkj, FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = 0;
                        table.AddCell(cell);

                        chunk = new Chunk("KWSP No: ", FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = 0;
                        table.AddCell(cell);

                        chunk = new Chunk(NoKwsp, FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = 0;
                        table.AddCell(cell);

                        chunk = new Chunk("Group Code: ", FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = 0;
                        table.AddCell(cell);

                        chunk = new Chunk(Kump, FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = 0;
                        table.AddCell(cell);

                        chunk = new Chunk("Position: ", FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = 0;
                        table.AddCell(cell);

                        chunk = new Chunk(Kategori, FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = 0;
                        table.AddCell(cell);

                        chunk = new Chunk("Socso No: ", FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = 0;
                        table.AddCell(cell);

                        chunk = new Chunk(NoSocso, FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = 0;
                        table.AddCell(cell);

                        chunk = new Chunk("Gender: ", FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = 0;
                        table.AddCell(cell);

                        chunk = new Chunk(Jantina, FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = 0;
                        table.AddCell(cell);

                        chunk = new Chunk("IC / Passport No: ", FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = 0;
                        table.AddCell(cell);

                        chunk = new Chunk(NoKp, FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = 0;
                        table.AddCell(cell);

                        cell = new PdfPCell();
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = 0;
                        table.AddCell(cell);

                        cell = new PdfPCell();
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = 0;
                        table.AddCell(cell);

                        pdfDoc.Add(table);

                        table = new PdfPTable(8);
                        table.WidthPercentage = 100;
                        table.SpacingBefore = 5f;
                        widths = new float[] { 4f, 1, 1, 1, 1, 1, 4f, 1 };
                        table.SetWidths(widths);

                        chunk = new Chunk("Earnings", FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Colspan = 6;
                        cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                        cell.BorderColor = BaseColor.RED;
                        table.AddCell(cell);

                        chunk = new Chunk("Deduction", FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Colspan = 2;
                        cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                        cell.BorderColor = BaseColor.RED;
                        table.AddCell(cell);

                        chunk = new Chunk("Description", FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                        cell.BorderColor = BaseColor.RED;
                        table.AddCell(cell);

                        chunk = new Chunk("Quantity", FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                        cell.BorderColor = BaseColor.RED;
                        table.AddCell(cell);

                        chunk = new Chunk("Unit", FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                        cell.BorderColor = BaseColor.RED;
                        table.AddCell(cell);

                        chunk = new Chunk("Rate (RM)", FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                        cell.BorderColor = BaseColor.RED;
                        table.AddCell(cell);

                        chunk = new Chunk("Attendance", FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK)); //modified by faeza 13.02.2023
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                        cell.BorderColor = BaseColor.RED;
                        table.AddCell(cell);

                        chunk = new Chunk("Total (RM)", FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                        cell.BorderColor = BaseColor.RED;
                        table.AddCell(cell);

                        chunk = new Chunk("Description", FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                        cell.BorderColor = BaseColor.RED;
                        table.AddCell(cell);

                        chunk = new Chunk("Total (RM)", FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                        cell.BorderColor = BaseColor.RED;
                        table.AddCell(cell);

                        var deductiondata = new List<sp_Payslip_Result>();
                        int i = 1;
                        foreach (var item in result.Where(x => x.fldNopkj == pkj && x.fldFlag == 3))
                        {
                            deductiondata.Add(new sp_Payslip_Result { fldID = i, fldKeterangan = item.fldKeterangan, fldJumlah = item.fldJumlah });
                            i++;
                        }

                        //Added by faeza on 13.02.2023
                        var additiondata = new List<sp_Payslip_Result>();
                        int j = 1;
                        decimal? sumbasicincome = 0;
                        decimal? sumotherincome = 0;
                        string nopekerja = "";
                        foreach (var item in result.Where((x => x.fldNopkj == pkj && x.fldFlag <= 2)))
                        {
                            if (item.fldFlag == 1 && item.fldFlagIncome == 1)
                            {
                                additiondata.Add(new sp_Payslip_Result
                                {
                                    //fldBasicIncomeType = "1",
                                    fldNopkj = item.fldNopkj,
                                    fldKod = item.fldKod,
                                    fldBulan = item.fldBulan,
                                    fldFlag = item.fldFlag,
                                    fldFlagIncome = item.fldFlagIncome,
                                    fldID = j,
                                    fldKodPkt = item.fldKodPkt,
                                    fldKuantiti = item.fldKuantiti,
                                    fldKeterangan = item.fldKeterangan,
                                    fldUnit = item.fldUnit,
                                    fldKadar = item.fldKadar,
                                    fldGandaan = item.fldGandaan,
                                    fldJumlah = item.fldJumlah
                                });
                                j++;
                            }
                            if (item.fldFlag == 2 && item.fldFlagIncome == 2)
                            {
                                additiondata.Add(new sp_Payslip_Result
                                {
                                    //fldBasicIncomeType = "1",
                                    fldNopkj = item.fldNopkj,
                                    fldKod = item.fldKod,
                                    fldBulan = item.fldBulan,
                                    fldFlag = item.fldFlag,
                                    fldFlagIncome = item.fldFlagIncome,
                                    fldID = j,
                                    fldKodPkt = item.fldKodPkt,
                                    fldKuantiti = item.fldKuantiti,
                                    fldKeterangan = item.fldKeterangan,
                                    fldUnit = item.fldUnit,
                                    fldKadar = item.fldKadar,
                                    fldGandaan = item.fldGandaan,
                                    fldJumlah = item.fldJumlah
                                });
                                sumbasicincome = item.fldJumlah + sumbasicincome;
                                j++;
                            }

                            nopekerja = item.fldNopkj;
                        }

                        j = j + 1;

                        additiondata.Add(new sp_Payslip_Result { fldNopkj = nopekerja, fldID = j, fldFlag = 2, fldKeterangan = "Basic Income", fldJumlah = sumbasicincome });

                        j = j + 1;

                        foreach (var item in result.Where((x => x.fldNopkj == pkj && x.fldFlag == 2)))
                        {

                            if (item.fldFlag == 2 && item.fldFlagIncome == 3)
                            {
                                additiondata.Add(new sp_Payslip_Result
                                {
                                    fldNopkj = item.fldNopkj,
                                    fldKod = item.fldKod,
                                    fldBulan = item.fldBulan,
                                    fldFlag = item.fldFlag,
                                    fldFlagIncome = item.fldFlagIncome,
                                    fldID = j,
                                    fldKodPkt = item.fldKodPkt,
                                    fldKuantiti = item.fldKuantiti,
                                    fldKeterangan = item.fldKeterangan,
                                    fldUnit = item.fldUnit,
                                    fldKadar = item.fldKadar,
                                    fldGandaan = item.fldGandaan,
                                    fldJumlah = item.fldJumlah
                                });
                                sumotherincome = item.fldJumlah + sumotherincome;
                                j++;
                            }
                            nopekerja = item.fldNopkj;
                        }
                        additiondata.Add(new sp_Payslip_Result { fldNopkj = nopekerja, fldID = j + 1, fldFlag = 2, fldKeterangan = "Other Income", fldJumlah = sumotherincome });
                        int f = 1;
                        decimal? totalsumotherincome = sumotherincome;

                        foreach (var item in additiondata.Where(x => x.fldNopkj == pkj && x.fldFlag <= 2))
                        {
                            if (item.fldKeterangan == "Basic Income" || item.fldKeterangan == "Other Income")
                            {

                                chunk = new Chunk("", FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                                cell = new PdfPCell(new Phrase(chunk));
                                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER;
                                cell.BorderColor = BaseColor.BLACK;
                                table.AddCell(cell);

                                chunk = new Chunk("", FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                                cell = new PdfPCell(new Phrase(chunk));
                                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER;
                                cell.BorderColor = BaseColor.BLACK;
                                table.AddCell(cell);

                                chunk = new Chunk("", FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                                cell = new PdfPCell(new Phrase(chunk));
                                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER;
                                cell.BorderColor = BaseColor.BLACK;
                                table.AddCell(cell);

                                chunk = new Chunk("", FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                                cell = new PdfPCell(new Phrase(chunk));
                                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER;
                                cell.BorderColor = BaseColor.BLACK;
                                table.AddCell(cell);

                                chunk = new Chunk(item.fldKeterangan.ToString(), FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                                cell = new PdfPCell(new Phrase(chunk));
                                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER;
                                cell.BorderColor = BaseColor.BLACK;
                                table.AddCell(cell);

                                chunk = new Chunk(GetTriager.GetTotalForMoney(item.fldJumlah), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                                cell = new PdfPCell(new Phrase(chunk));
                                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER;
                                cell.BorderColor = BaseColor.BLACK;
                                table.AddCell(cell);
                            }
                            else
                            {
                                if (item.fldKodPkt != null)
                                {
                                    chunk = new Chunk(item.fldKeterangan + " - " + item.fldKodPkt, FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                                }
                                else
                                {
                                    chunk = new Chunk(item.fldKeterangan, FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                                }
                                cell = new PdfPCell(new Phrase(chunk));
                                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                cell.Border = Rectangle.BOTTOM_BORDER;
                                cell.BorderColor = BaseColor.BLACK;
                                table.AddCell(cell);

                                chunk = new Chunk(GetTriager.GetDashForNull(item.fldKuantiti.ToString()), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                                cell = new PdfPCell(new Phrase(chunk));
                                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                cell.Border = Rectangle.BOTTOM_BORDER;
                                cell.BorderColor = BaseColor.BLACK;
                                table.AddCell(cell);

                                chunk = new Chunk(GetTriager.GetDashForNull(item.fldUnit), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                                cell = new PdfPCell(new Phrase(chunk));
                                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                cell.Border = Rectangle.BOTTOM_BORDER;
                                cell.BorderColor = BaseColor.BLACK;
                                table.AddCell(cell);

                                chunk = new Chunk(GetTriager.GetDashForNull(item.fldKadar.ToString()), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                                cell = new PdfPCell(new Phrase(chunk));
                                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                cell.Border = Rectangle.BOTTOM_BORDER;
                                cell.BorderColor = BaseColor.BLACK;
                                table.AddCell(cell);

                                chunk = new Chunk(GetTriager.GetDashForNull(item.fldGandaan.ToString()), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                                cell = new PdfPCell(new Phrase(chunk));
                                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                cell.Border = Rectangle.BOTTOM_BORDER;
                                cell.BorderColor = BaseColor.BLACK;
                                table.AddCell(cell);

                                chunk = new Chunk(GetTriager.GetTotalForMoney(item.fldJumlah), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                                cell = new PdfPCell(new Phrase(chunk));
                                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                cell.Border = Rectangle.BOTTOM_BORDER;
                                cell.BorderColor = BaseColor.BLACK;
                                table.AddCell(cell);

                            }
                            var getdeduction = deductiondata.Where(x => x.fldID == f).FirstOrDefault();
                            if (getdeduction != null)
                            {
                                chunk = new Chunk(item.fldKeterangan, FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                                cell = new PdfPCell(new Phrase(chunk));
                                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                cell.Border = Rectangle.BOTTOM_BORDER;
                                cell.BorderColor = BaseColor.BLACK;
                                table.AddCell(cell);

                                chunk = new Chunk(GetTriager.GetTotalForMoney(getdeduction.fldJumlah), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                                cell = new PdfPCell(new Phrase(chunk));
                                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                cell.Border = Rectangle.BOTTOM_BORDER;
                                cell.BorderColor = BaseColor.BLACK;
                                table.AddCell(cell);
                            }
                            else
                            {
                                cell = new PdfPCell();
                                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                cell.Border = 0;
                                table.AddCell(cell);

                                cell = new PdfPCell();
                                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                cell.Border = 0;
                                table.AddCell(cell);
                            }
                            f++;
                        }

                        chunk = new Chunk("Total Earnings", FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Colspan = 5;
                        cell.Border = Rectangle.TOP_BORDER;
                        cell.BorderColor = BaseColor.RED;
                        table.AddCell(cell);

                        decimal? TotalPendapatan = result.Where(x => x.fldFlag == 2).Select(s => s.fldJumlah).Sum();

                        chunk = new Chunk(GetTriager.GetTotalForMoney(TotalPendapatan), FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Colspan = 1;
                        cell.Border = Rectangle.TOP_BORDER | Rectangle.BOTTOM_BORDER;
                        cell.BorderColor = BaseColor.RED;
                        table.AddCell(cell);

                        chunk = new Chunk("Total Deductions", FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Colspan = 1;
                        cell.Border = Rectangle.TOP_BORDER;
                        cell.BorderColor = BaseColor.RED;
                        table.AddCell(cell);

                        decimal? TotalPotongan = deductiondata.Select(s => s.fldJumlah).Sum();

                        chunk = new Chunk(GetTriager.GetTotalForMoney(TotalPotongan), FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Colspan = 1;
                        cell.Border = Rectangle.TOP_BORDER | Rectangle.BOTTOM_BORDER;
                        cell.BorderColor = BaseColor.RED;
                        table.AddCell(cell);

                        decimal GajiBersih = TotalPendapatan.Value - TotalPotongan.Value;

                        chunk = new Chunk("Net Salary", FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Colspan = 7;
                        cell.Border = 0;
                        table.AddCell(cell);

                        chunk = new Chunk(GetTriager.GetTotalForMoney(GajiBersih), FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Colspan = 1;
                        cell.Border = Rectangle.BOTTOM_BORDER;
                        cell.BorderColor = BaseColor.RED;
                        table.AddCell(cell);

                        pdfDoc.Add(table);

                        Paragraph line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLACK, Element.ALIGN_LEFT, 1)));
                        pdfDoc.Add(line);

                        table = new PdfPTable(1);
                        table.WidthPercentage = 100;

                        chunk = new Chunk("*Attendance : 1 = Weekdays, 2 = Weekend, 3 = Public Holiday\n*Bonus Price Multiples : 0.5 = 50% Achievement, 1 = 100% Achievement", FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK));
                        PdfPCell cell1 = new PdfPCell(new Phrase(chunk));
                        cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell1.VerticalAlignment = Element.ALIGN_TOP;
                        cell1.Border = 0;
                        table.AddCell(cell1);

                        pdfDoc.Add(table);

                        PdfPTable maintable = new PdfPTable(1);
                        maintable.WidthPercentage = 100;
                        maintable.SpacingBefore = 5f;

                        table = new PdfPTable(6);
                        table.WidthPercentage = 100;
                        table.HorizontalAlignment = 0;
                        widths = new float[] { 1, 1, 1, 1, 1, 1 };
                        table.SetWidths(widths);

                        chunk = new Chunk("Details", FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Colspan = 6;
                        cell.Border = Rectangle.BOTTOM_BORDER;
                        cell.BorderColor = BaseColor.RED;
                        table.AddCell(cell);

                        //get Hadir and cuti Count
                        var hdr = dbr.tbl_Kerjahdr.Where(x => x.fld_Nopkj == pkj && x.fld_Tarikh.Value.Month == period && x.fld_Tarikh.Value.Year == year && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).ToList();
                        var hdrhrbs = hdr.Where(x => x.fld_Kdhdct == "H01").Count();

                        var hdrhrmg = hdr.Where(x => x.fld_Kdhdct == "H02").Count();

                        var hdrhrcu = hdr.Where(x => x.fld_Kdhdct == "H03").Count();

                        var hdrhrpg = hdr.Where(x => x.fld_Kdhdct == "P01").Count();

                        var hdrhrct = hdr.Where(x => x.fld_Kdhdct == "C02").Count();

                        var hdrhrtg = hdr.Where(x => x.fld_Kdhdct == "C05").Count();

                        var hdrhrcs = hdr.Where(x => x.fld_Kdhdct == "C03").Count();

                        var hdrhrca = hdr.Where(x => x.fld_Kdhdct == "C01").Count();

                        var hdrhrcm = hdr.Where(x => x.fld_Kdhdct == "C07").Count();

                        var hdrhrcb = hdr.Where(x => x.fld_Kdhdct == "C04").Count();

                        //get hdr OT
                        var hdrot = dbr.vw_KerjaHdrOT.Where(x => x.fld_Nopkj == pkj && x.fld_Tarikh.Value.Month == period && x.fld_Tarikh.Value.Year == year && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).ToList();
                        var hdrothrbs = hdrot.Where(x => x.fld_Kdhdct == "H01").Sum(s => s.fld_JamOT);
                        hdrothrbs = hdrothrbs == null ? 0m : hdrothrbs;

                        var hdrothrcm = hdrot.Where(x => x.fld_Kdhdct == "H02").Sum(s => s.fld_JamOT);
                        hdrothrcm = hdrothrcm == null ? 0m : hdrothrcm;

                        var hdrothrcu = hdrot.Where(x => x.fld_Kdhdct == "H03").Sum(s => s.fld_JamOT);
                        hdrothrcu = hdrothrcu == null ? 0m : hdrothrcu;

                        //get Jumlah Hari Kerja
                        //int? hrkrja = 0;//db.tbl_HariBekerjaLadang.Where(x => x.fld_Month == MonthList && x.fld_Year == YearList && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => s.fld_BilHariBekerja).FirstOrDefault();
                        int? hrkrja = db.tbl_HariBekerja.Where(x => x.fld_Year == year && x.fld_Month == period && x.fld_NegaraID == NegaraID && x.fld_NegeriID == 15 && x.fld_Deleted == false).Select(x => x.fld_BilanganHariBekerja).FirstOrDefault();
                        if (db.tbl_HariBekerja.Where(x => x.fld_Year == year && x.fld_Month == period && x.fld_NegaraID == NegaraID && x.fld_NegeriID == 15 && x.fld_Deleted == false).Select(x => x.fld_BilanganHariBekerja).FirstOrDefault() == null)
                        { hrkrja = 0; }

                        //get jmlh hari hadir
                        var cdct = new string[] { "H01", "H02", "H03" };
                        var jmlhhdr = hdr.Where(x => cdct.Contains(x.fld_Kdhdct)).Count();


                        //get avg slry
                        DateTime cdate = new DateTime(year, period, 15);
                        DateTime ldate = cdate.AddMonths(-1);
                        var crmnthavgslry = dbr.tbl_GajiBulanan.Where(x => x.fld_Month == cdate.Month && x.fld_Year == cdate.Year && x.fld_Nopkj == pkj && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => s.fld_PurataGaji).FirstOrDefault();
                        crmnthavgslry = crmnthavgslry == null ? 0m : crmnthavgslry;

                        var lsmnthavgslry = dbr.tbl_GajiBulanan.Where(x => x.fld_Month == ldate.Month && x.fld_Year == ldate.Year && x.fld_Nopkj == pkj && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => s.fld_PurataGaji).FirstOrDefault();
                        lsmnthavgslry = lsmnthavgslry == null ? 0m : lsmnthavgslry;

                        chunk = new Chunk("Jumlah Hadir Hari Biasa", FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.BOTTOM_BORDER;
                        cell.BorderColor = BaseColor.BLACK;
                        table.AddCell(cell);

                        chunk = new Chunk(hdrhrbs.ToString(), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.BOTTOM_BORDER;
                        cell.BorderColor = BaseColor.BLACK;
                        table.AddCell(cell);

                        chunk = new Chunk("Jumlah Cuti Sakit", FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.BOTTOM_BORDER;
                        cell.BorderColor = BaseColor.BLACK;
                        table.AddCell(cell);

                        chunk = new Chunk(hdrhrcs.ToString(), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.BOTTOM_BORDER;
                        cell.BorderColor = BaseColor.BLACK;
                        table.AddCell(cell);

                        chunk = new Chunk("Jumlah OT - Hari Biasa (Jam)", FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.BOTTOM_BORDER;
                        cell.BorderColor = BaseColor.BLACK;
                        table.AddCell(cell);

                        chunk = new Chunk(hdrothrbs.ToString(), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.BOTTOM_BORDER;
                        cell.BorderColor = BaseColor.BLACK;
                        table.AddCell(cell);

                        chunk = new Chunk("Jumlah Hadir Hari Minggu", FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.BOTTOM_BORDER;
                        cell.BorderColor = BaseColor.BLACK;
                        table.AddCell(cell);

                        chunk = new Chunk(hdrhrmg.ToString(), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.BOTTOM_BORDER;
                        cell.BorderColor = BaseColor.BLACK;
                        table.AddCell(cell);

                        chunk = new Chunk("Jumlah Cuti Hospitalisasi", FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.BOTTOM_BORDER;
                        cell.BorderColor = BaseColor.BLACK;
                        table.AddCell(cell);

                        chunk = new Chunk("0", FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.BOTTOM_BORDER;
                        cell.BorderColor = BaseColor.BLACK;
                        table.AddCell(cell);

                        chunk = new Chunk("Jumlah OT - Hari Cuti Minggu (Jam)", FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.BOTTOM_BORDER;
                        cell.BorderColor = BaseColor.BLACK;
                        table.AddCell(cell);

                        chunk = new Chunk(hdrothrcm.ToString(), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.BOTTOM_BORDER;
                        cell.BorderColor = BaseColor.BLACK;
                        table.AddCell(cell);

                        chunk = new Chunk("Jumlah Hadir Hari Cuti Umum", FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.BOTTOM_BORDER;
                        cell.BorderColor = BaseColor.BLACK;
                        table.AddCell(cell);

                        chunk = new Chunk(hdrhrcu.ToString(), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.BOTTOM_BORDER;
                        cell.BorderColor = BaseColor.BLACK;
                        table.AddCell(cell);

                        chunk = new Chunk("Jumlah Cuti Umum", FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.BOTTOM_BORDER;
                        cell.BorderColor = BaseColor.BLACK;
                        table.AddCell(cell);

                        chunk = new Chunk(hdrhrca.ToString(), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.BOTTOM_BORDER;
                        cell.BorderColor = BaseColor.BLACK;
                        table.AddCell(cell);

                        chunk = new Chunk("Jumlah OT - Hari Cuti Umum (Jam)", FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.BOTTOM_BORDER;
                        cell.BorderColor = BaseColor.BLACK;
                        table.AddCell(cell);

                        chunk = new Chunk(hdrothrcu.ToString(), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.BOTTOM_BORDER;
                        cell.BorderColor = BaseColor.BLACK;
                        table.AddCell(cell);

                        chunk = new Chunk("Jumlah Ponteng", FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.BOTTOM_BORDER;
                        cell.BorderColor = BaseColor.BLACK;
                        table.AddCell(cell);

                        chunk = new Chunk(hdrhrpg.ToString(), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.BOTTOM_BORDER;
                        cell.BorderColor = BaseColor.BLACK;
                        table.AddCell(cell);

                        chunk = new Chunk("Jumlah Cuti Hari Minggu", FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.BOTTOM_BORDER;
                        cell.BorderColor = BaseColor.BLACK;
                        table.AddCell(cell);

                        chunk = new Chunk(hdrhrcm.ToString(), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.BOTTOM_BORDER;
                        cell.BorderColor = BaseColor.BLACK;
                        table.AddCell(cell);

                        chunk = new Chunk("Jumlah Hari Bekerja", FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.BOTTOM_BORDER;
                        cell.BorderColor = BaseColor.BLACK;
                        table.AddCell(cell);

                        chunk = new Chunk(hrkrja.ToString(), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.BOTTOM_BORDER;
                        cell.BorderColor = BaseColor.BLACK;
                        table.AddCell(cell);

                        chunk = new Chunk("Jumlah Cuti Tahunan", FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.BOTTOM_BORDER;
                        cell.BorderColor = BaseColor.BLACK;
                        table.AddCell(cell);

                        chunk = new Chunk(hdrhrct.ToString(), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.BOTTOM_BORDER;
                        cell.BorderColor = BaseColor.BLACK;
                        table.AddCell(cell);

                        chunk = new Chunk("Jumlah Cuti Bersalin", FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.BOTTOM_BORDER;
                        cell.BorderColor = BaseColor.BLACK;
                        table.AddCell(cell);

                        chunk = new Chunk(hdrhrcb.ToString(), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.BOTTOM_BORDER;
                        cell.BorderColor = BaseColor.BLACK;
                        table.AddCell(cell);

                        chunk = new Chunk("Jumlah Hari Hadir", FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.BOTTOM_BORDER;
                        cell.BorderColor = BaseColor.BLACK;
                        table.AddCell(cell);

                        chunk = new Chunk(jmlhhdr.ToString(), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.BOTTOM_BORDER;
                        cell.BorderColor = BaseColor.BLACK;
                        table.AddCell(cell);

                        chunk = new Chunk("Jumlah Hari Terabai", FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.BOTTOM_BORDER;
                        cell.BorderColor = BaseColor.BLACK;
                        table.AddCell(cell);

                        chunk = new Chunk("0", FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.BOTTOM_BORDER;
                        cell.BorderColor = BaseColor.BLACK;
                        table.AddCell(cell);

                        chunk = new Chunk("", FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.BOTTOM_BORDER;
                        cell.BorderColor = BaseColor.BLACK;
                        table.AddCell(cell);

                        chunk = new Chunk("", FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.BOTTOM_BORDER;
                        cell.BorderColor = BaseColor.BLACK;
                        table.AddCell(cell);

                        chunk = new Chunk("Purata Gaji Bulan ini", FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.BOTTOM_BORDER;
                        cell.BorderColor = BaseColor.BLACK;
                        table.AddCell(cell);

                        chunk = new Chunk(crmnthavgslry.ToString(), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.BOTTOM_BORDER;
                        cell.BorderColor = BaseColor.BLACK;
                        table.AddCell(cell);

                        chunk = new Chunk("Jumlah Cuti Tanpa Gaji", FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = 0;
                        table.AddCell(cell);

                        chunk = new Chunk(hdrhrtg.ToString(), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = 0;
                        table.AddCell(cell);

                        chunk = new Chunk("", FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = 0;
                        table.AddCell(cell);

                        chunk = new Chunk("", FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = 0;
                        table.AddCell(cell);

                        chunk = new Chunk("Purata Gaji Bulan Lepas", FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = 0;
                        table.AddCell(cell);

                        chunk = new Chunk(lsmnthavgslry.ToString(), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = 0;
                        table.AddCell(cell);

                        cell = new PdfPCell(table);
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER;
                        cell.BorderColor = BaseColor.RED;
                        maintable.AddCell(cell);

                        pdfDoc.Add(maintable);
                    }
                }
            }
            else
            {
                PdfPTable table = new PdfPTable(1);
                table.WidthPercentage = 100;
                PdfPCell cell = new PdfPCell();
                chunk = new Chunk("No Data Found", FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK));
                cell = new PdfPCell(new Phrase(chunk));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = 0;
                table.AddCell(cell);
                pdfDoc.Add(table);
            }

            //pdfDoc = Footer(pdfDoc, chunk, para);
            pdfWriter.CloseStream = false;
            pdfDoc.Close();
            byte[] file = ms.ToArray();
            output.Write(file, 0, file.Length);
            output.Position = 0;
            return new FileStreamResult(output, "application/pdf");
        }

        public Document WorkerPaySlipContent(Document pdfDoc, List<sp_Payslip_Result> item)
        {
            return pdfDoc;
        }

        public Document Header(Document pdfDoc, string headername, string headername2, string headername1, string headername3)
        {
            Paragraph date = new Paragraph(new Chunk("Date : " + timezone.gettimezone().ToString("dd/MM/yyyy"), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK)));
            date.Alignment = Element.ALIGN_RIGHT;
            pdfDoc.Add(date);
            PdfPTable table = new PdfPTable(1);
            table.WidthPercentage = 100;
            Image image = Image.GetInstance(Server.MapPath("~/Asset/Images/logo_FTPSB.jpg"));
            PdfPCell cell = new PdfPCell(image);
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.Border = 0;
            image.ScaleAbsolute(50, 50);
            table.AddCell(cell);

            Chunk chunk = new Chunk(headername, FontFactory.GetFont("Arial", 9, Font.BOLD, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.Border = 0;
            table.AddCell(cell);
            chunk = new Chunk(headername2, FontFactory.GetFont("Arial", 9, Font.BOLD, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.Border = 0;
            table.AddCell(cell);
            chunk = new Chunk(headername1, FontFactory.GetFont("Arial", 9, Font.BOLD, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.Border = 0;
            table.AddCell(cell);
            chunk = new Chunk(headername3, FontFactory.GetFont("Arial", 9, Font.BOLD, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.Border = 0;
            table.AddCell(cell);
            pdfDoc.Add(table);
            return pdfDoc;
        }

        public Document Footer(Document pdfDoc, Chunk chunk, Paragraph para)
        {

            return pdfDoc;
        }
    }
}