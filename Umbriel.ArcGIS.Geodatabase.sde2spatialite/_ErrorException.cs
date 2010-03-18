// <copyright file="_ErrorException.cs" company="Umbriel Project">
// Copyright (c) 2010 All Right Reserved
// </copyright>
// <author>Jay Cummins</author>
// <email>cumminsjp@gmail.com</email>
// <date>2010-03-14</date>
// <summary>_ErrorException class file</summary>

using System;

/// <summary>
/// ErrorException base class
/// </summary>
    [Serializable]
    public class _ErrorException 
        : Exception
    {
         /// <summary>
        /// Initializes a new instance of the <see cref="_ErrorException"/> class.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        public _ErrorException(string errorMessage)
            : base(errorMessage) 
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="_ErrorException"/> class.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="innerEx">The inner ex.</param>
        public _ErrorException(string errorMessage, Exception innerEx)
            : base(errorMessage, innerEx) 
        {
        }

        /// <summary>
        /// Gets the error message.
        /// </summary>
        /// <value>The error message.</value>
        public string ErrorMessage
        {
            get
            {
                return this.Message.ToString();
            }
        }
    }

