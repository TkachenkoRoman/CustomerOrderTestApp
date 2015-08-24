using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace CustomerOrderApp
{
    public partial class CustomerDetailsForm : Form
    {
        public Customer Customer { get { return _customer; } }
        private Customer _customer;
        private bool _isEditForm;
        public bool IsSaved { get; set; }
        public CustomerDetailsForm(Customer customer = null)
        {
            _isEditForm = false;
            _customer = new Customer();
            InitializeComponent();
            this.buttonCustomerCancel.CausesValidation = false;
            if (customer != null)
            {
                _isEditForm = true;
                this._customer = customer;
                this.textBoxCustomerId.Text = _customer.CustomerId.ToString();
                this.textBoxCustomerId.ReadOnly = true;
                this.textBoxAddress.Text = _customer.Address;
                this.textBoxName.Text = _customer.Name;
                this.textBoxPhoneNumber.Text = _customer.PhoneNum;
            }
        }

        private void buttonCustomerSave_Click(object sender, EventArgs e)
        {
            IsSaved = true;
            textBoxCustomerId_Validating(textBoxCustomerId, new CancelEventArgs(false));
            if (!IsSaved) return;
            textBoxName_Validating(textBoxName, new CancelEventArgs(false));
            if (!IsSaved) return;
            textBoxAddress_Validating(textBoxAddress, new CancelEventArgs(false));
            if (!IsSaved) return;
            textBoxPhoneNumber_Validating(textBoxPhoneNumber, new CancelEventArgs(false));
            if (!IsSaved) return;

            _customer.CustomerId = Convert.ToInt32(this.textBoxCustomerId.Text);
            _customer.Address = this.textBoxAddress.Text;
            _customer.Name = this.textBoxName.Text;
            _customer.PhoneNum = this.textBoxPhoneNumber.Text;
            this.Close();
        }
        void buttonCustomerCancel_Click(object sender, System.EventArgs e)
        {
            IsSaved = false;
            this.Close();
        }

        private void textBoxCustomerId_Validating(object sender, CancelEventArgs e)
        {
            string error = null;
            if (textBoxCustomerId.Text.Length == 0)
            {
                error = "Please enter id";
                e.Cancel = true;
            }
            else
            {
                try
                {
                    int temp = int.Parse(textBoxCustomerId.Text);
                    if (!IsUniqueCustomerId(temp) && !_isEditForm)
                    {
                        error = "Id already exists";
                        e.Cancel = true;
                    }
                    else if (temp <= 0)
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
            errorProviderCustomer.SetError(textBoxCustomerId, error);
        }
       
        private bool IsUniqueCustomerId(int id)
        {
            CustomerContext context = new CustomerContext();
            if (context.Customer.Any(x => x.CustomerId == id))
                return false;
            return true;
        }

        private void textBoxName_Validating(object sender, CancelEventArgs e)
        {
            string error = null;
            int nameLength = textBoxName.Text.Length;
            if (nameLength == 0)
            {
                error = "Please enter name";
                e.Cancel = true;
            }
            else
            {
                if (nameLength > 50)
                {
                    error = "Name is too long";
                    e.Cancel = true;
                }                  
            }
            if (error != null) IsSaved = false;
            errorProviderCustomer.SetError((Control)sender, error);
        }

        private void textBoxAddress_Validating(object sender, CancelEventArgs e)
        {
            string error = null;
            int addressLength = textBoxAddress.Text.Length;
            if (addressLength == 0)
            {
                error = "Please enter address";
                e.Cancel = true;
            }
            else
            {
                if (addressLength > 100)
                {
                    error = "Address is too long";
                    e.Cancel = true;
                }
                    
            }
            if (error != null) IsSaved = false;
            errorProviderCustomer.SetError((Control)sender, error);
        }

        private void textBoxPhoneNumber_Validating(object sender, CancelEventArgs e)
        {
            string error = null;
            int phoneNumLength = textBoxPhoneNumber.Text.Length;
            if (phoneNumLength == 0)
            {
                error = "Please enter phone number";
                e.Cancel = true;
            }
            else
            {
                if (phoneNumLength > 50)
                {
                    error = "Phone number is too long";
                    e.Cancel = true;
                }
            }
            if (error != null) IsSaved = false;
            errorProviderCustomer.SetError((Control)sender, error);
        }

    }
}
