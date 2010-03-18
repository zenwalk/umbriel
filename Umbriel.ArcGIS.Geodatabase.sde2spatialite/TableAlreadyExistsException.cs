// <copyright file="TableAlreadyExistsException.cs" company="Umbriel Project">
// Copyright (c) 2010 All Right Reserved
// </copyright>
// <author>Jay Cummins</author>
// <email>cumminsjp@gmail.com</email>
// <date>2010-03-14</date>
// <summary>TableAlreadyExistsException class file</summary>

using System;

/// <summary>
/// TableAlreadyExists Exception class
/// </summary>
[Serializable]
public class TableAlreadyExistsException : _ErrorException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TableAlreadyExistsException"/> class.
    /// </summary>
    /// <param name="errorMessage">The error message.</param>
    /// <param name="tablename">The tablename.</param>
    public TableAlreadyExistsException(string errorMessage, string tablename)
        : base(errorMessage)
    {
        this.TableName = tablename;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TableAlreadyExistsException"/> class.
    /// </summary>
    /// <param name="errorMessage">The error message.</param>
    /// <param name="innerEx">The inner ex.</param>
    public TableAlreadyExistsException(string errorMessage, Exception innerEx)
        : base(errorMessage, innerEx)
    {
    }

    /// <summary>
    /// Gets the name of the table.
    /// </summary>
    /// <value>The name of the table.</value>
    public string TableName { get; private set; }
}