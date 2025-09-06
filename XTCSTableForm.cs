using System;
using System.Data;
using System.Windows.Forms;
using System.Drawing;

namespace AdelCardReaderManager
{
    public partial class XTCSTableForm : Form
    {
        private DataGridView dataGridView;
        private Button btnRefresh;
        private Button btnClose;
        private Label lblStatus;
        private Label lblConnectionInfo;
        private Label lblExplanation;

        public XTCSTableForm()
        {
            // We're not using a designer-generated InitializeComponent method
            this.Text = "XTCS Table Records (Authorization Codes)";
            this.Size = new System.Drawing.Size(1000, 650);
            this.StartPosition = FormStartPosition.CenterParent;

            InitializeComponents();
            LoadXTCSData();
        }

        private void InitializeComponents()
        {
            // Initialize explanation label
            lblExplanation = new Label();
            lblExplanation.AutoSize = false;
            lblExplanation.Width = 960;
            lblExplanation.Height = 40;
            lblExplanation.Location = new System.Drawing.Point(20, 10);
            lblExplanation.Text = "The XTCS table contains authorization codes and parameters used by the ADEL SDK for Init() function. " + 
                                 "Error code 33 (Invalid authorization code) might be related to mismatched values in this table.";
            lblExplanation.Font = new Font(lblExplanation.Font, FontStyle.Bold);
            
            // Initialize DataGridView
            dataGridView = new DataGridView();
            dataGridView.Location = new Point(20, 60);
            dataGridView.Width = 960;
            dataGridView.Height = 500;
            dataGridView.AllowUserToAddRows = false;
            dataGridView.AllowUserToDeleteRows = false;
            dataGridView.ReadOnly = true;
            dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView.ColumnHeadersDefaultCellStyle.Font = new Font(dataGridView.Font, FontStyle.Bold);

            // Initialize buttons
            btnRefresh = new Button();
            btnRefresh.Text = "Refresh Data";
            btnRefresh.Width = 120;
            btnRefresh.Height = 30;
            btnRefresh.Location = new System.Drawing.Point(20, 570);
            btnRefresh.Click += BtnRefresh_Click;

            btnClose = new Button();
            btnClose.Text = "Close";
            btnClose.Width = 120;
            btnClose.Height = 30;
            btnClose.Location = new System.Drawing.Point(860, 570);
            btnClose.Click += BtnClose_Click;

            // Initialize status label
            lblStatus = new Label();
            lblStatus.AutoSize = true;
            lblStatus.Location = new System.Drawing.Point(150, 575);
            lblStatus.Text = "Ready";
        
            // Initialize connection info label
            lblConnectionInfo = new Label();
            lblConnectionInfo.AutoSize = true;
            lblConnectionInfo.Location = new System.Drawing.Point(20, 610);
            // Mask password in connection string display
            string connectionString = DatabaseHelper.GetConnectionString();
            string maskedConnectionString = connectionString.Replace("Password=ADELOK", "Password=*****");
            lblConnectionInfo.Text = $"Connection: {maskedConnectionString}";
            lblConnectionInfo.ForeColor = Color.Blue;

            // Add controls to form
            this.Controls.Add(lblExplanation);
            this.Controls.Add(dataGridView);
            this.Controls.Add(btnRefresh);
            this.Controls.Add(btnClose);
            this.Controls.Add(lblStatus);
            this.Controls.Add(lblConnectionInfo);
        }

        private void LoadXTCSData()
        {
            try
            {
                lblStatus.Text = "Loading XTCS data...";
                Application.DoEvents();

                DataTable xtcsData = DatabaseHelper.GetAllXTCSRecords();
                
                if (xtcsData.Rows.Count > 0)
                {
                    dataGridView.DataSource = xtcsData;
                    
                    // Highlight columns that might be related to error code 33
                    foreach (DataGridViewColumn column in dataGridView.Columns)
                    {
                        if (column.Name.Contains("RIGHT") || 
                            column.Name.Contains("AUTH") || 
                            column.Name.Contains("USER") || 
                            column.Name.Contains("PARAMETER"))
                        {
                            column.DefaultCellStyle.BackColor = Color.LightYellow;
                        }
                    }
                    
                    lblStatus.Text = $"Loaded {xtcsData.Rows.Count} records successfully. Yellow columns might be related to authorization.";
                }
                else
                {
                    lblStatus.Text = "No records found in XTCS table.";
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Error: {ex.Message}";
                MessageBox.Show($"Error loading XTCS data: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnRefresh_Click(object? sender, EventArgs e)
        {
            LoadXTCSData();
        }

        private void BtnClose_Click(object? sender, EventArgs e)
        {
            this.Close();
        }
    }
}