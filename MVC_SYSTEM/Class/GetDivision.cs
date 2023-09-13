using MVC_SYSTEM.MasterModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.Class
{
    public class GetDivision
    {
        //private MVC_SYSTEM_Auth db = new MVC_SYSTEM_Auth();
        private MVC_SYSTEM_MasterModels db = new MVC_SYSTEM_MasterModels();
        public int[] GetDivisionID(int? SyarikatID)
        {
            IEnumerable<int> enumerable = db.tbl_Division.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).Select(s => s.fld_ID).ToArray();
            int[] dvsnid = enumerable.ToArray();

            return dvsnid;
        }

        public int[] GetDivisionID2(int? SyarikatID, int? WilayahID)
        {
            IEnumerable<int> enumerable = db.tbl_Division.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_Deleted == false).Select(s => s.fld_ID).ToArray();
            int[] dvsnid = enumerable.ToArray();

            return dvsnid;
        }
        public int[] GetDivisionID3(int? SyarikatID, int? WilayahID, int? LadangID)
        {
            IEnumerable<int> enumerable = db.tbl_Division.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).Select(s => s.fld_ID).ToArray();
            int[] dvsnid = enumerable.ToArray();

            return dvsnid;
        }
    }
}