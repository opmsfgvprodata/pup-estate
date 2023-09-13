using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVC_SYSTEM.Models;
using MVC_SYSTEM.Class;
using MVC_SYSTEM.MasterModels;


namespace MVC_SYSTEM.Class
{
    public class GetEstateDetail
    {
        Connection Connection = new Connection();
        GetIdentity GetIdentity = new GetIdentity();
        GetNSWL GetNSWL = new GetNSWL();
        //added by Faeza on 25.06.2020
        private MVC_SYSTEM_MasterModels db = new MVC_SYSTEM_MasterModels();

        public string GroupName(int groupID,int? getuserid, string getusername)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, getusername);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            string groupname = dbr.vw_KumpulanKerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false && x.fld_KumpulanID == groupID).Select(s => s.fld_Keterangan).FirstOrDefault();
            return groupname;
        }

        public string GroupCode(int? groupID, int? getuserid, string getusername)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, getusername);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            string groupcode = dbr.vw_KumpulanKerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false && x.fld_KumpulanID == groupID).Select(s => s.fld_KodKumpulan).FirstOrDefault();
            return groupcode;
        }

        public string Name(string nopkj, int wlyh, int syrkt, int ngra, int ldg)
        {
            string host, catalog, user, pass = "";
            Connection.GetConnection(out host, out catalog, out user, out pass, wlyh, syrkt, ngra);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            var name = dbr.tbl_Pkjmast.Where(x => x.fld_Nopkj == nopkj && x.fld_WilayahID == wlyh && x.fld_SyarikatID == syrkt && x.fld_NegaraID == ngra && x.fld_LadangID == ldg).Select(s => s.fld_Nama).FirstOrDefault();
            return name;
        }

        //added by Faeza on 25.06.2020
        //*
        public string Division(string nopkj, int wlyh, int syrkt, int ngra, int ldg)
        {
            string host, catalog, user, pass, division = "";
            Connection.GetConnection(out host, out catalog, out user, out pass, wlyh, syrkt, ngra);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            var divisionid = dbr.tbl_Pkjmast.Where(x => x.fld_Nopkj == nopkj && x.fld_WilayahID == wlyh && x.fld_SyarikatID == syrkt && x.fld_NegaraID == ngra && x.fld_LadangID == ldg).Select(s => s.fld_DivisionID).FirstOrDefault();
            division = db.tbl_Division.Where(x => x.fld_ID == divisionid).Select(s => s.fld_DivisionName).FirstOrDefault();
            return division;
        }//*

        public string GetImageUrl(string nopkj, int ngra, int syrkt, int wlyh, int ldg)
        {
            string host, catalog, user, pass = "";
            Connection.GetConnection(out host, out catalog, out user, out pass, wlyh, syrkt, ngra);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            var findImage = dbr.tbl_SupportedDoc.Where(x => x.fld_Nopkj == nopkj && x.fld_Flag == "picPkj" && x.fld_NegaraID == ngra && x.fld_SyarikatID == syrkt && x.fld_WilayahID == wlyh && x.fld_LadangID == ldg && x.fld_Deleted == false).Select(s => s.fld_Url).FirstOrDefault();
            if (findImage == null)
            {
                findImage = "/Asset/Images/default-user.png";
            }
            return findImage;
        }

        public List<tbl_PktUtama> GetPktDetail(int NegaraID, int SyarikatID, int WilayahID, int LadangID)
        {
            string host, catalog, user, pass = "";
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID, SyarikatID, NegaraID);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            var GetPktDetail = dbr.tbl_PktUtama.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).ToList();

            return GetPktDetail;
        }
    }
}