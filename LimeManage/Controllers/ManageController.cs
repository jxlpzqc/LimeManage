using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LimeManage.Models;
using LimeManage.ViewModels;
using System.Data;
using FineUIMvc;
using Newtonsoft.Json.Linq;
using System.Text;

namespace LimeManage.Controllers
{
    public class ManageController : BaseController
    {
        #region 业务操作

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
            if (String.IsNullOrEmpty(projectID))
            {
                txtProjectID.MarkInvalid("编号不能为空");
            }
            else if (!ValidateProjectID(projectID)) 
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
            if (String.IsNullOrEmpty(ID))
            {
                UIHelper.TextBox("tbID").MarkInvalid("编号不能为空");
            }
            else if(!ValidateInvoiceID(ID))
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

        #region 开支出模块
        public ActionResult NewCost()
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
                    dtProRow["Name"] = String.Format("(代码:{0}) {1}", pro.ID, pro.Name);
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

        public ActionResult doNewCost(string tbID, string ddlProject, string ddlPartyB,
            string dpDate, string nbMoney, string ddlMoneyUnit)
        {
            if (Session["username"] == null || Session["username"].Equals("")) { return RedirectToAction("Index", "Session"); }
            if (!ValidateCostID(tbID))
            {
                ShowNotify(string.Format("存在{0}这样的支出编号", tbID), MessageBoxIcon.Warning);
                UIHelper.TextBox("tbID").MarkInvalid("已存在相同的支出编号");
                return UIHelper.Result();
            }
            Cost cos = new Cost();
            cos.ID = Convert.ToInt32(tbID);
            cos.ProjectID = Convert.ToInt32(ddlProject);
            cos.PartyBID = Convert.ToInt32(ddlPartyB);
            cos.Money = computeMoney(nbMoney, ddlMoneyUnit);
            cos.Date = parseDate(dpDate);

            using (var db = new ProjectManagementEntities())
            {
                db.Cost.Add(cos);
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
        public ActionResult BlurValidateCostID(string ID)
        {
            if(String.IsNullOrEmpty(ID))
            {
                UIHelper.TextBox("tbID").MarkInvalid("编号不能为空");
            }
            else if (!ValidateCostID(ID))
            {
                UIHelper.TextBox("tbID").MarkInvalid("已存在相同的支出编号");

            }
            return UIHelper.Result();

        }

        private bool ValidateCostID(string ID)
        {
            using (var db = new ProjectManagementEntities())
            {
                var projectIDNum = Convert.ToInt32(ID);
                if (db.Cost.Where(u => u.ID == (projectIDNum)).Count() > 0)
                {
                    //txtProjectID.MarkInvalid(String.Format("已存在{0}这样的项目编号", projectID));
                    return false;
                }
            }
            return true;
        }

        #endregion

        #region 更改项目模块
        
        public ActionResult EditProject(string id)
        {
            if (Session["username"] == null || Session["username"].Equals("")) { return RedirectToAction("Index", "Session"); }

            var vm = new EditProjectViewModel() { ID = id };

            //UIHelper.DropDownList("ddlProvince")

            using (var db = new ProjectManagementEntities())
            {
                var item = db.Project.Where(u => u.ID.ToString().Equals(id.Trim())).FirstOrDefault();
                if(item != null)
                {
                    vm.Name = item.Name;
                    vm.PartyA = item.PartyAName;
                    vm.ResponsiblePerson = item.ResponsiblePerson;
                    vm.ContactPhone = item.ContactPhone;
                    vm.Date = item.Date.ToString("yyyy-MM-dd");
                    vm.Money = item.Money.ToString();
                }
                else
                {
                    vm.ID = "不合法的查询信息";
                    return View(vm);
                }
             
                

                //Get Province 
                List<MyListItem> dtPro = new List<MyListItem>();
                dtPro.Add(new MyListItem("请选择省份","-1"));
                var proArr = db.Province;
                foreach (var pro in proArr)
                {
                    
                    dtPro.Add(new MyListItem(pro.Name, pro.ID.ToString()));
                }
                vm.ProvinceTable = dtPro;
                vm.Province = item.County.City.ProvinceID.ToString();
                //Get City
                List<MyListItem> dtCity = new List<MyListItem>();
                dtCity.Add(new MyListItem("请选择市", "-1"));
                var cityArr = db.City.Where(u => u.ProvinceID == item.County.City.ProvinceID);
                foreach (var city in cityArr)
                {
                    
                    dtCity.Add(new MyListItem(city.Name, city.ID.ToString()));
                }
                vm.CityTable = dtCity;
                vm.City = item.County.CityID.ToString();
                //Get County
                List<MyListItem> dtCounty = new List<MyListItem>();
                dtCounty.Add(new MyListItem("请选择县区", "-1"));
                var countyArr = db.County.Where(u => u.CityID == item.County.CityID);
                foreach (var county in countyArr)
                {
                    
                    dtCounty.Add(new MyListItem(county.Name, county.ID.ToString()));
                }
                vm.CountyTable = dtCounty;
                vm.County = item.CountyID.ToString();
                //Get PartyB
                List<MyListItem> dtPartyB = new List<MyListItem>();
                var partyBArr = db.PartyB;
                foreach (var partyB in partyBArr)
                {

                    dtPartyB.Add(new MyListItem(partyB.Name, partyB.ID.ToString()));
                }
                vm.PartyBTable = dtPartyB;
                vm.partyB = item.PartyBID.ToString();


            }
            return View(vm);
        }

        [HttpPost]
        public ActionResult DoEditProject(string projectID, string ProjectName, string ddlCounty,
            string tbPartyA, string ddlPartyB, string tbResponsiblePerson,
            string tbContact, string nbMoney, string ddlMoneyUnit,
            string dpDate)
        {
            if (Session["username"] == null || Session["username"].Equals("")) { return RedirectToAction("Index", "Session"); }

            
            if (String.IsNullOrEmpty(projectID) || String.IsNullOrEmpty(ddlCounty) ||
                String.IsNullOrEmpty(tbPartyA) || String.IsNullOrEmpty(ddlPartyB)
                || String.IsNullOrEmpty(nbMoney) || String.IsNullOrEmpty(dpDate))
            {
                return RedirectToAction("LackOfJS", "Home");
            }

           
            using (var db = new ProjectManagementEntities())
            {
                int pid = Convert.ToInt32(projectID);
                var p = db.Project.Where(u => u.ID == pid).FirstOrDefault();
                if(p != null)
                {
                    
                    p.Name = ProjectName;
                    p.CountyID = Convert.ToInt32(ddlCounty);

                    p.PartyAName = tbPartyA;
                    p.PartyBID = Convert.ToInt32(ddlPartyB);
                    p.ResponsiblePerson = tbResponsiblePerson;
                    p.ContactPhone = tbContact;

                    p.Money = computeMoney(nbMoney, ddlMoneyUnit);
                    p.Date = DateTime.ParseExact(dpDate, "yyyy-MM-dd", System.Globalization.CultureInfo.CurrentCulture);
                }
                try
                {
                    var rowCount = db.SaveChanges();
                    if (rowCount > 0)
                    {
                        ShowNotify("项目修改成功", MessageBoxIcon.Success);
                    }
                    else
                    {
                        ShowNotify("项目未修改", MessageBoxIcon.Information);
                    }
                    ActiveWindow.Hide();
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

        #endregion

        #endregion


        #region 公共

        private int pageCount = 5;

        

        private int computeMoney(string value, string unit)
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

        private DateTime parseDate(string date)
        {
            return DateTime.ParseExact(date, "yyyy-MM-dd", System.Globalization.CultureInfo.CurrentCulture);
        }

        #endregion


        #region 查询功能

        #region 查项目
        public ActionResult SearchProject()
        {
            if (Session["username"] == null || Session["username"].Equals("")) { return RedirectToAction("Index", "Session"); }

            SearchProjectViewModel vm = new SearchProjectViewModel();
            vm.Data = new DataTable();
            vm.Data.Columns.Add(new DataColumn("ID",typeof(string)));
            vm.Data.Columns.Add(new DataColumn("Name", typeof(string)));
            vm.Data.Columns.Add(new DataColumn("Area", typeof(string)));
            vm.Data.Columns.Add(new DataColumn("PartyA", typeof(string)));
            vm.Data.Columns.Add(new DataColumn("PartyB", typeof(string)));
            vm.Data.Columns.Add(new DataColumn("Money", typeof(string)));
            vm.Data.Columns.Add(new DataColumn("Date", typeof(string)));
            using (var db= new ProjectManagementEntities())
            {
                vm.RecordCount = db.Project.Count();
                foreach(var item in db.Project.OrderBy(u => u.ID).Take(pageCount))
                {
                    var newRow = vm.Data.NewRow();
                    newRow["ID"] = item.ID;
                    newRow["Name"] = item.Name;
                    newRow["Area"] = String.Format("{0} - {1} - {2}",item.County.City.Province.Name ,item.County.City.Name ,item.County.Name);
                    newRow["PartyA"] = item.PartyAName;
                    newRow["PartyB"] = item.PartyB.Name;
                    newRow["Money"] = item.Money.ToString("C");
                    newRow["Date"] = item.Date.ToString("yyyy-MM-dd");
                    vm.Data.Rows.Add(newRow);
                }
            }


            return View(vm);
        }

        [HttpPost]
        public ActionResult SearchProjectGridIndexChangedOrSort(JArray mainGrid_fields, int mainGrid_pageIndex, string mainGrid_sortField, string mainGrid_sortDirection)
        {
            if (Session["username"] == null || Session["username"].Equals("")) { return RedirectToAction("Index", "Session"); }

            var mainGrid = UIHelper.Grid("mainGrid");

            //ShowNotify(mainGrid_sortField + mainGrid_sortDirection);

            // 1.设置总项数（数据库分页回发时，如果总记录数不变，可以不设置RecordCount）
            //mainGrid.RecordCount(recordCount);

            // 2.获取当前分页数据
            //var dataSource = DataSourceUtil.GetPagedDataTable(pageIndex: Grid1_pageIndex, pageSize: 5, recordCount: recordCount);

            var pagingTable = new DataTable();
            pagingTable.Columns.Add(new DataColumn("ID", typeof(string)));
            pagingTable.Columns.Add(new DataColumn("Name", typeof(string)));
            pagingTable.Columns.Add(new DataColumn("Area", typeof(string)));
            pagingTable.Columns.Add(new DataColumn("PartyA", typeof(string)));
            pagingTable.Columns.Add(new DataColumn("PartyB", typeof(string)));
            pagingTable.Columns.Add(new DataColumn("Money", typeof(string)));
            pagingTable.Columns.Add(new DataColumn("Date", typeof(string)));

            //get sort field
            


            using (var db = new ProjectManagementEntities())
            {

                IQueryable<Project> result = null;

                bool isASC = mainGrid_sortDirection.Equals("ASC");

                switch(mainGrid_sortField)
                {
                    case "ID":
                        result = isASC ? db.Project.OrderBy(u => u.ID).Skip(pageCount * mainGrid_pageIndex).Take(pageCount)
                            : db.Project.OrderByDescending(u => u.ID).Skip(pageCount * mainGrid_pageIndex).Take(pageCount);
                        break;
                    case "Area":
                        result = isASC ? db.Project.OrderBy(u => u.County.Name).Skip(pageCount * mainGrid_pageIndex).Take(pageCount)
                            : db.Project.OrderByDescending(u => u.County.Name).Skip(pageCount * mainGrid_pageIndex).Take(pageCount);
                        break;
                    case "Money":
                        result = isASC ? db.Project.OrderBy(u => u.Money).Skip(pageCount * mainGrid_pageIndex).Take(pageCount)
                            : db.Project.OrderByDescending(u => u.Money).Skip(pageCount * mainGrid_pageIndex).Take(pageCount);
                        break;
                    case "Date":
                        result = isASC ? db.Project.OrderBy(u => u.Date).Skip(pageCount * mainGrid_pageIndex).Take(pageCount)
                            : db.Project.OrderByDescending(u => u.Date).Skip(pageCount * mainGrid_pageIndex).Take(pageCount);
                        break;

                    default:
                        result = db.Project.OrderBy(u => u.ID).Skip(pageCount * mainGrid_pageIndex).Take(pageCount);
                        break;

                }


                foreach (var item in result)
                {
                    var newRow = pagingTable.NewRow();
                    newRow["ID"] = item.ID;
                    newRow["Name"] = item.Name;
                    newRow["Area"] = String.Format("{0} - {1} - {2}", item.County.City.Province.Name, item.County.City.Name, item.County.Name);
                    newRow["PartyA"] = item.PartyAName;
                    newRow["PartyB"] = item.PartyB.Name;
                    newRow["Money"] = item.Money.ToString("C");
                    newRow["Date"] = item.Date.ToString("yyyy-MM-dd");
                    pagingTable.Rows.Add(newRow);
                }
            }
            
            
            mainGrid.DataSource(pagingTable, mainGrid_fields);

            return UIHelper.Result();
        }

        [HttpPost]
        public ActionResult DeleteProject(string ID)
        {
            if (Session["username"] == null || Session["username"].Equals("")) { return RedirectToAction("Index", "Session"); }

            using (var db = new ProjectManagementEntities())
            {
                Project deleteItem = new Project { ID = Convert.ToInt32(ID) };
                db.Project.Attach(deleteItem);
                db.Project.Remove(deleteItem);
                int result;
                try
                {
                   result = db.SaveChanges();
                }
                catch (Exception)
                {
                    ShowNotify("服务器出现异常，删除失败",MessageBoxIcon.Error);
                    return UIHelper.Result();
                }
                
                if(result > 0)
                {
                    ShowNotify("成功删除", MessageBoxIcon.Success);
                    PageContext.Redirect(Url.Action("SearchProject"));
                }
                else
                {
                    ShowNotify("删除失败", MessageBoxIcon.Error);
                }
                return UIHelper.Result();
            }
        }

        /// <summary>
        /// 将所有项目导出为excel
        /// </summary>
        /// <returns></returns>
        public ActionResult ProjectExportToXls()
        {
            if (Session["username"] == null || Session["username"].Equals("")) { return RedirectToAction("Index", "Session"); }

            string TH_HTML = "<th>{0}</th>";
            string TD_HTML = "<td>{0}</td>";
            string TD_IMAGE_HTML = "<td><img src=\"{0}\"></td>";

            StringBuilder sb = new StringBuilder();
            sb.Append("<table cellspacing=\"0\" rules=\"all\" border=\"1\" style=\"border-collapse:collapse;\">");
            sb.Append("<tr colspan='7' style='text-align:center;font-size:2em'><b>项目表</b></tr>");
            sb.AppendFormat("<tr colspan='7'>由Lime项目管理系统在{0}生成。</tr>",DateTime.Now.ToString("F"));
            sb.Append("<tr>");
            sb.AppendFormat(TH_HTML, "项目编号");
            sb.AppendFormat(TH_HTML, "项目名称");
            sb.AppendFormat(TH_HTML, "项目地区");
            sb.AppendFormat(TH_HTML, "甲方名称");
            sb.AppendFormat(TH_HTML, "乙方名称");
            sb.AppendFormat(TH_HTML, "金额");
            sb.AppendFormat(TH_HTML, "添加日期");
            sb.Append("</tr>");


            using (var db = new ProjectManagementEntities())
            {
                foreach (var item in db.Project.OrderBy(u => u.ID))
                {
                    sb.Append("<tr>");
                    sb.AppendFormat(TD_HTML, item.ID);
                    sb.AppendFormat(TD_HTML, item.Name);
                    sb.AppendFormat(TD_HTML, String.Format("{0} - {1} - {2}",item.County.City.Province.Name ,item.County.City.Name ,item.County.Name));
                    sb.AppendFormat(TD_HTML, item.PartyAName);
                    sb.AppendFormat(TD_HTML, item.PartyB.Name);
                    sb.AppendFormat(TD_HTML, item.Money.ToString("C"));
                    sb.AppendFormat(TD_HTML, (item.Date).ToString("yyyy-MM-dd"));
                    sb.Append("</tr>");
                }

            }
            sb.Append("</table>");


            return File(Encoding.UTF8.GetBytes(sb.ToString()), "application/excel", String.Format("项目表{0}导出.xls",DateTime.Now.ToString("yyyyMMddHHmmss")));
        }

        #endregion

        #region 项目详情
        
        public ActionResult ProjectDetail(string ID)
        {
            if (Session["username"] == null || Session["username"].Equals("")) { return RedirectToAction("Index", "Session"); }

            var vm = new ProjectDetailViewModel();
            using (var db = new ProjectManagementEntities())
            {
                var result = db.Project.Where(u => u.ID.ToString().Equals(ID.Trim()));
                if (result.Count() > 0)
                {
                    var item = result.FirstOrDefault();
                    vm.ID = item.ID.ToString();
                    vm.Name = item.Name;
                    vm.Province = item.County.City.Province.Name;
                    vm.City = item.County.City.Name; 
                    vm.County = item.County.Name;
                    vm.PartyA = item.PartyAName;
                    vm.PartyB = item.PartyB.Name;
                    vm.ResponsiblePerson = item.ResponsiblePerson;
                    vm.ContactPhone = item.ContactPhone;
                    vm.Money = item.Money.ToString("C");
                    vm.Date = item.Date.ToString("yyyy-MM-dd");
                }
                else
                {
                    vm.ID = "错误的查询信息";
                }
                
            }
            return View(vm);
        }
        #endregion

        #endregion
    }

    

}