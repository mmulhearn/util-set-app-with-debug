using System;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SetAppWithDebug
{
    public partial class Results : Form
    {
        public Context Context { get; set; }

        public Results()
        {
            InitializeComponent();
        }

        private void Results_Load(object sender, EventArgs e)
        {
            if (Context.Errors.Any())
            {
                lblResult.Text = "Errors occurred. Please see the output below.";
            }
            else
            {
                lblResult.Text = "Success!";
            }

            var output = new StringBuilder();
            output.Append("Altered nugets");
            output.Append(Environment.NewLine);
            output.Append("-----------------------");
            output.Append(Environment.NewLine);
            output.Append(string.Join(Environment.NewLine, Context.AlteredNugets));

            output.Append(Environment.NewLine);
            output.Append(Environment.NewLine);

            output.Append("Errors");
            output.Append(Environment.NewLine);
            output.Append("-----------------------");
            output.Append(string.Join(Environment.NewLine, Context.Errors));

            txtErrors.Text = output.ToString();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
