// <copyright file="LMStatTable.cs" company="Earth">
// Copyright (c) 2009 All Rights Reserved
// </copyright>
// <author>Jay Cummins</author>
// <email>cumminsjp@gmail.com</email>
// <date>2009-11-13</date>
// <summary>
////</summary>

namespace Umbriel.GIS.LMStat
{
    using System.Data;

    /// <summary>
    ///  LMStatTable class 
    /// </summary>
    public class LMStatTable : DataTable
    {
        /// <summary>
        /// Adds the lmstatparser table rows to the data table
        /// </summary>
        /// <param name="rows">The DataRowCollection of lmstatparser rows.</param>
        public void AddRows(DataRowCollection rows)
        {
            foreach (DataRow row in rows)
            {
                DataRow newRow = this.NewRow();
                newRow["MachineName"] = row["MachineName"];
                newRow["LicenseName"] = row["LicenseName"];
                newRow["TotalLicense"] = row["TotalLicense"];
                newRow["InUseLicense"] = row["InUseLicense"];
                newRow["StatusDate"] = row["StatusDateTime"];
                this.Rows.Add(newRow);
            }
        }
    }
}
