// <copyright file="ShowSelectByLocationButton.cs" company="Umbriel Project">
// Copyright (c) 2010 All Right Reserved
// </copyright>
// <author>Jay Cummins</author>
// <email>cumminsjp@gmail.com</email>
// <date>2010-10-18</date>
// <summary>ShowSelectByLocationButton class file</summary>

namespace ShowSelectByLocation
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;
    using ESRI.ArcGIS.esriSystem;
    using ESRI.ArcGIS.Framework;

    /// <summary>
    /// ArcMap AddIn Button class for a button that Shows the Select By Location 
    /// followed by bringing the Select window to the top of the z-order
    /// </summary>
    public class ShowSelectByLocationButton : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShowSelectByLocationButton"/> class.
        /// </summary>
        public ShowSelectByLocationButton()
        {
        }

        /// <summary>
        /// Called when [click].
        /// </summary>
        protected override void OnClick()
        {
            ArcMap.Application.CurrentTool = null;

            try
            {
                UID uid = new UIDClass();
                uid.Value = "{82B9951B-DD63-11D1-AA7F-00C04FA37860}";  // Select By Location Command

                ICommandItem commandItem = ArcMap.Application.Document.CommandBars.Find(uid, false, false);

                // open the Select By Location dialog (in case it isn't already open)
                if (commandItem != null)
                {
                    commandItem.Execute();
                }
                                
                IntPtr selectwindow = this.FindSelectByLocationWindow();

                if (!selectwindow.Equals(IntPtr.Zero))
                {
                    WindowFunctions.BringWindowToTop(selectwindow);
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// Called when [update].
        /// </summary>
        protected override void OnUpdate()
        {
            Enabled = ArcMap.Application != null;
        }

        /// <summary>
        /// Finds the select by location window.
        /// </summary>
        /// <returns>IntPtr of the Select By Location that matches this ArcMap's process ID</returns>
        private IntPtr FindSelectByLocationWindow()
        {
            IntPtr selectwindow = IntPtr.Zero;

            IntPtr ptr = new IntPtr(ArcMap.Application.hWnd);
            int arcmapPID = WindowFunctions.GetProcessThreadFromWindow(ArcMap.Application.hWnd);

            List<IntPtr> windows = WindowFunctions.GetWindows();

            foreach (IntPtr i in windows)
            {
                int length = WindowFunctions.GetWindowTextLength(i);

                StringBuilder sb = new StringBuilder(length);

                string title = WindowFunctions.GetText(i);

                /*
                 Trace.WriteLine(string.Format(
                 "IntPtr={0},  Window Title={1}",
                 i.ToInt32(),
                 title));
                */
                if (!string.IsNullOrEmpty(title))
                {
                    if (title.Equals("Select By Location"))
                    {
                        int pid = WindowFunctions.GetProcessThreadFromWindow(i.ToInt32());
                        if (pid.Equals(arcmapPID))
                        {
                            selectwindow = i;
                        }
                    }
                }
            }

            return selectwindow;
        }
    }
}
