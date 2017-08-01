using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using FineUIMvc;
using LimeManage.Models;

namespace LimeManage.ViewModels
{
    public class EditProjectViewModel
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public List<MyListItem> ProvinceTable { get; set; }
        public string Province { get; set; }
        public List<MyListItem> CityTable { get; set; }
        public string City { get; set; }
        public List<MyListItem> CountyTable { get; set; }
        public string County { get; set; }
        public string PartyA { get; set; }
        public List<MyListItem> PartyBTable { get; set; }
        public string partyB { get; set; }
        public string ResponsiblePerson { get; set; }
        public string ContactPhone { get; set; }
        public string Date { get; set; }
        public string Money { get; set; }
    }
}