using System;
using System.Collections.Generic;
using System.Text;

namespace Smart.Data.Abstractions.Models
{
    public class DataSourceInput
    {
        public string DSRC_COD { get; set; }
        public Guid DSRC_ID { get; set; }
        public DataSourceType DSRC_TYP_SRC { get; set; }
        public string DSRC_SRC { get; set; }
        public string DSRC_TIT { get; set; }
        public string DSRC_QRY_PARAMS { get; set; }
        public string DSRC_JSO_FIELDS { get; set; }
        public string DSRC_COD_RLS { get; set; }
        public string DSRC_ORDER { get; set; }

    }
}
