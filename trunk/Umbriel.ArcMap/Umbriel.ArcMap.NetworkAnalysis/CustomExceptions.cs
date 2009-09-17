// <copyright file="CustomExceptions.cs" company="Earth">
// Copyright (c) 2009 All Rights Reserved
// </copyright>
// <author>Jay Cummins</author>
// <email>cumminsjp@gmail.com</email>
// <date>2009-09-17</date>
// <summary>CustomExceptions class file</summary>

namespace Umbriel.ArcMap.NetworkAnalysis
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// CustomExceptions class for Umbriel.ArcMap.NetworkAnalysis
    /// </summary>
    public class CustomExceptions
    {
        /// <summary>
        /// Base exception which all custom exceptions should inherit from
        /// </summary>
        public class BaseException : ApplicationException
        {
            /// <summary>
            /// Gets or sets the custom message.
            /// </summary>
            /// <value>The custom message.</value>
            public string CustomMessage { get;  set; }
        }

        /// <summary>
        /// Custom exception for the CountPath Analysis
        /// </summary>
        public class CountPathFlagException : BaseException
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="CountPathFlagException"/> class.
            /// </summary>
            public CountPathFlagException()
            {
                this.CustomMessage = "Invalid flag configuration.";
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="CountPathFlagException"/> class.
            /// </summary>
            /// <param name="strCustomMessage">The STR custom message.</param>
            /// <param name="junctionFlagCount">The junction flag count.</param>
            /// <param name="edgeFlagCount">The edge flag count.</param>
            public CountPathFlagException(string strCustomMessage, int junctionFlagCount, int edgeFlagCount)
            {
                this.CustomMessage = strCustomMessage;
                this.JunctionFlagCount = junctionFlagCount;
                this.EdgeFlagCount = edgeFlagCount;
            }

            /// <summary>
            /// Gets or sets the junction flag count.
            /// </summary>
            /// <value>The junction flag count.</value>
            public int JunctionFlagCount { get; set; }

            /// <summary>
            /// Gets or sets the edge flag count.
            /// </summary>
            /// <value>The edge flag count.</value>
            public int EdgeFlagCount { get; set; }
        }
    }
}
