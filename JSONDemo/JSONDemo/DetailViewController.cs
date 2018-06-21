using Foundation;
using System;
using UIKit;
using SQLite;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace JSONDemo
{
    public partial class DetailViewController : UIViewController
    {
        int selected;
        UIWindow window;
        UITableView tblEmpList;
        List<Employee> employeeList;
        protected string cellIdentifier = "TableCell";
        Employee emp = new Employee();
        UITableViewController table = new UITableViewController();

        public DetailViewController (IntPtr handle) : base (handle)
        {
        }

        /// <summary>
        /// Testing git command from terminal
        /// 
        /// Views the did load.
        /// </summary>
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            tblEmpList = new UITableView();
            tblEmpList.Frame = new CoreGraphics.CGRect(x: 5, y: 5, width: 502, height: 598);
            tblEmpList.backgroundcolor = UIColor.LightGrey;
           
            tblEmpList.DataSource = table;
            tblEmpList.Delegate = table;

            this.View.AddSubview(tblEmpList);
            GetEmplyees();
        }

        public async void GetEmplyees()
        {
            RestApi restapi = new RestApi();
            employeeList = await restapi.GetEmployeeList();
            tblEmpList.WeakDelegate = this;
            tblEmpList.WeakDataSource = this;
            tblEmpList.ReloadData();
        }


        [Export("tableView:numberOfRowsInSection:")]
        public nint RowsInSection(UITableView tableView, nint section)
        {
            return employeeList == null ? 0 : employeeList.Count;
        }

        [Export("tableView:cellForRowAtIndexPath:")]
        public UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            Contract.Ensures(Contract.Result<UITableViewCell>() != null);
            // request a recycled cell to save memory
            UITableViewCell cell = tableView.DequeueReusableCell(cellIdentifier);
            // if there are no cells to reuse, create a new one
            if (cell == null)
            {
                cell = new UITableViewCell(UITableViewCellStyle.Default, cellIdentifier);
            }

            cell.TextLabel.Text = "ID: " + Convert.ToString(employeeList[indexPath.Row].Id) + " " + " " + " " + "Name: " + employeeList[indexPath.Row].Name;;

            return cell;
        }

        [Export("tableView:didSelectRowAtIndexPath:")]
        public virtual void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            
           // tableView.CellAt(NSIndexPath.FromRowSection(selected, 0)).Accessory = UITableViewCellAccessory.None;
            selected = indexPath.Row;
            tableView.DeselectRow(indexPath, true);

            GetEmpDetails(selected);

        }

        public async void GetEmpDetails(int index)
        {
            RestApi restapi = new RestApi();
            emp = new Employee();
            employeeList = await restapi.GetEmployeeList();

            emp = employeeList[index];

            if (emp != null)
            {
                EmpDetailViewController dvc = null;
                UIViewController[] vcArray = NavigationController.ViewControllers;
                bool found = false;
                if (vcArray != null)
                {
                    foreach (UIViewController vc in vcArray)
                    {
                        if (vc.GetType() == typeof(EmpDetailViewController))
                        {
                            dvc = vc as EmpDetailViewController;
                            found = true;
                            break;
                        }
                    }
                }

                if (found)
                {
                    dvc.employeedetail = emp;
                    NavigationController.PushViewController(dvc, true);
                }
                else
                {
                    //dvc = new EmpDetailViewController();

                    UIStoryboard mainStoryboarad = UIStoryboard.FromName("Main", null);
                    dvc = mainStoryboarad.InstantiateViewController("EmpDetailViewController") as EmpDetailViewController;
                    dvc.employeedetail = emp;
                    NavigationController.PushViewController(dvc, true);
                }
            }
        }
    }
}