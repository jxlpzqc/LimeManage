using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace LimeManage.ViewModels
{
    public class NewProjectViewModel
    {
        public string ProjectID { get; set; }
        public DataTable Province { get; set; }
        public DataTable PartyB { get; set; }
    }
}