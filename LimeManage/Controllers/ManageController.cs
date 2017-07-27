using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LimeManage.Models;
using LimeManage.ViewModels;
using System.Data;
using FineUIMvc;

namespace LimeManage.Controllers
{
    public class ManageController : BaseController
    {

        #region 新建项目模块

        public ActionResult NewProject()
        {
            if (Session["username"] == null || Session["username"].Equals("")) { 
                return RedirectToAction("Index", "Session"); 
            }
            
            NewProjectViewModel vm = new NewProjectViewModel();
            
            using (var db = new ProjectManagementEntities())
            {

                vm.ProjectID = getDefaultProjectID().ToString();
                
                //Get Province 
                DataTable dtPro = new DataTable();
                dtPro.Columns.Add(new DataColumn("ID", typeof(string)));
                dtPro.Columns.Add(new DataColumn("Name", typeof(string)));
                var proArr = db.Province;
                var defaultProRow = dtPro.NewRow();
                defaultProRow["ID"] = "-1";
                defaultProRow["Name"] = "请选择省份";
                dtPro.Rows.Add(defaultProRow);
                foreach(var pro in proArr)
                {
                    var dtProRow = dtPro.NewRow();
                    dtProRow["ID"] =  pro.ID;
                    dtProRow["Name"] = pro.Name;
                    dtPro.Rows.Add(dtProRow);
                }
                vm.Province = dtPro;

                //GetPartyB
                DataTable dtPartyB = new DataTable();
                dtPartyB.Columns.Add(new DataColumn("ID", typeof(string)));
                dtPartyB.Columns.Add(new DataColumn("Name", typeof(string)));
                var partyBArr = db.PartyB;
                foreach (var partyB in partyBArr)
                {
                    var dtPartyBRow = dtPartyB.NewRow();
                    dtPartyBRow["ID"] = partyB.ID;
                    dtPartyBRow["Name"] = "(代码:" + partyB.ID + ")  " + partyB.Name;
                    dtPartyB.Rows.Add(dtPartyBRow);
                }
                vm.PartyB = dtPartyB;

             }
            return View(vm);
        }

        [HttpPost]
        public ActionResult DoNewProject(string projectID, string ProjectName,string ddlCounty,
            string tbPartyA, string ddlPartyB, string tbResponsiblePerson,
            string tbContact, string nbMoney,string ddlMoneyUnit,
            string dpDate)
        {
            if (Session["username"] == null || Session["username"].Equals("")) { return RedirectToAction("Index", "Session"); }
            
            Project p = new Project();
            if (String.IsNullOrEmpty(projectID) || String.IsNullOrEmpty(ddlCounty)||
                String.IsNullOrEmpty(tbPartyA) || String.IsNullOrEmpty(ddlPartyB)
                || String.IsNullOrEmpty(nbMoney) || String.IsNullOrEmpty(dpDate)) 
            { 
                return RedirectToAction("LackOfJS", "Home");
            }

            if(!ValidateProjectID(projectID))
            {
                ShowNotify("已经具有相同的项目编号",MessageBoxIcon.Warning);
                var txtProjectID = UIHelper.TextBox("projectID");
                txtProjectID.MarkInvalid(String.Format("已存在{0}这样的项目编号", projectID));
                return UIHelper.Result();
            }

            p.ID = Convert.ToInt32(projectID);
            p.Name = ProjectName;
            p.CountyID = Convert.ToInt32(ddlCounty);
            
            p.PartyAName = tbPartyA;
            p.PartyBID = Convert.ToInt32(ddlPartyB);
            p.ResponsiblePerson = tbResponsiblePerson;
            p.ContactPhone = tbContact;
            
            p.Money = computeMoney(nbMoney,ddlMoneyUnit);
            p.Date = DateTime.ParseExact(dpDate,"yyyy-MM-dd",System.Globalization.CultureInfo.CurrentCulture);

            using (var db = new ProjectManagementEntities())
            {

                db.Project.Add(p);
                try
                {
                    //int de = db.Database.ExecuteSqlCommand("INSERT INTO Project" +
                    //    "values (@p0 , @p1 , @p2 , @p3 , @p4 , @p5 , @p6 , @p7)",
                    //    new String[] { p.ID.ToString() ,p.CountyID.ToString() ,
                    //        p.PartyAName,p.PartyBID.ToString(),p.ResponsiblePerson,
                    //    p.ContactPhone,p.Date.ToString(),p.Money.ToString()});
                    
                    db.SaveChanges();
                    ShowNotify("项目添加成功",MessageBoxIcon.Success);
                    UIHelper.Form("mainForm").Reset();
                    UIHelper.TextBox("projectID").Text(getDefaultProjectID().ToString());
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException)
                {
                    
                    ShowNotify("数据无效，请检查表单！",MessageBoxIcon.Error);
                }
                catch (Exception)
                {
                    ShowNotify("服务器发生了未能处理的异常，请联系技术人员进行检测。",MessageBoxIcon.Error);
                }
            }
            return UIHelper.Result();
        }

        #region 省区市联动模块
        [HttpPost]
        public ActionResult ProvinceChange(string ddlProvince)
        {
            var ddlCity = UIHelper.DropDownList("ddlCity");
            
            List<ListItem> items = new List<ListItem>();
            items.Add(new ListItem("选择地区市", "-1", true));

            

            if (ddlProvince != "-1")
            {
                using (var db = new ProjectManagementEntities()){
                    int ddlProvinceNum = Convert.ToInt32(ddlProvince);
                    var CityArr = db.City.Where(u=>(u.ProvinceID == ddlProvinceNum));
                    foreach (var City in CityArr )
                    {
                        ListItem item = new ListItem();
                        item.Text = City.Name;
                        item.Value = City.ID.ToString();
                        items.Add(item);
                    }
                }
            }
            ddlCity.LoadData(items.ToArray());

            // 是否禁用
            ddlCity.Enabled(!(ddlCity.Source.Items.Count == 1));

            var ddlCounty = UIHelper.DropDownList("ddlCounty");
            List<ListItem> items2 = new List<ListItem>();
            items2.Add(new ListItem("选择县", "-1", true));
            ddlCounty.LoadData(items2.ToArray());
            ddlCounty.Enabled(false);

            return UIHelper.Result();
            
        }

        [HttpPost]
        public ActionResult CityChange(string ddlCity)
        {
            var ddlCounty = UIHelper.DropDownList("ddlCounty");

            List<ListItem> items = new List<ListItem>();
            items.Add(new ListItem("选择县", "-1", true));

            if (ddlCity != "-1")
            {
                using (var db = new ProjectManagementEntities())
                {
                    int ddlCityNum = Convert.ToInt32(ddlCity);
                    var CountyArr = db.County.Where(u => (u.CityID == ddlCityNum));
                    foreach (var county in CountyArr)
                    {
                        ListItem item = new ListItem();
                        item.Text = county.Name;
                        item.Value = county.ID.ToString();
                        items.Add(item);
                    }
                }
            }
            ddlCounty.LoadData(items.ToArray());

            // 是否禁用
            ddlCounty.Enabled(!(ddlCounty.Source.Items.Count == 1));

            return UIHelper.Result();

        }
        #endregion

        [HttpPost]
        public ActionResult BlurValidateID(string projectID)
        {

            var txtProjectID = UIHelper.TextBox("projectID");
            if (!ValidateProjectID(projectID)) 
            {
                txtProjectID.MarkInvalid(String.Format("已存在{0}这样的项目编号", projectID)); 
            }
            return UIHelper.Result();
        }

        private bool ValidateProjectID(string projectID)
        {
            //var txtProjectID = UIHelper.TextBox("projectID");
            using (var db = new ProjectManagementEntities())
            { 
                int projectIDNum = Convert.ToInt32(projectID);
                if (db.Project.Where(u => u.ID == projectIDNum).Count() > 0 ) 
                {
                    //txtProjectID.MarkInvalid(String.Format("已存在{0}这样的项目编号", projectID));
                    return false;
                }
            }

            return true;
        }

        private int getDefaultProjectID()
        {
            using (var db = new ProjectManagementEntities())
            {
                // Search the ID
                int now = Convert.ToInt32(System.DateTime.Now.ToString("yyyyMM"));
                var pid = (from u in db.Project where u.ID / 1000 == now select u.ID);
                //.Equals(System.DateTime.Now.ToString("yyyymm")))).Max(u=>u.ID);
                //if (idres == null) { pid = System.DateTime.Now.ToString("yyyymm") + "0001"; }
                int m;
                if (pid.Count() == 0) { m = now * 1000 + 1; }
                else { m = pid.Max() + 1; }
                return m;
            }
        }

        #endregion

        #region 公共
        private int computeMoney(string value,string unit)
        {
            int money = Convert.ToInt32(value);
            switch (unit)
            {

                case "wan":
                    money = money * 10000;
                    break;
                case "yi":
                    money = money * 100000000;
                    break;

            }
            return money;
        }
        #endregion

        #region 收发票模块
        public ActionResult NewInvoice()
        {
            if (Session["username"] == null || Session["username"].Equals("")) { return RedirectToAction("Index", "Session"); }
            NewInvoiceViewModel vm = new NewInvoiceViewModel();
            using (var db = new ProjectManagementEntities())
            {

                
                //Get Project 
                DataTable dtPro = new DataTable();
                dtPro.Columns.Add(new DataColumn("ID", typeof(string)));
                dtPro.Columns.Add(new DataColumn("Name", typeof(string)));
                var proArr = db.Project;
                foreach (var pro in proArr)
                {
                    var dtProRow = dtPro.NewRow();
                    dtProRow["ID"] = pro.ID;
                    dtProRow["Name"] = String.Format("(代码:{0}) {1}",pro.ID , pro.Name);
                    dtPro.Rows.Add(dtProRow);
                }
                vm.Project = dtPro;

                //GetPartyB
                DataTable dtPartyB = new DataTable();
                dtPartyB.Columns.Add(new DataColumn("ID", typeof(string)));
                dtPartyB.Columns.Add(new DataColumn("Name", typeof(string)));
                var partyBArr = db.PartyB;
                foreach (var partyB in partyBArr)
                {
                    var dtPartyBRow = dtPartyB.NewRow();
                    dtPartyBRow["ID"] = partyB.ID;
                    dtPartyBRow["Name"] = "(代码:" + partyB.ID + ")  " + partyB.Name;
                    dtPartyB.Rows.Add(dtPartyBRow);
                }
                vm.PartyB = dtPartyB;

            }

            return View(vm);

        }

        public ActionResult DoNewInvoice(string tbID, string ddlProject, string ddlPartyB,
            string dpDate, string nbMoney, string ddlMoneyUnit, string ddlType,
            string nbTaxPayed, string ddlTaxUnit, string ddlPayMethod
            )
        {
            if (Session["username"] == null || Session["username"].Equals("")) { return RedirectToAction("Index", "Session"); }
            if (!ValidateInvoiceID(tbID))
            {
                ShowNotify(string.Format("存在{0}这样的发票编号",tbID),MessageBoxIcon.Warning);
                UIHelper.TextBox("tbID").MarkInvalid("已存在相同的发票编号");
                return UIHelper.Result();
            }
            
            Invoice inv = new Invoice();
            inv.ID = tbID;
            inv.ProjectID = Convert.ToInt32(ddlProject);
            inv.PartyBID = Convert.ToInt32(ddlPartyB);
            inv.Date = DateTime.ParseExact(dpDate, "yyyy-MM-dd", System.Globalization.CultureInfo.CurrentCulture);
            inv.Money = computeMoney(nbMoney, ddlMoneyUnit);
            inv.Type = ddlType;
            inv.TaxPayed = computeMoney(nbTaxPayed, ddlTaxUnit);
            inv.PayMethod = ddlPayMethod;

            using(var db = new ProjectManagementEntities())
            {
                db.Invoice.Add(inv);

                try
                {
                    db.SaveChanges();
                    ShowNotify("发票添加成功", MessageBoxIcon.Success);
                    UIHelper.Form("mainForm").Reset();
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException)
                {

                    ShowNotify("数据无效，请检查表单！", MessageBoxIcon.Error);
                }
                catch (Exception)
                {
                    ShowNotify("服务器发生了未能处理的异常，请联系技术人员进行检测。", MessageBoxIcon.Error);
                }
            }

            return UIHelper.Result();
        }
        
        [HttpPost]
        public ActionResult BlurValidateInvoiceID(string ID)
        {
            if(!ValidateInvoiceID(ID))
            {
                UIHelper.TextBox("tbID").MarkInvalid("已存在相同的发票编号");

            }
            return UIHelper.Result();
            
        }

        private bool ValidateInvoiceID(string ID)
        {
            using (var db = new ProjectManagementEntities())
            {
                var projectIDNum = ID;
                if (db.Invoice.Where(u => u.ID.Equals(projectIDNum) ).Count() > 0)
                {
                    //txtProjectID.MarkInvalid(String.Format("已存在{0}这样的项目编号", projectID));
                    return false;
                }
            }
            return true;
        }


        #endregion

    }
}