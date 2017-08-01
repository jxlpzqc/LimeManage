using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LimeManage.ViewModels
{
    public class ProjectDetailViewModel
    {
       
        public string ID { get; set; }
        public string Name { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string County { get; set; }
        public string PartyA { get; set; }
        public string PartyB { get; set; }
        public string ResponsiblePerson { get; set; }
        public string ContactPhone { get; set; }
        public string Date { get; set; }
        public string Money { get; set; }

    }
}