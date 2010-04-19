using System;
using System.Collections.Generic;
using System.Text;

namespace Umbriel.Utility
{
    public static class Constants
    {
        public static string[] USPSStreetSuffixes
        {
            get
            {
                return new string[] {"ALY","ANX","ARC","AVE","BCH","BG","BGS","BLF","BLFS","BLVD","BND","BR","BRG","BRK","BRKS","BTM","BYP","BYU","CIR","CIRS","CLB","CLF","CLFS","CMN","COR","CORS","CP","CPE","CRES","CRK","CRSE","CRST","CSWY","CT","CTR","CTRS","CTS","CURV","CV","CVS","CYN","DL","DM","DR","DRS","DV","EST","ESTS","EXPY","EXT","EXTS","FALL","FLD","FLDS","FLS","FLT","FLTS","FRD","FRDS","FRG","FRGS","FRK","FRKS","FRST","FRY","FT","FWY","GDN","GDNS","GLN","GLNS","GRN","GRNS","GRV","GRVS","GTWY","HBR","HBRS","HL","HLS","HOLW","HTS","HVN","HWY","INLT","IS","ISLE","ISS","JCT","JCTS","KNL","KNLS","KY","KYS","LAND","LCK","LCKS","LDG","LF","LGT","LGTS","LK","LKS","LN","LNDG","LOOP","MALL","MDW","MDWS","MEWS","ML","MLS","MNR","MNRS","MSN","MT","MTN","MTNS","MTWY","NCK","OPAS","ORCH","OVAL","PARK","PASS","PATH","PIKE","PKWY","PL","PLN","PLNS","PLZ","PNE","PNES","PR","PRT","PRTS","PSGE","PT","PTS","RADL","RAMP","RD","RDG","RDGS","RDS","RIV","RNCH","ROW","RPD","RPDS","RST","RTE","RUE","RUN","SHL","SHLS","SHR","SHRS","SKWY","SMT","SPG","SPGS","SPUR","SQ","SQS","ST","STA","STRA","STRM","STS","TER","TPKE","TRAK","TRCE","TRFY","TRL","TRWY","TUNL","UN","UNS","UPAS","VIA","VIS","VL","VLG","VLGS","VLY","VLYS","VW","VWS","WALK","WALL","WAY","WAYS","WL","WLS","XING","XRD"};
            }
        }

        /// <summary>
        /// Gets the length of the min street name.
        /// </summary>
        /// <value>The length of the min street name.</value>
        public static int MinStreetNameLength
        {
            get
            {
                return 3;
            }
        }

        /// <summary>
        /// Gets the length of the max street name.
        /// </summary>
        /// <value>The length of the max street name.</value>
        public static int MaxStreetNameLength
        {
            get
            {
                return 34;
            }
        }

        /// <summary>
        /// Gets the size of the min street number.
        /// </summary>
        /// <value>The size of the min street number.</value>
        public static int MinStreetNumberSize
        {
            get
            {
                return 1;
            }
        }

        /// <summary>
        /// Gets the size of the max street number.
        /// </summary>
        /// <value>The size of the max street number.</value>
        public static int MaxStreetNumberSize
        {
            get
            {
                return 99999;
            }
        }
    }
}
