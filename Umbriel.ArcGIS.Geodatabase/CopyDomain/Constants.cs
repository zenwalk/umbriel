// <copyright file="Constants.cs" company="Umbriel Project">
// Copyright (c) 2010 All Right Reserved
// </copyright>
// <author>Jay Cummins</author>
// <email>cum30@co.henrico.va.us,cumminsjp@gmail.com</email>
// <date>2010-11-10</date>
// <summary>Constants class file</summary>

namespace CopyDomain
{
    using System;
    using System.Diagnostics;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public static class Constants
    {
        /// <summary>
        /// Gets the length of the min street name.
        /// </summary>
        /// <value>The length of the min street name.</value>
        public static string CopyStartMessage
        {
            get
            {
                return "Copying '{0}'...";
            }
        }

        public static string GeneralErrorMessage
        {
            get
            {
                return "ERROR: {0}";
            }
        }        
    }
}


    


