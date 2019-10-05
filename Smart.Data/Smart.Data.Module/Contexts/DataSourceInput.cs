﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Smart.Data.Module.Contexts
{
    public class DataSourceInput
    {
        public string DSRC_COD { get; set; }
        public Guid DSRC_ID { get; set; }
        public DataSourceType DSRC_TYP_SRC { get; set; }
        public string DSRC_SRC { get; set; }
        public string DSRC_TIT { get; set; }
        public string DSRC_QRY_PARAMS { get; set; }

    }
}
