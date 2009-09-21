// <copyright file="CommandArguments.cs" company="Earth">
// Copyright (c) 2009 All Right Reserved
// </copyright>
// <author>Jay Cummins</author>
// <email>cumminsjp@gmail.com</email>
// <date>2009-09-21</date>
// <summary>CommandArguments Class File
//   Revision History:
//   Name:             Date:                  Description:
//   JCummins       2009-09-21    initial creation
// </summary>

namespace CommandLine.Utility
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Text.RegularExpressions;

    /// <summary>
    /// CommandArguments class to provide basic access to dash-switched arguments
    ///  inherits from Collection T"/>
    /// </summary>
    public class CommandArguments : Collection<string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandArguments"/> class.
        /// </summary>
        /// <param name="args">The command line args.</param>
        public CommandArguments(string[] args)
            : base(args)
        {
        }

        /// <summary>
        /// Determines whether [contains] [the specified value].
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="comparisonType">Type of the comparison.</param>
        /// <returns>
        ///  <c>true</c> if [contains] [the specified value]; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(string value, StringComparison comparisonType)
        {
            foreach (string item in this)
            {
                if (item.Equals(value, comparisonType))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>argument value</returns>
        public string GetValue(string value)
        {
            return this.GetValue(value, StringComparison.CurrentCulture);
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="argument">The argument.</param>
        /// <param name="comparisonType">Type of the comparison.</param>
        /// <returns>argument value</returns>
        public string GetValue(string argument, StringComparison comparisonType)
        {
            string value = string.Empty;

            for (int i = 0; i < this.Count; i++)
            {
                if (this[i].Equals(argument, comparisonType))
                {
                    if (i < this.Count)
                    {
                        value = this[i + 1];
                    }
                }
            }

            return value;
        }
    }
}
