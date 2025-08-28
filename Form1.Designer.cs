namespace AdelCardReaderManager;

partial class Form1
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        this.btnInitialize = new System.Windows.Forms.Button();
        this.btnReadCard = new System.Windows.Forms.Button();
        this.btnCreateCard = new System.Windows.Forms.Button();
        this.btnEraseCard = new System.Windows.Forms.Button();
        this.txtOutput = new System.Windows.Forms.TextBox();
        this.txtRoomNumber = new System.Windows.Forms.TextBox();
        this.txtGuestName = new System.Windows.Forms.TextBox();
        this.txtGuestId = new System.Windows.Forms.TextBox();
        this.dtpStartTime = new System.Windows.Forms.DateTimePicker();
        this.dtpEndTime = new System.Windows.Forms.DateTimePicker();
        this.lblRoom = new System.Windows.Forms.Label();
        this.lblGuestName = new System.Windows.Forms.Label();
        this.lblGuestId = new System.Windows.Forms.Label();
        this.lblStartTime = new System.Windows.Forms.Label();
        this.lblEndTime = new System.Windows.Forms.Label();
        this.cmbComPort = new System.Windows.Forms.ComboBox();
        this.lblComPort = new System.Windows.Forms.Label();
        this.cmbLockSystem = new System.Windows.Forms.ComboBox();
        this.lblLockSystem = new System.Windows.Forms.Label();
        this.lblStatus = new System.Windows.Forms.Label();
        this.btnRefreshPorts = new System.Windows.Forms.Button();
        this.btnDiagnostics = new System.Windows.Forms.Button();
        this.btnCheckDrivers = new System.Windows.Forms.Button();
        this.SuspendLayout();
        // 
        // btnInitialize
        // 
        this.btnInitialize.Location = new System.Drawing.Point(12, 100);
        this.btnInitialize.Name = "btnInitialize";
        this.btnInitialize.Size = new System.Drawing.Size(100, 30);
        this.btnInitialize.TabIndex = 0;
        this.btnInitialize.Text = "Initialize";
        this.btnInitialize.UseVisualStyleBackColor = true;
        this.btnInitialize.Click += new System.EventHandler(this.btnInitialize_Click);
        // 
        // btnReadCard
        // 
        this.btnReadCard.Location = new System.Drawing.Point(130, 100);
        this.btnReadCard.Name = "btnReadCard";
        this.btnReadCard.Size = new System.Drawing.Size(100, 30);
        this.btnReadCard.TabIndex = 1;
        this.btnReadCard.Text = "Read Card";
        this.btnReadCard.UseVisualStyleBackColor = true;
        this.btnReadCard.Click += new System.EventHandler(this.btnReadCard_Click);
        // 
        // btnCreateCard
        // 
        this.btnCreateCard.Location = new System.Drawing.Point(248, 100);
        this.btnCreateCard.Name = "btnCreateCard";
        this.btnCreateCard.Size = new System.Drawing.Size(100, 30);
        this.btnCreateCard.TabIndex = 2;
        this.btnCreateCard.Text = "Create Card";
        this.btnCreateCard.UseVisualStyleBackColor = true;
        this.btnCreateCard.Click += new System.EventHandler(this.btnCreateCard_Click);
        // 
        // btnEraseCard
        // 
        this.btnEraseCard.Location = new System.Drawing.Point(366, 100);
        this.btnEraseCard.Name = "btnEraseCard";
        this.btnEraseCard.Size = new System.Drawing.Size(100, 30);
        this.btnEraseCard.TabIndex = 3;
        this.btnEraseCard.Text = "Erase Card";
        this.btnEraseCard.UseVisualStyleBackColor = true;
        this.btnEraseCard.Click += new System.EventHandler(this.btnEraseCard_Click);
        // 
        // txtOutput
        // 
        this.txtOutput.Location = new System.Drawing.Point(12, 280);
        this.txtOutput.Multiline = true;
        this.txtOutput.Name = "txtOutput";
        this.txtOutput.ReadOnly = true;
        this.txtOutput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
        this.txtOutput.Size = new System.Drawing.Size(760, 200);
        this.txtOutput.TabIndex = 4;
        // 
        // txtRoomNumber
        // 
        this.txtRoomNumber.Location = new System.Drawing.Point(100, 150);
        this.txtRoomNumber.Name = "txtRoomNumber";
        this.txtRoomNumber.Size = new System.Drawing.Size(100, 23);
        this.txtRoomNumber.TabIndex = 5;
        this.txtRoomNumber.Text = "101";
        // 
        // txtGuestName
        // 
        this.txtGuestName.Location = new System.Drawing.Point(280, 150);
        this.txtGuestName.Name = "txtGuestName";
        this.txtGuestName.Size = new System.Drawing.Size(150, 23);
        this.txtGuestName.TabIndex = 6;
        this.txtGuestName.Text = "John Doe";
        // 
        // txtGuestId
        // 
        this.txtGuestId.Location = new System.Drawing.Point(500, 150);
        this.txtGuestId.Name = "txtGuestId";
        this.txtGuestId.Size = new System.Drawing.Size(120, 23);
        this.txtGuestId.TabIndex = 7;
        this.txtGuestId.Text = "ID123456";
        // 
        // dtpStartTime
        // 
        this.dtpStartTime.CustomFormat = "yyyy-MM-dd HH:mm";
        this.dtpStartTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
        this.dtpStartTime.Location = new System.Drawing.Point(100, 190);
        this.dtpStartTime.Name = "dtpStartTime";
        this.dtpStartTime.Size = new System.Drawing.Size(150, 23);
        this.dtpStartTime.TabIndex = 8;
        // 
        // dtpEndTime
        // 
        this.dtpEndTime.CustomFormat = "yyyy-MM-dd HH:mm";
        this.dtpEndTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
        this.dtpEndTime.Location = new System.Drawing.Point(330, 190);
        this.dtpEndTime.Name = "dtpEndTime";
        this.dtpEndTime.Size = new System.Drawing.Size(150, 23);
        this.dtpEndTime.TabIndex = 9;
        // 
        // lblRoom
        // 
        this.lblRoom.AutoSize = true;
        this.lblRoom.Location = new System.Drawing.Point(12, 153);
        this.lblRoom.Name = "lblRoom";
        this.lblRoom.Size = new System.Drawing.Size(39, 15);
        this.lblRoom.TabIndex = 10;
        this.lblRoom.Text = "Room:";
        // 
        // lblGuestName
        // 
        this.lblGuestName.AutoSize = true;
        this.lblGuestName.Location = new System.Drawing.Point(220, 153);
        this.lblGuestName.Name = "lblGuestName";
        this.lblGuestName.Size = new System.Drawing.Size(42, 15);
        this.lblGuestName.TabIndex = 11;
        this.lblGuestName.Text = "Guest:";
        // 
        // lblGuestId
        // 
        this.lblGuestId.AutoSize = true;
        this.lblGuestId.Location = new System.Drawing.Point(450, 153);
        this.lblGuestId.Name = "lblGuestId";
        this.lblGuestId.Size = new System.Drawing.Size(21, 15);
        this.lblGuestId.TabIndex = 12;
        this.lblGuestId.Text = "ID:";
        // 
        // lblStartTime
        // 
        this.lblStartTime.AutoSize = true;
        this.lblStartTime.Location = new System.Drawing.Point(12, 194);
        this.lblStartTime.Name = "lblStartTime";
        this.lblStartTime.Size = new System.Drawing.Size(34, 15);
        this.lblStartTime.TabIndex = 13;
        this.lblStartTime.Text = "Start:";
        // 
        // lblEndTime
        // 
        this.lblEndTime.AutoSize = true;
        this.lblEndTime.Location = new System.Drawing.Point(270, 194);
        this.lblEndTime.Name = "lblEndTime";
        this.lblEndTime.Size = new System.Drawing.Size(30, 15);
        this.lblEndTime.TabIndex = 14;
        this.lblEndTime.Text = "End:";
        // 
        // cmbComPort
        // 
        this.cmbComPort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.cmbComPort.FormattingEnabled = true;
        this.cmbComPort.Location = new System.Drawing.Point(100, 20);
        this.cmbComPort.Name = "cmbComPort";
        this.cmbComPort.Size = new System.Drawing.Size(100, 23);
        this.cmbComPort.TabIndex = 15;
        // 
        // lblComPort
        // 
        this.lblComPort.AutoSize = true;
        this.lblComPort.Location = new System.Drawing.Point(12, 23);
        this.lblComPort.Name = "lblComPort";
        this.lblComPort.Size = new System.Drawing.Size(62, 15);
        this.lblComPort.TabIndex = 16;
        this.lblComPort.Text = "COM Port:";
        // 
        // btnRefreshPorts
        // 
        this.btnRefreshPorts.Location = new System.Drawing.Point(210, 19);
        this.btnRefreshPorts.Name = "btnRefreshPorts";
        this.btnRefreshPorts.Size = new System.Drawing.Size(60, 25);
        this.btnRefreshPorts.TabIndex = 20;
        this.btnRefreshPorts.Text = "Refresh";
        this.btnRefreshPorts.UseVisualStyleBackColor = true;
        this.btnRefreshPorts.Click += new System.EventHandler(this.btnRefreshPorts_Click);
        // 
        // btnDiagnostics
        // 
        this.btnDiagnostics.Location = new System.Drawing.Point(280, 19);
        this.btnDiagnostics.Name = "btnDiagnostics";
        this.btnDiagnostics.Size = new System.Drawing.Size(80, 25);
        this.btnDiagnostics.TabIndex = 21;
        this.btnDiagnostics.Text = "Diagnostics";
        this.btnDiagnostics.UseVisualStyleBackColor = true;
        this.btnDiagnostics.Click += new System.EventHandler(this.btnDiagnostics_Click);
        // 
        // btnCheckDrivers
        // 
        this.btnCheckDrivers.Location = new System.Drawing.Point(370, 19);
        this.btnCheckDrivers.Name = "btnCheckDrivers";
        this.btnCheckDrivers.Size = new System.Drawing.Size(90, 25);
        this.btnCheckDrivers.TabIndex = 22;
        this.btnCheckDrivers.Text = "Check Drivers";
        this.btnCheckDrivers.UseVisualStyleBackColor = true;
        this.btnCheckDrivers.Click += new System.EventHandler(this.btnCheckDrivers_Click);
        // 
        // cmbLockSystem
        // 
        this.cmbLockSystem.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.cmbLockSystem.FormattingEnabled = true;
        this.cmbLockSystem.Items.AddRange(new object[] { "A90", "A92", "A30", "Lock9200T", "Lock3200K" });
        this.cmbLockSystem.Location = new System.Drawing.Point(550, 20);
        this.cmbLockSystem.Name = "cmbLockSystem";
        this.cmbLockSystem.Size = new System.Drawing.Size(120, 23);
        this.cmbLockSystem.TabIndex = 17;
        this.cmbLockSystem.SelectedIndex = 0;
        // 
        // lblLockSystem
        // 
        this.lblLockSystem.AutoSize = true;
        this.lblLockSystem.Location = new System.Drawing.Point(470, 23);
        this.lblLockSystem.Name = "lblLockSystem";
        this.lblLockSystem.Size = new System.Drawing.Size(75, 15);
        this.lblLockSystem.TabIndex = 18;
        this.lblLockSystem.Text = "Lock System:";
        // 
        // lblStatus
        // 
        this.lblStatus.AutoSize = true;
        this.lblStatus.Location = new System.Drawing.Point(12, 250);
        this.lblStatus.Name = "lblStatus";
        this.lblStatus.Size = new System.Drawing.Size(145, 15);
        this.lblStatus.TabIndex = 19;
        this.lblStatus.Text = "Status: Not initialized";
        // 
        // Form1
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(784, 492);
        this.Controls.Add(this.btnCheckDrivers);
        this.Controls.Add(this.btnDiagnostics);
        this.Controls.Add(this.btnRefreshPorts);
        this.Controls.Add(this.lblStatus);
        this.Controls.Add(this.lblLockSystem);
        this.Controls.Add(this.cmbLockSystem);
        this.Controls.Add(this.lblComPort);
        this.Controls.Add(this.cmbComPort);
        this.Controls.Add(this.lblEndTime);
        this.Controls.Add(this.lblStartTime);
        this.Controls.Add(this.lblGuestId);
        this.Controls.Add(this.lblGuestName);
        this.Controls.Add(this.lblRoom);
        this.Controls.Add(this.dtpEndTime);
        this.Controls.Add(this.dtpStartTime);
        this.Controls.Add(this.txtGuestId);
        this.Controls.Add(this.txtGuestName);
        this.Controls.Add(this.txtRoomNumber);
        this.Controls.Add(this.txtOutput);
        this.Controls.Add(this.btnEraseCard);
        this.Controls.Add(this.btnCreateCard);
        this.Controls.Add(this.btnReadCard);
        this.Controls.Add(this.btnInitialize);
        this.Name = "Form1";
        this.Text = "ADEL Card Reader Manager";
        this.Load += new System.EventHandler(this.Form1_Load);
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    #endregion

    private System.Windows.Forms.Button btnInitialize;
    private System.Windows.Forms.Button btnReadCard;
    private System.Windows.Forms.Button btnCreateCard;
    private System.Windows.Forms.Button btnEraseCard;
    private System.Windows.Forms.TextBox txtOutput;
    private System.Windows.Forms.TextBox txtRoomNumber;
    private System.Windows.Forms.TextBox txtGuestName;
    private System.Windows.Forms.TextBox txtGuestId;
    private System.Windows.Forms.DateTimePicker dtpStartTime;
    private System.Windows.Forms.DateTimePicker dtpEndTime;
    private System.Windows.Forms.Label lblRoom;
    private System.Windows.Forms.Label lblGuestName;
    private System.Windows.Forms.Label lblGuestId;
    private System.Windows.Forms.Label lblStartTime;
    private System.Windows.Forms.Label lblEndTime;
    private System.Windows.Forms.ComboBox cmbComPort;
    private System.Windows.Forms.Label lblComPort;
    private System.Windows.Forms.ComboBox cmbLockSystem;
    private System.Windows.Forms.Label lblLockSystem;
    private System.Windows.Forms.Label lblStatus;
    private System.Windows.Forms.Button btnRefreshPorts;
    private System.Windows.Forms.Button btnDiagnostics;
    private System.Windows.Forms.Button btnCheckDrivers;
}
