// <copyright file="InvalidFieldException.cs" company="Umbriel Project">
// Copyright (c) 2010 All Right Reserved
// </copyright>
// <author>Jay Cummins</author>
// <email>cumminsjp@gmail.com</email>
// <date>2010-03-14</date>
// <summary>InvalidFieldException class file</summary>

using System;

/// <summary>
/// InvalidFieldException class
/// </summary>
[Serializable]
public class InvalidFieldException : _ErrorException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidFieldException"/> class.
    /// </summary>
    /// <param name="errorMessage">The error message.</param>
    public InvalidFieldException(string errorMessage)
        : base(errorMessage)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidFieldException"/> class.
    /// </summary>
    /// <param name="errorMessage">The error message.</param>
    /// <param name="innerEx">The inner ex.</param>
    public InvalidFieldException(string errorMessage, Exception innerEx)
        : base(errorMessage, innerEx)
    {
    }
}