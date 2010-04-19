using System;
using System.Collections.Generic;
using System.Text;

namespace Umbriel.Utility
{
    public class StreetAddressGenerator
    {
        #region Fields

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="StreetAddressGenerator"/> class.
        /// </summary>
        public StreetAddressGenerator()
        {
            this.StreetSuffixes = new List<string>(Constants.USPSStreetSuffixes);

            this.MaxStreetNameLength = Constants.MaxStreetNameLength;
            this.MinStreetNameLength = Constants.MinStreetNameLength;

            this.MinStreetNumberSize = Constants.MinStreetNumberSize;
            this.MaxStreetNumberSize = Constants.MaxStreetNumberSize;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StreetAddressGenerator"/> class.
        /// </summary>
        /// <param name="streetSuffixes">The street suffixes.</param>
        public StreetAddressGenerator(List<string> streetSuffixes)
        {
            if (streetSuffixes != null)
            {
                this.StreetSuffixes = streetSuffixes;
            }

            this.MaxStreetNameLength = Constants.MaxStreetNameLength;
            this.MinStreetNameLength = Constants.MinStreetNameLength;

            this.MinStreetNumberSize = Constants.MinStreetNumberSize;
            this.MaxStreetNumberSize = Constants.MaxStreetNumberSize;
        }

        #endregion

        #region Properties


        /// <summary>
        /// Gets or sets the size of the min street number.
        /// </summary>
        /// <value>The size of the min street number.</value>
        public int MinStreetNumberSize { get; set; }

        /// <summary>
        /// Gets or sets the size of the max street number.
        /// </summary>
        /// <value>The size of the max street number.</value>
        public int MaxStreetNumberSize { get; set; }

        /// <summary>
        /// Gets or sets the length of the min street name.
        /// </summary>
        /// <value>The length of the min street name.</value>
        public int MinStreetNameLength { get; set; }

        /// <summary>
        /// Gets or sets the length of the max street name.
        /// </summary>
        /// <value>The length of the max street name.</value>
        public int MaxStreetNameLength { get; set; }

        /// <summary>
        /// Gets or sets the street suffixes.
        /// </summary>
        /// <value>The street suffixes.</value>
        public List<string> StreetSuffixes { get; set; }
        #endregion

        #region Methods

        public string GenerateSingleAddress()
        {
            RandomDataGenerator dataGenerator = new RandomDataGenerator();

            string streetName = dataGenerator.RandomString(this.MinStreetNameLength, this.MaxStreetNameLength);

            string streetNumber = dataGenerator.RandomInt(this.MinStreetNumberSize, this.MaxStreetNumberSize).ToString();

            string streetSuffix = string.Empty;

            if (this.StreetSuffixes != null && this.StreetSuffixes.Count > 0)
            {
                int suffixindex = dataGenerator.RandomInt(1, this.StreetSuffixes.Count - 1);
                streetSuffix = this.StreetSuffixes[suffixindex];
            }

            StringBuilder sb = new StringBuilder();

            sb.Append(streetNumber);
            sb.Append(" ");
            sb.Append(streetName);
            sb.Append(" ");
            sb.Append(streetSuffix);


            return sb.ToString(); ;
        }

        #endregion

    }
}
