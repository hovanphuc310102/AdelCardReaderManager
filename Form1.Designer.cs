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
        this.btnInitWithDatabase = new System.Windows.Forms.Button();
        this.btnReadCard = new System.Windows.Forms.Button();
        this.btnCreateCard = new System.Windows.Forms.Button();
        this.txtOutput = new System.Windows.Forms.TextBox();
        this.txtRoomNumber = new System.Windows.Forms.TextBox();
        this.txtGuestName = new System.Windows.Forms.TextBox();
        this.lblRoom = new System.Windows.Forms.Label();
        this.lblGuestName = new System.Windows.Forms.Label();
        this.cmbLockSystem = new System.Windows.Forms.ComboBox();
        this.lblLockSystem = new System.Windows.Forms.Label();
        this.lblStatus = new System.Windows.Forms.Label();
        this.btnClearLog = new System.Windows.Forms.Button();
        this.lblConnectionType = new System.Windows.Forms.Label();
        this.btnTestAllSystems = new System.Windows.Forms.Button();
        
        // New buttons for individual ReadCard function tests
        this.btnTestReadCardId = new System.Windows.Forms.Button();
        this.btnTestReadMagCard = new System.Windows.Forms.Button();
        this.btnTestReadIC = new System.Windows.Forms.Button();
        this.btnTestGetCardInfo = new System.Windows.Forms.Button();
        this.btnTestReaderBeep = new System.Windows.Forms.Button();
        this.lblReadCardTests = new System.Windows.Forms.Label();
        
        // New Direct ReadCard button
        this.btnReadCardDirect = new System.Windows.Forms.Button();
        
        // New Auth Test button
        this.btnTestAuth = new System.Windows.Forms.Button();
        
        // New JBB Table button
        this.btnViewJBBTable = new System.Windows.Forms.Button();
        
        // New XTCS Table button
        this.btnViewXTCSTable = new System.Windows.Forms.Button();
        
        this.SuspendLayout();
        // 
        // btnInitialize
        // 
        this.btnInitialize.Location = new System.Drawing.Point(12, 100);
        this.btnInitialize.Name = "btnInitialize";
        this.btnInitialize.Size = new System.Drawing.Size(120, 35);
        this.btnInitialize.TabIndex = 0;
        this.btnInitialize.Text = "Initialize USB (SetPort)";
        this.btnInitialize.UseVisualStyleBackColor = true;
        this.btnInitialize.Click += new System.EventHandler(this.btnInitialize_Click);
        // 
        // btnInitWithDatabase
        // 
        this.btnInitWithDatabase = new System.Windows.Forms.Button();
        this.btnInitWithDatabase.Location = new System.Drawing.Point(140, 100);
        this.btnInitWithDatabase.Name = "btnInitWithDatabase";
        this.btnInitWithDatabase.Size = new System.Drawing.Size(120, 35);
        this.btnInitWithDatabase.TabIndex = 0;
        this.btnInitWithDatabase.Text = "Init with Database";
        this.btnInitWithDatabase.UseVisualStyleBackColor = true;
        this.btnInitWithDatabase.Click += new System.EventHandler(this.btnInitWithDatabase_Click);
        // 
        // btnTestAuth
        // 
        this.btnTestAuth.Location = new System.Drawing.Point(270, 100);
        this.btnTestAuth.Name = "btnTestAuth";
        this.btnTestAuth.Size = new System.Drawing.Size(120, 35);
        this.btnTestAuth.TabIndex = 28;
        this.btnTestAuth.Text = "Change User to Admin";
        this.btnTestAuth.UseVisualStyleBackColor = false;
        this.btnTestAuth.BackColor = System.Drawing.Color.LightBlue;
        this.btnTestAuth.Click += new System.EventHandler(this.btnTestAuth_Click);
        // 
        // btnViewJBBTable
        // 
        this.btnViewJBBTable.Location = new System.Drawing.Point(140, 145);
        this.btnViewJBBTable.Name = "btnViewJBBTable";
        this.btnViewJBBTable.Size = new System.Drawing.Size(120, 35);
        this.btnViewJBBTable.TabIndex = 30;
        this.btnViewJBBTable.Text = "View JBB Table";
        this.btnViewJBBTable.UseVisualStyleBackColor = false;
        this.btnViewJBBTable.BackColor = System.Drawing.Color.LightGreen;
        this.btnViewJBBTable.Click += new System.EventHandler(this.btnViewJBBTable_Click);
        // 
        // btnViewXTCSTable
        // 
        this.btnViewXTCSTable.Location = new System.Drawing.Point(270, 145);
        this.btnViewXTCSTable.Name = "btnViewXTCSTable";
        this.btnViewXTCSTable.Size = new System.Drawing.Size(120, 35);
        this.btnViewXTCSTable.TabIndex = 31;
        this.btnViewXTCSTable.Text = "View XTCS Table";
        this.btnViewXTCSTable.UseVisualStyleBackColor = false;
        this.btnViewXTCSTable.BackColor = System.Drawing.Color.LightYellow;
        this.btnViewXTCSTable.Click += new System.EventHandler(this.btnViewXTCSTable_Click);
        // 
        // btnReadCard
        // 
        this.btnReadCard.Enabled = false;
        this.btnReadCard.Location = new System.Drawing.Point(400, 100);
        this.btnReadCard.Name = "btnReadCard";
        this.btnReadCard.Size = new System.Drawing.Size(100, 35);
        this.btnReadCard.TabIndex = 1;
        this.btnReadCard.Text = "Read Card";
        this.btnReadCard.UseVisualStyleBackColor = true;
        this.btnReadCard.Click += new System.EventHandler(this.btnReadCard_Click);
        // 
        // btnCreateCard
        // 
        this.btnCreateCard.Enabled = false;
        this.btnCreateCard.Location = new System.Drawing.Point(510, 100);
        this.btnCreateCard.Name = "btnCreateCard";
        this.btnCreateCard.Size = new System.Drawing.Size(100, 35);
        this.btnCreateCard.TabIndex = 2;
        this.btnCreateCard.Text = "Create Card";
        this.btnCreateCard.UseVisualStyleBackColor = true;
        this.btnCreateCard.Click += new System.EventHandler(this.btnCreateCard_Click);
        // 
        // btnTestAllSystems
        // 
        this.btnTestAllSystems.Enabled = false;
        this.btnTestAllSystems.Location = new System.Drawing.Point(12, 145);
        this.btnTestAllSystems.Name = "btnTestAllSystems";
        this.btnTestAllSystems.Size = new System.Drawing.Size(120, 35);
        this.btnTestAllSystems.TabIndex = 3;
        this.btnTestAllSystems.Text = "Test A90 Config";
        this.btnTestAllSystems.UseVisualStyleBackColor = true;
        this.btnTestAllSystems.Click += new System.EventHandler(this.btnTestAllSystems_Click);
        //
        // btnReadCardDirect
        //
        this.btnReadCardDirect = new System.Windows.Forms.Button();
        this.btnReadCardDirect.Enabled = false;
        this.btnReadCardDirect.Location = new System.Drawing.Point(650, 50);
        this.btnReadCardDirect.Name = "btnReadCardDirect";
        this.btnReadCardDirect.Size = new System.Drawing.Size(110, 50);
        this.btnReadCardDirect.TabIndex = 27;
        this.btnReadCardDirect.Text = "🔍 Direct ReadCard";
        this.btnReadCardDirect.UseVisualStyleBackColor = false;
        this.btnReadCardDirect.BackColor = System.Drawing.Color.LightGreen;
        this.btnReadCardDirect.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
        this.btnReadCardDirect.Click += new System.EventHandler(this.btnReadCardDirect_Click);
        //
        // lblReadCardTests
        //
        this.lblReadCardTests.AutoSize = true;
        this.lblReadCardTests.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
        this.lblReadCardTests.Location = new System.Drawing.Point(530, 23);
        this.lblReadCardTests.Name = "lblReadCardTests";
        this.lblReadCardTests.Size = new System.Drawing.Size(150, 15);
        this.lblReadCardTests.TabIndex = 21;
        this.lblReadCardTests.Text = "ReadCard Function Tests:";
        //
        // btnTestReadCardId
        //
        this.btnTestReadCardId.Enabled = false;
        this.btnTestReadCardId.Location = new System.Drawing.Point(530, 50);
        this.btnTestReadCardId.Name = "btnTestReadCardId";
        this.btnTestReadCardId.Size = new System.Drawing.Size(110, 30);
        this.btnTestReadCardId.TabIndex = 22;
        this.btnTestReadCardId.Text = "Test ReadCardId";
        this.btnTestReadCardId.UseVisualStyleBackColor = true;
        this.btnTestReadCardId.Click += new System.EventHandler(this.btnTestReadCardId_Click);
        //
        // btnTestReadMagCard
        //
        this.btnTestReadMagCard.Enabled = false;
        this.btnTestReadMagCard.Location = new System.Drawing.Point(530, 85);
        this.btnTestReadMagCard.Name = "btnTestReadMagCard";
        this.btnTestReadMagCard.Size = new System.Drawing.Size(110, 30);
        this.btnTestReadMagCard.TabIndex = 23;
        this.btnTestReadMagCard.Text = "Test ReadMagCard";
        this.btnTestReadMagCard.UseVisualStyleBackColor = true;
        this.btnTestReadMagCard.Click += new System.EventHandler(this.btnTestReadMagCard_Click);
        //
        // btnTestReadIC
        //
        this.btnTestReadIC.Enabled = false;
        this.btnTestReadIC.Location = new System.Drawing.Point(530, 120);
        this.btnTestReadIC.Name = "btnTestReadIC";
        this.btnTestReadIC.Size = new System.Drawing.Size(110, 30);
        this.btnTestReadIC.TabIndex = 24;
        this.btnTestReadIC.Text = "Test ReadIC";
        this.btnTestReadIC.UseVisualStyleBackColor = true;
        this.btnTestReadIC.Click += new System.EventHandler(this.btnTestReadIC_Click);
        //
        // btnTestGetCardInfo
        //
        this.btnTestGetCardInfo.Enabled = false;
        this.btnTestGetCardInfo.Location = new System.Drawing.Point(530, 155);
        this.btnTestGetCardInfo.Name = "btnTestGetCardInfo";
        this.btnTestGetCardInfo.Size = new System.Drawing.Size(110, 30);
        this.btnTestGetCardInfo.TabIndex = 25;
        this.btnTestGetCardInfo.Text = "Test GetCardInfo";
        this.btnTestGetCardInfo.UseVisualStyleBackColor = true;
        this.btnTestGetCardInfo.Click += new System.EventHandler(this.btnTestGetCardInfo_Click);
        //
        // btnTestReaderBeep
        //
        this.btnTestReaderBeep.Enabled = false;
        this.btnTestReaderBeep.Location = new System.Drawing.Point(530, 190);
        this.btnTestReaderBeep.Name = "btnTestReaderBeep";
        this.btnTestReaderBeep.Size = new System.Drawing.Size(110, 30);
        this.btnTestReaderBeep.TabIndex = 26;
        this.btnTestReaderBeep.Text = "Test Reader_Beep";
        this.btnTestReaderBeep.UseVisualStyleBackColor = true;
        this.btnTestReaderBeep.Click += new System.EventHandler(this.btnTestReaderBeep_Click);
        // 
        // txtOutput
        // 
        this.txtOutput.Location = new System.Drawing.Point(12, 230);
        this.txtOutput.Multiline = true;
        this.txtOutput.Name = "txtOutput";
        this.txtOutput.ReadOnly = true;
        this.txtOutput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
        this.txtOutput.Size = new System.Drawing.Size(750, 250);
        this.txtOutput.TabIndex = 4;
        // 
        // txtRoomNumber
        // 
        this.txtRoomNumber.Location = new System.Drawing.Point(80, 185);
        this.txtRoomNumber.Name = "txtRoomNumber";
        this.txtRoomNumber.Size = new System.Drawing.Size(100, 23);
        this.txtRoomNumber.TabIndex = 5;
        this.txtRoomNumber.Text = "101";
        // 
        // txtGuestName
        // 
        this.txtGuestName.Location = new System.Drawing.Point(270, 185);
        this.txtGuestName.Name = "txtGuestName";
        this.txtGuestName.Size = new System.Drawing.Size(150, 23);
        this.txtGuestName.TabIndex = 6;
        this.txtGuestName.Text = "John Doe";
        // 
        // lblRoom
        // 
        this.lblRoom.AutoSize = true;
        this.lblRoom.Location = new System.Drawing.Point(12, 188);
        this.lblRoom.Name = "lblRoom";
        this.lblRoom.Size = new System.Drawing.Size(39, 15);
        this.lblRoom.TabIndex = 10;
        this.lblRoom.Text = "Room:";
        // 
        // lblGuestName
        // 
        this.lblGuestName.AutoSize = true;
        this.lblGuestName.Location = new System.Drawing.Point(200, 188);
        this.lblGuestName.Name = "lblGuestName";
        this.lblGuestName.Size = new System.Drawing.Size(42, 15);
        this.lblGuestName.TabIndex = 11;
        this.lblGuestName.Text = "Guest:";
        // 
        // cmbLockSystem
        // 
        this.cmbLockSystem.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.cmbLockSystem.FormattingEnabled = true;
        this.cmbLockSystem.Items.AddRange(new object[] { "A90", "A92", "A30", "Lock9200T", "Lock3200K" });
        this.cmbLockSystem.Location = new System.Drawing.Point(100, 20);
        this.cmbLockSystem.Name = "cmbLockSystem";
        this.cmbLockSystem.Size = new System.Drawing.Size(120, 23);
        this.cmbLockSystem.TabIndex = 17;
        this.cmbLockSystem.SelectedIndex = 0;
        // 
        // lblLockSystem
        // 
        this.lblLockSystem.AutoSize = true;
        this.lblLockSystem.Location = new System.Drawing.Point(12, 23);
        this.lblLockSystem.Name = "lblLockSystem";
        this.lblLockSystem.Size = new System.Drawing.Size(75, 15);
        this.lblLockSystem.TabIndex = 18;
        this.lblLockSystem.Text = "Lock System:";
        // 
        // lblStatus
        // 
        this.lblStatus.AutoSize = true;
        this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
        this.lblStatus.ForeColor = System.Drawing.Color.Red;
        this.lblStatus.Location = new System.Drawing.Point(12, 70);
        this.lblStatus.Name = "lblStatus";
        this.lblStatus.Size = new System.Drawing.Size(145, 15);
        this.lblStatus.TabIndex = 19;
        this.lblStatus.Text = "Status: Not Connected";
        // 
        // btnClearLog
        // 
        this.btnClearLog.Location = new System.Drawing.Point(672, 490);
        this.btnClearLog.Name = "btnClearLog";
        this.btnClearLog.Size = new System.Drawing.Size(90, 28);
        this.btnClearLog.TabIndex = 99;
        this.btnClearLog.Text = "Clear Log";
        this.btnClearLog.UseVisualStyleBackColor = true;
        this.btnClearLog.Click += new System.EventHandler(this.btnClearLog_Click);
        // 
        // lblConnectionType
        // 
        this.lblConnectionType.AutoSize = true;
        this.lblConnectionType.Location = new System.Drawing.Point(250, 23);
        this.lblConnectionType.Name = "lblConnectionType";
        this.lblConnectionType.Size = new System.Drawing.Size(170, 15);
        this.lblConnectionType.TabIndex = 20;
        this.lblConnectionType.Text = "Connection: USB HID (Port 0)";
        // 
        // Form1
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(774, 530);
        this.Controls.Add(this.btnViewXTCSTable);
        this.Controls.Add(this.btnViewJBBTable);
        this.Controls.Add(this.btnTestAuth);
        this.Controls.Add(this.btnReadCardDirect);
        this.Controls.Add(this.btnTestReaderBeep);
        this.Controls.Add(this.btnTestGetCardInfo);
        this.Controls.Add(this.btnTestReadIC);
        this.Controls.Add(this.btnTestReadMagCard);
        this.Controls.Add(this.btnTestReadCardId);
        this.Controls.Add(this.lblReadCardTests);
        this.Controls.Add(this.btnTestAllSystems);
        this.Controls.Add(this.lblConnectionType);
        this.Controls.Add(this.btnClearLog);
        this.Controls.Add(this.lblStatus);
        this.Controls.Add(this.lblLockSystem);
        this.Controls.Add(this.cmbLockSystem);
        this.Controls.Add(this.lblGuestName);
        this.Controls.Add(this.lblRoom);
        this.Controls.Add(this.txtGuestName);
        this.Controls.Add(this.txtRoomNumber);
        this.Controls.Add(this.txtOutput);
        this.Controls.Add(this.btnCreateCard);
        this.Controls.Add(this.btnReadCard);
        this.Controls.Add(this.btnInitWithDatabase);
        this.Controls.Add(this.btnInitialize);
        this.Name = "Form1";
        this.Text = "ADEL Card Reader - Multiple ReadCard Function Tests";
        this.Load += new System.EventHandler(this.Form1_Load);
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    #endregion

    private System.Windows.Forms.Button btnInitialize;
    private System.Windows.Forms.Button btnInitWithDatabase;
    private System.Windows.Forms.Button btnReadCard;
    private System.Windows.Forms.Button btnCreateCard;
    private System.Windows.Forms.TextBox txtOutput;
    private System.Windows.Forms.TextBox txtRoomNumber;
    private System.Windows.Forms.TextBox txtGuestName;
    private System.Windows.Forms.Label lblRoom;
    private System.Windows.Forms.Label lblGuestName;
    private System.Windows.Forms.ComboBox cmbLockSystem;
    private System.Windows.Forms.Label lblLockSystem;
    private System.Windows.Forms.Label lblStatus;
    private System.Windows.Forms.Button btnClearLog;
    private System.Windows.Forms.Label lblConnectionType;
    private System.Windows.Forms.Button btnTestAllSystems;
    
    // New buttons for individual ReadCard function tests
    private System.Windows.Forms.Button btnTestReadCardId;
    private System.Windows.Forms.Button btnTestReadMagCard;
    private System.Windows.Forms.Button btnTestReadIC;
    private System.Windows.Forms.Button btnTestGetCardInfo;
    private System.Windows.Forms.Button btnTestReaderBeep;
    private System.Windows.Forms.Label lblReadCardTests;
    
    // New Direct ReadCard button
    private System.Windows.Forms.Button btnReadCardDirect;
    
    // New Auth Test button
    private System.Windows.Forms.Button btnTestAuth;
    
    // New JBB Table button
    private System.Windows.Forms.Button btnViewJBBTable;
    
    // New XTCS Table button
    private System.Windows.Forms.Button btnViewXTCSTable;
}
