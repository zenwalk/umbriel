// <copyright file="??.cs" company="">
// Copyright (c) 2010 All Right Reserved
// </copyright>
// <author>Jay Cummins</author>
// <email>cum30@co.henrico.va.us,cumminsjp@gmail.com</email>
// <date>2010-??-??</date>
// <summary> class file</summary>

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

        

        /// <summary>
        /// Gets the length of the max street name.
        /// </summary>
        /// <value>The length of the max street name.</value>
        public static int MaxStreetNameLength
        {
            get
            {
                return 34;
            }
        }

        /// <summary>
        /// Gets the size of the min street number.
        /// </summary>
        /// <value>The size of the min street number.</value>
        public static int MinStreetNumberSize
        {
            get
            {
                return 1;
            }
        }

        /// <summary>
        /// Gets the size of the max street number.
        /// </summary>
        /// <value>The size of the max street number.</value>
        public static int MaxStreetNumberSize
        {
            get
            {
                return 99999;
            }
        }
    }
}


    


