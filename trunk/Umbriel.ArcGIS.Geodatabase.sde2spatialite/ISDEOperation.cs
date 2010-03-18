// <copyright file="ISDEOperation.cs" company="Umbriel Project">
// Copyright (c) 2010 All Rights Reserved
// </copyright>
// <author>Jay Cummins</author>
// <email>cumminsjp@gmail.com</email>
// <date>2010-03-14</date>
// <summary>ISDEOperation class file </summary>

namespace Umbriel.ArcGIS.Geodatabase
{
    /// <summary>
    /// ISDEOperation interface
    /// </summary>
    public interface ISDEOperation
    {
        /// <summary>
        /// Gets or sets the command line arguments.
        /// </summary>
        /// <value>The command line arguments.</value>
        CommandLine.Utility.Arguments CommandLineArguments { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [valid arguments].
        /// </summary>
        /// <value><c>true</c> if [valid arguments]; otherwise, <c>false</c>.</value>
        bool ValidArguments { get; set; }

        /// <summary>
        /// Executes the operation.
        /// </summary>
        void Execute();
    }
}
