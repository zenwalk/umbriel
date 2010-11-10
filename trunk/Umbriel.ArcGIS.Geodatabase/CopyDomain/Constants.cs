// <copyright file="Constants.cs" company="Umbriel Project">
// Copyright (c) 2010 All Right Reserved
// </copyright>
// <author>Jay Cummins</author>
// <email>cumminsjp@gmail.com</email>
// <date>2010-11-10</date>
// <summary>Constants class file</summary>

namespace CopyDomain
{
    /// <summary>
    /// Constants class for the copydomain exe
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Gets the copy start message.
        /// </summary>
        /// <value>The copy start message.</value>
        public static string CopyStartMessage
        {
            get
            {
                return "Copying '{0}'...";
            }
        }

        /// <summary>
        /// Gets the general error message.
        /// </summary>
        /// <value>The general error message.</value>
        public static string GeneralErrorMessage
        {
            get
            {
                return "ERROR: {0}";
            }
        }        
    }
}