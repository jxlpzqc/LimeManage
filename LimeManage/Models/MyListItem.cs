using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LimeManage.Models
{
    public class MyListItem
    {
        public string ID { get; set; }
        public string Name { get; set; }
        
        public MyListItem(string Name, string ID)
        { this.ID = ID; this.Name = Name;  }
        

    }
}