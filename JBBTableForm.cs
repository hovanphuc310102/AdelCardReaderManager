using System;
using System.Data;
using System.Windows.Forms;
using System.Drawing;


namespace AdelCardReaderManager
{
    public partial class JBBTableForm : Form
    {
        private DataGridView dataGridView;
        private Button btnRefresh;
        private Button btnClose;
        private Label lblStatus;
        private Label lblConnectionInfo;

        public JBBTableForm()
        {
            // We're not using a designer-generated InitializeComponent method
            this.Text = "JBB Table Records";
            this.Size = new System.Drawing.Size(900, 600);
            this.StartPosition = FormStartPosition.CenterParent;

            InitializeComponents();
            LoadJBBData();
        }

        private void InitializeComponents()
        {
            // Initialize DataGridView
            dataGridView = new DataGridView();
            dataGridView.Dock = DockStyle.Top;
            dataGridView.Height = 500;
            dataGridView.AllowUserToAddRows = false;
            dataGridView.AllowUserToDeleteRows = false;
            dataGridView.ReadOnly = true;
            dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Initialize buttons
            btnRefresh = new Button();
            btnRefresh.Text = "Refresh Data";
            btnRefresh.Width = 120;
            btnRefresh.Height = 30;
            btnRefresh.Location = new System.Drawing.Point(20, 510);
            btnRefresh.Click += BtnRefresh_Click;

            btnClose = new Button();
            btnClose.Text = "Close";
            btnClose.Width = 120;
            btnClose.Height = 30;
            btnClose.Location = new System.Drawing.Point(750, 510);
            btnClose.Click += BtnClose_Click;

            // Initialize status label
            lblStatus = new Label();
            lblStatus.AutoSize = true;
            lblStatus.Location = new System.Drawing.Point(150, 515);
            lblStatus.Text = "Ready";
        
            // Initialize connection info label
            lblConnectionInfo = new Label();
            lblConnectionInfo.AutoSize = true;
            lblConnectionInfo.Location = new System.Drawing.Point(20, 550);
            // Mask password in connection string display
            string connectionString = DatabaseHelper.GetConnectionString();
            string maskedConnectionString = connectionString.Replace("Password=ADELOK", "Password=*****");
            lblConnectionInfo.Text = $"Connection: {maskedConnectionString}";
            lblConnectionInfo.ForeColor = Color.Blue;

            // Add controls to form
            this.Controls.Add(dataGridView);
            this.Controls.Add(btnRefresh);
            this.Controls.Add(btnClose);
            this.Controls.Add(lblStatus);
            this.Controls.Add(lblConnectionInfo);
        }

        private void LoadJBBData()
        {
            try
            {
                lblStatus.Text = "Loading data...";
                Application.DoEvents();

                DataTable jbbData = DatabaseHelper.GetAllJBBRecords();
                
                if (jbbData.Rows.Count > 0)
                {
                    dataGridView.DataSource = jbbData;
                    lblStatus.Text = $"Loaded {jbbData.Rows.Count} records successfully.";
                }
                else
                {
                    lblStatus.Text = "No records found in JBB table.";
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Error: {ex.Message}";
                MessageBox.Show($"Error loading JBB data: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnRefresh_Click(object? sender, EventArgs e)
        {
            LoadJBBData();
        }

        private void BtnClose_Click(object? sender, EventArgs e)
        {
            this.Close();
        }
    }
}