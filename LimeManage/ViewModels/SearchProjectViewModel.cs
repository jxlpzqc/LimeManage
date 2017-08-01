using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace LimeManage.ViewModels
{
    public class SearchProjectViewModel
    {
        public DataTable Data { get; set; }
        public int RecordCount { get; set; }
    }
}