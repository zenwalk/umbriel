// <copyright file="Extensions.cs" company="Umbriel Project">
// Copyright (c) 2010 All Right Reserved
// </copyright>
// <author>Jay Cummins</author>
// <email>cumminsjp@gmail.com</email>
// <date>2010-12-20</date>
// <summary>Extensions class file</summary>

namespace DumpConnection
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// static class of extension methods to use with ESRI ArcObjects
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Extension method for string.Format
        /// </summary>
        /// <param name="format">The string to be formatted.</param>
        /// <param name="args">object parameters.</param>
        /// <returns>formatted string</returns>
        public static string FormatString(this string format, params object[] args)
        {
            return string.Format(format, args);
        }
    }
}
