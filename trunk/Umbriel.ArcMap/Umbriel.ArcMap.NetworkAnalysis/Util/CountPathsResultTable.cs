using System;
using System.Data;
using System.Collections.Generic;
using System.Text;

namespace Umbriel.ArcMap.NetworkAnalysis.Util
{
    internal class CountPathsResultTable : DataTable
    {
        protected internal CountPathsResultTable()
        {
            AddResultColumns();
        }

        private void AddResultColumns()
        {
            this.Columns.Add(new DataColumn("OID", typeof(int)));

            this.Columns.Add(new DataColumn("WKB", typeof(System.Byte[])));

            this.Columns.Add(new DataColumn("X", typeof(System.Double)));

            this.Columns.Add(new DataColumn("Y", typeof(System.Double)));

            this.Columns.Add(new DataColumn("PATHCOUNT", typeof(int)));

            this.Columns.Add(new DataColumn("FEATCLASSNAME", typeof(string)));
        }

    }
}
