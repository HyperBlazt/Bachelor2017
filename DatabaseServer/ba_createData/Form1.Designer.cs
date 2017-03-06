namespace ba_createData
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.createDataFile = new System.ComponentModel.BackgroundWorker();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.MainTextArea = new System.Windows.Forms.RichTextBox();
            this.button4 = new System.Windows.Forms.Button();
            this.UseRAMOnly = new System.Windows.Forms.CheckBox();
            this.UseHHDOnly = new System.Windows.Forms.CheckBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.button5 = new System.Windows.Forms.Button();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.button6 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.textBox6 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // createDataFile
            // 
            this.createDataFile.DoWork += new System.ComponentModel.DoWorkEventHandler(this.CreateDatafileDoWork);
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.DarkSlateGray;
            this.button1.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.button1.Location = new System.Drawing.Point(13, 32);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(131, 23);
            this.button1.TabIndex = 6;
            this.button1.Text = "Create Database";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.DarkSlateGray;
            this.button2.ForeColor = System.Drawing.SystemColors.Control;
            this.button2.Location = new System.Drawing.Point(13, 62);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(130, 23);
            this.button2.TabIndex = 7;
            this.button2.Text = "Create Suffix Array";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.Color.DarkSlateGray;
            this.button3.ForeColor = System.Drawing.SystemColors.Control;
            this.button3.Location = new System.Drawing.Point(13, 92);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(130, 23);
            this.button3.TabIndex = 8;
            this.button3.Text = "Create Suffix Tree";
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.BackColor = System.Drawing.Color.DarkSlateGray;
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.ForeColor = System.Drawing.SystemColors.Control;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(138, 241);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(42, 21);
            this.comboBox1.TabIndex = 6;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // MainTextArea
            // 
            this.MainTextArea.BackColor = System.Drawing.Color.DarkSlateGray;
            this.MainTextArea.ForeColor = System.Drawing.SystemColors.Control;
            this.MainTextArea.Location = new System.Drawing.Point(237, 32);
            this.MainTextArea.Name = "MainTextArea";
            this.MainTextArea.Size = new System.Drawing.Size(314, 207);
            this.MainTextArea.TabIndex = 10;
            this.MainTextArea.Text = "";
            // 
            // button4
            // 
            this.button4.BackColor = System.Drawing.Color.DarkSlateGray;
            this.button4.ForeColor = System.Drawing.SystemColors.Control;
            this.button4.Location = new System.Drawing.Point(13, 122);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(130, 23);
            this.button4.TabIndex = 11;
            this.button4.Text = "Update Database";
            this.button4.UseVisualStyleBackColor = false;
            // 
            // UseRAMOnly
            // 
            this.UseRAMOnly.AutoSize = true;
            this.UseRAMOnly.Checked = true;
            this.UseRAMOnly.CheckState = System.Windows.Forms.CheckState.Checked;
            this.UseRAMOnly.Location = new System.Drawing.Point(12, 151);
            this.UseRAMOnly.Name = "UseRAMOnly";
            this.UseRAMOnly.Size = new System.Drawing.Size(94, 17);
            this.UseRAMOnly.TabIndex = 12;
            this.UseRAMOnly.Text = "Use RAM only";
            this.UseRAMOnly.UseVisualStyleBackColor = true;
            this.UseRAMOnly.CheckedChanged += new System.EventHandler(this.UseRAMOnly_CheckedChanged);
            // 
            // UseHHDOnly
            // 
            this.UseHHDOnly.AutoSize = true;
            this.UseHHDOnly.Location = new System.Drawing.Point(12, 173);
            this.UseHHDOnly.Name = "UseHHDOnly";
            this.UseHHDOnly.Size = new System.Drawing.Size(94, 17);
            this.UseHHDOnly.TabIndex = 13;
            this.UseHHDOnly.Text = "Use HHD only";
            this.UseHHDOnly.UseVisualStyleBackColor = true;
            this.UseHHDOnly.CheckedChanged += new System.EventHandler(this.UseHHDOnly_CheckedChanged);
            // 
            // textBox1
            // 
            this.textBox1.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.textBox1.BackColor = System.Drawing.Color.Teal;
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox1.Font = new System.Drawing.Font("Modern No. 20", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.Location = new System.Drawing.Point(13, 192);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(130, 17);
            this.textBox1.TabIndex = 14;
            this.textBox1.Text = "Server";
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // textBox2
            // 
            this.textBox2.BackColor = System.Drawing.Color.Teal;
            this.textBox2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox2.Font = new System.Drawing.Font("Modern No. 20", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox2.Location = new System.Drawing.Point(13, 9);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(130, 17);
            this.textBox2.TabIndex = 15;
            this.textBox2.Text = "Options";
            // 
            // textBox3
            // 
            this.textBox3.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.textBox3.BackColor = System.Drawing.Color.Teal;
            this.textBox3.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox3.Font = new System.Drawing.Font("Modern No. 20", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox3.Location = new System.Drawing.Point(237, 9);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(124, 17);
            this.textBox3.TabIndex = 16;
            this.textBox3.Text = "Server Messages";
            // 
            // button5
            // 
            this.button5.BackColor = System.Drawing.Color.DarkSlateGray;
            this.button5.ForeColor = System.Drawing.SystemColors.Control;
            this.button5.Location = new System.Drawing.Point(496, 7);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(55, 23);
            this.button5.TabIndex = 20;
            this.button5.Text = "Close";
            this.button5.UseVisualStyleBackColor = false;
            this.button5.Click += new System.EventHandler(button5_Click);
            // 
            // textBox4
            // 
            this.textBox4.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.textBox4.BackColor = System.Drawing.Color.Teal;
            this.textBox4.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox4.Cursor = System.Windows.Forms.Cursors.No;
            this.textBox4.Font = new System.Drawing.Font("Modern No. 20", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox4.Location = new System.Drawing.Point(107, 192);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(124, 17);
            this.textBox4.TabIndex = 19;
            this.textBox4.Text = "Advanced Option";
            // 
            // button6
            // 
            this.button6.BackColor = System.Drawing.Color.DarkSlateGray;
            this.button6.ForeColor = System.Drawing.SystemColors.Control;
            this.button6.Location = new System.Drawing.Point(138, 214);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(45, 23);
            this.button6.TabIndex = 21;
            this.button6.Text = "-->";
            this.button6.UseVisualStyleBackColor = false;
            // 
            // button7
            // 
            this.button7.BackColor = System.Drawing.Color.Green;
            this.button7.Location = new System.Drawing.Point(12, 214);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(40, 23);
            this.button7.TabIndex = 22;
            this.button7.Text = "On";
            this.button7.UseVisualStyleBackColor = false;
            // 
            // button8
            // 
            this.button8.BackColor = System.Drawing.Color.Red;
            this.button8.Location = new System.Drawing.Point(58, 214);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(43, 23);
            this.button8.TabIndex = 23;
            this.button8.Text = "Off";
            this.button8.UseVisualStyleBackColor = false;
            // 
            // textBox5
            // 
            this.textBox5.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.textBox5.BackColor = System.Drawing.Color.Teal;
            this.textBox5.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox5.Font = new System.Drawing.Font("Modern No. 20", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox5.Location = new System.Drawing.Point(393, 266);
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new System.Drawing.Size(100, 17);
            this.textBox5.TabIndex = 24;
            this.textBox5.Text = "Server Status:";
            this.textBox5.TextChanged += new System.EventHandler(this.textBox5_TextChanged);
            // 
            // textBox6
            // 
            this.textBox6.BackColor = System.Drawing.Color.DarkSlateGray;
            this.textBox6.Font = new System.Drawing.Font("Modern No. 20", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox6.ForeColor = System.Drawing.Color.Red;
            this.textBox6.Location = new System.Drawing.Point(501, 259);
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new System.Drawing.Size(50, 24);
            this.textBox6.TabIndex = 25;
            this.textBox6.Text = "Off";
            this.textBox6.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Teal;
            this.ClientSize = new System.Drawing.Size(565, 295);
            this.Controls.Add(this.textBox6);
            this.Controls.Add(this.textBox5);
            this.Controls.Add(this.button8);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.textBox4);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.UseHHDOnly);
            this.Controls.Add(this.UseRAMOnly);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.MainTextArea);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Form1";
            this.Text = "Malware Database Server";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        /// <summary>
        /// The create data file.
        /// </summary>
        private System.ComponentModel.BackgroundWorker createDataFile;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.RichTextBox MainTextArea;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.CheckBox UseRAMOnly;
        private System.Windows.Forms.CheckBox UseHHDOnly;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.TextBox textBox5;
        private System.Windows.Forms.TextBox textBox6;
    }
}

