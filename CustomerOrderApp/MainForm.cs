using System;
using System.Linq;
using System.Windows.Forms;
using System.Data.Entity;
using System.Diagnostics;

namespace CustomerOrderApp
{
    public partial class MainForm : Form
    {
        CustomerContext _context;
        public MainForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            _context = new CustomerContext();

            _context.Customer.Load();
            _context.Order.Load();
            this.customerBindingSource.DataSource = _context.Customer.Local.ToBindingList();
            this.orderBindingSource.DataSource = _context.Order.Local.ToBindingList();
        }

        private void customerBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            _context.SaveChanges();
        }

        private void customerBindingSource_CurrentChanged(object sender, EventArgs e)
        {
            FilterDataInOrderGrid();
        }

        private void FilterDataInOrderGrid()
        {

            int customerId = 0;
            if (customerBindingSource.Current != null)
                customerId = ((Customer)customerBindingSource.Current).CustomerId;
            // Display filtered data in order grid
            if (customerId != 0)
                this.orderBindingSource.DataSource = _context.Order.Local.Where(x => x.CustomerId == customerId).ToList();
            else
                this.orderBindingSource.DataSource = _context.Order.Local.ToBindingList();
        }

        //private void customerDataGridView_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e)
        //{
        //    // For any other operation except, StateChanged, do nothing
        //    if (e.StateChanged != DataGridViewElementStates.Selected) return;

        //    //int customerId = ((Customer)customerBindingSource.Current).CustomerId;
        //    int customerId = 0;
        //    if (e.Row.Cells["CustomerId"].Value != null &&
        //        e.Row.Cells["CustomerId"].Value != DBNull.Value)
        //        customerId = (int)e.Row.Cells["CustomerId"].Value;
        //    else
        //        return;

        //    // Display filtered data in order grid
        //    if (customerId != null)
        //        this.orderBindingSource.DataSource = _context.Order.Local.Where(x => x.CustomerId == customerId);
        //}

        private void customerBindingNavigatorDeleteItem_Click(object sender, EventArgs e)
        {
            Customer currentCustomer = (Customer)this.customerBindingSource.Current;
            if (currentCustomer != null)
            { 
                if (currentCustomer.Order.Count > 0)
                {
                    // warning with cascade deleted order amount
                    string message = "In case of deleting '" + currentCustomer.Name.ToString() +
                                     "' (id=" + currentCustomer.CustomerId.ToString() + "), " +
                                     currentCustomer.Order.Count.ToString() + " order(s) will be deleted." +
                                     "Do you want to continue?";
                    DialogResult dialogResult = MessageBox.Show(message, "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (dialogResult == DialogResult.Yes)
                    {
                        this.customerBindingSource.RemoveCurrent();
                    }
                }
                else
                    this.customerBindingSource.RemoveCurrent();
                //// show all (unfiltered) orders after deleting one
                //this.orderBindingSource.DataSource = _context.Order.Local.ToBindingList();
                FilterDataInOrderGrid();
            }
        }

        private void orderBindingNavigatorDeleteItem_Click(object sender, EventArgs e)
        {
            Order currentOrder = (Order)orderBindingSource.Current;
            if (currentOrder != null)
            {
                orderBindingSource.RemoveCurrent();
                _context.Order.Remove(currentOrder);
                //orderDataGridView.Refresh();
            }           
        }

        private void bindingNavigatorAddNewItem_Click(object sender, EventArgs e)
        {
            CustomerDetailsForm addNewCustomerDialog = new CustomerDetailsForm();

            addNewCustomerDialog.Owner = this;
            addNewCustomerDialog.ShowDialog();
            if (addNewCustomerDialog.IsSaved)
            {
                Customer newCustomer = addNewCustomerDialog.Customer;
                _context.Customer.Add(newCustomer);
                try
                {
                    _context.SaveChanges();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("***************" + ex);
                }
            }
        }

        private void customerDataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            Customer currentCustomer = (Customer)customerBindingSource.Current;
            CustomerDetailsForm editCustomerDialog = new CustomerDetailsForm(currentCustomer);

            editCustomerDialog.Owner = this;
            editCustomerDialog.ShowDialog();
            if (editCustomerDialog.IsSaved)
            {
                Customer editedCustomer = editCustomerDialog.Customer;
                _context.Entry(editedCustomer).State = EntityState.Modified;
                try
                {
                    _context.SaveChanges();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("***************" + ex);
                    return;
                }
                this.customerDataGridView.Refresh();
            }
        }

        private void bindingNavigatorAddNewItem1_Click(object sender, EventArgs e)
        {
            Customer selectedCustomer = (Customer)customerBindingSource.Current;
            OrderDetailsForm addNewOrderDialog = new OrderDetailsForm(selectedCustomer, _context);

            addNewOrderDialog.Owner = this;
            addNewOrderDialog.ShowDialog();
            if (addNewOrderDialog.IsSaved)
            {
                FilterDataInOrderGrid();
                orderDataGridView.Refresh();
            }
        }

        private void orderDataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            Order selectedOrder = (Order)orderBindingSource.Current;
            //Customer selectedCustomer = (Customer)_context.Customer.First(x => x.CustomerId == selectedOrder.CustomerId);
            Customer selectedCustomer = selectedOrder.Customer;
            OrderDetailsForm editOrderDialog = new OrderDetailsForm(selectedCustomer, _context, selectedOrder);
            editOrderDialog.Owner = this;
            editOrderDialog.ShowDialog();
            if (editOrderDialog.IsSaved)
            {
                orderDataGridView.Refresh();
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _context.SaveChanges();
        }
    }
}
