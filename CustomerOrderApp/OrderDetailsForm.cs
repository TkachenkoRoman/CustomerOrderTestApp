using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using System.Data.Entity;

namespace CustomerOrderApp
{
    public partial class OrderDetailsForm : Form
    {
        private CustomerContext _context;
        private Order _order;
        public bool IsSaved { get; set; }
        private bool _updateMode;
        public OrderDetailsForm(Customer selectedCustomer, CustomerContext context, Order editOrder = null)
        {
            _context = context;
            InitializeComponent();
            this.comboBoxClient.DataSource = _context.Customer.Local.ToBindingList();
            if (editOrder != null) // edit order
            {
                _order = editOrder;
                _updateMode = true;
                SetInitialData(editOrder); 
            }
            else // add new order
            {
                _order = new Order();
                _updateMode = false;
                if (selectedCustomer != null)
                    this.comboBoxClient.SelectedValue = selectedCustomer.CustomerId;
            }
            this.Shown += OrderDetailsForm_Shown;
        }

        void OrderDetailsForm_Shown(object sender, EventArgs e)
        {
            textBoxNumber.Enabled = true;
            textBoxNumber.Focus();
        }


        private void SetInitialData(Order editOrder)
        {
            this.comboBoxClient.SelectedValue = editOrder.CustomerId;
            this.textBoxNumber.Text = editOrder.Number;
            this.textBoxAmount.Text = editOrder.Amount.ToString();
            this.textBoxDescription.Text = editOrder.Description;
            if (editOrder.DueTime != null)
            {
                this.dateTimePickerDueTime.Value = (DateTime)editOrder.DueTime;
                this.dateTimePickerDueTime.Checked = true;
            }
            else
            {
                this.dateTimePickerDueTime.Checked = false;
            }
            if (editOrder.ProcessedTime != null)
            {
                this.dateTimePickerProcessedTime.Value = (DateTime)editOrder.ProcessedTime;
                this.dateTimePickerProcessedTime.Checked = true;
            }
            else
            {
                this.dateTimePickerProcessedTime.Checked = false;
            }
        }

        private void buttonAddNewClient_Click(object sender, EventArgs e)
        {
            AddNewClientDialog();
        }

        private void AddNewClientDialog()
        {
            CustomerDetailsForm addNewCustomerDialog = new CustomerDetailsForm();

            addNewCustomerDialog.Owner = this;
            if (addNewCustomerDialog.ShowDialog() == DialogResult.OK)
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
                    return;
                }
                this.comboBoxClient.SelectedValue = newCustomer.CustomerId;
            }
        }

        private void buttonSaveOrder_Click(object sender, EventArgs e)
        {
            IsSaved = true;
            textBoxNumber_Validating(textBoxNumber, new CancelEventArgs(false));
            if (!IsSaved) return;
            textBoxAmount_Validating(textBoxAmount, new CancelEventArgs(false));
            if (!IsSaved) return;
            comboBoxClient_Validating(comboBoxClient, new CancelEventArgs(false));
            if (!IsSaved) return;
            textBoxDescription_Validating(textBoxDescription, new CancelEventArgs(false));
            if (!IsSaved) return;

            _order.CustomerId = (int)this.comboBoxClient.SelectedValue;
            _order.Number = this.textBoxNumber.Text;
            _order.Amount = Convert.ToInt32(this.textBoxAmount.Text);

            if (this.dateTimePickerDueTime.Checked)
                _order.DueTime = this.dateTimePickerDueTime.Value;
            else
                _order.DueTime = null;

            if (this.dateTimePickerProcessedTime.Checked)
                _order.ProcessedTime = this.dateTimePickerProcessedTime.Value;
            else
                _order.ProcessedTime = null;

            _order.Description = this.textBoxDescription.Text;

            if (_updateMode)
            {
                _context.Entry(_order).State = EntityState.Modified;
            }
            else
            {
                _context.Order.Add(_order);
            }
            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("***************" + ex);
            }

            this.Close();
        }

        private void textBoxNumber_Validating(object sender, CancelEventArgs e)
        {
            string error = null;
            int numberLength = textBoxNumber.Text.Length;
            if (numberLength == 0)
            {
                error = "Please enter number";
                e.Cancel = true;
            }
            else
            {
                if (numberLength > 50)
                {
                    error = "Number is too long";
                    e.Cancel = true;
                }
            }
            if (error != null) IsSaved = false;
            errorProviderOrder.SetError((Control)sender, error);
        }

        private void textBoxAmount_Validating(object sender, CancelEventArgs e)
        {
            string error = null;
            if (textBoxAmount.Text.Length == 0)
            {
                error = "Please enter amount";
                e.Cancel = true;
            }
            else
            {
                try
                {
                    int temp = int.Parse(textBoxAmount.Text);
                    if (temp <= 0)
                    {
                        error = "Please enter positive number";
                        e.Cancel = true;
                    }
                }
                catch
                {
                    error = "Please enter integer number";
                    e.Cancel = true;
                }
            }
            if (error != null) IsSaved = false;
            errorProviderOrder.SetError((Control)sender, error);
        }

        private void comboBoxClient_Validating(object sender, CancelEventArgs e)
        {
            string error = null;

            if (comboBoxClient.SelectedValue == null)
            {
                error = "Please choose customer or add new";
                e.Cancel = true;
            }
            if (error != null) IsSaved = false;
            errorProviderOrder.SetError((Control)sender, error);
        }

        private void textBoxDescription_Validating(object sender, CancelEventArgs e)
        {
            string error = null;
            int descriptionLength = textBoxDescription.Text.Length;
            if (descriptionLength > 200)
            {
                error = "Description is too long";
                e.Cancel = true;
            }
            if (error != null) IsSaved = false;
            errorProviderOrder.SetError((Control)sender, error);
        }

        private void buttonCancelOrder_Click(object sender, EventArgs e)
        {
            IsSaved = false;
            this.Close();
        }
    }
}
