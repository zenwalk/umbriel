// <copyright file="CommandLine.Utility.Arguments.cs" company="Umbriel Project">
// none
// </copyright>
// <author>Jay Cummins</author>
// <email>cumminsjp@gmail.com</email>
// <date>2010-03-14</date>
// <summary>Arguments class file</summary>

namespace CommandLine.Utility
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Arguments class
    /// </summary>
    public class Arguments
    {
        /// <summary>
        /// Dictionary containing the command line switches and values
        /// </summary>
        private Dictionary<string, string> parameters;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Arguments"/> class.
        /// </summary>
        /// <param name="args">The command line argument string array</param>
        public Arguments(string[] args)
        {
            this.parameters = new Dictionary<string, string>();

            // Regex Spliter = new Regex(@"^-{1,2}|^/|=|:",
            //    RegexOptions.IgnoreCase | RegexOptions.Compiled);
            Regex spliter = new Regex(@"^-{1,2}|^/|=", RegexOptions.IgnoreCase | RegexOptions.Compiled);

            Regex remover = new Regex(@"^['""]?(.*?)['""]?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

            string parameter = null;
            string[] parts;

            // Valid parameters forms:
            // {-,/,--}param{ ,=,:}((",')value(",'))
            // Examples: 
            // -param1 value1 --param2 /param3:"Test-:-work" 
            //   /param4=happy -param5 '--=nice=--'
            foreach (string txt in args)
            {
                // Look for new parameters (-,/ or --) and a
                // possible enclosed value (=,:)
                parts = spliter.Split(txt, 3);

                switch (parts.Length)
                {
                    // Found a value (for the last parameter 
                    // found (space separator))
                    case 1:
                        if (parameter != null)
                        {
                            if (!this.parameters.ContainsKey(parameter))
                            {
                                parts[0] =
                                    remover.Replace(parts[0], "$1");

                                this.parameters.Add(parameter, parts[0]);
                            }

                            parameter = null;
                        }

                        // else Error: no parameter waiting for a value (skipped)
                        break;

                    // Found just a parameter
                    case 2:
                        // The last parameter is still waiting. 
                        // With no value, set it to true.
                        if (parameter != null)
                        {
                            if (!this.parameters.ContainsKey(parameter))
                            {
                                this.parameters.Add(parameter, "true");
                            }
                        }

                        parameter = parts[1];
                        break;

                    // Parameter with enclosed value
                    case 3:
                        // The last parameter is still waiting. 
                        // With no value, set it to true.
                        if (parameter != null)
                        {
                            if (!this.parameters.ContainsKey(parameter))
                            {
                                this.parameters.Add(parameter, "true");
                            }
                        }

                        parameter = parts[1];

                        // Remove possible enclosing characters (",')
                        if (!this.parameters.ContainsKey(parameter))
                        {
                            parts[2] = remover.Replace(parts[2], "$1");
                            this.parameters.Add(parameter, parts[2]);
                        }

                        parameter = null;
                        break;
                }
            }

            // In case a parameter is still waiting
            if (parameter != null)
            {
                if (!this.parameters.ContainsKey(parameter))
                {
                    this.parameters.Add(parameter, "true");
                }
            }
        }

        /// <summary>
        /// Gets the <see cref="System.String"/> with the specified param.
        /// </summary>
        /// <param name="param">The string parameter (the command line switch)</param>
        /// <value>parameter value</value>
        public string this[string param]
        {
            get
            {
                if (this.parameters.ContainsKey(param))
                {
                    return this.parameters[param];
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Determines whether [contains] [the specified param].
        /// </summary>
        /// <param name="param">The param.</param>
        /// <returns>
        /// <c>true</c> if [contains] [the specified param]; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(string param)
        {
            return this.parameters.ContainsKey(param);
        }
    }
}