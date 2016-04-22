namespace SeaBattleClient
{
    partial class RegimeDialog
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
            this.label1 = new System.Windows.Forms.Label();
            this.buttonWithRealPerson = new System.Windows.Forms.Button();
            this.buttonBotLvl1 = new System.Windows.Forms.Button();
            this.buttonBotLvl2 = new System.Windows.Forms.Button();
            this.buttonBotLvl3 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Palatino Linotype", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(34, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(214, 26);
            this.label1.TabIndex = 0;
            this.label1.Text = "Выберите режим игры";
            // 
            // buttonWithRealPerson
            // 
            this.buttonWithRealPerson.Font = new System.Drawing.Font("Palatino Linotype", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonWithRealPerson.Location = new System.Drawing.Point(12, 49);
            this.buttonWithRealPerson.Name = "buttonWithRealPerson";
            this.buttonWithRealPerson.Size = new System.Drawing.Size(260, 60);
            this.buttonWithRealPerson.TabIndex = 1;
            this.buttonWithRealPerson.Text = "С живым человеком";
            this.buttonWithRealPerson.UseVisualStyleBackColor = true;
            this.buttonWithRealPerson.Click += new System.EventHandler(this.buttonWithRealPerson_Click);
            // 
            // buttonBotLvl1
            // 
            this.buttonBotLvl1.Font = new System.Drawing.Font("Palatino Linotype", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonBotLvl1.Location = new System.Drawing.Point(12, 115);
            this.buttonBotLvl1.Name = "buttonBotLvl1";
            this.buttonBotLvl1.Size = new System.Drawing.Size(260, 60);
            this.buttonBotLvl1.TabIndex = 2;
            this.buttonBotLvl1.Text = "С ботом lvl 1";
            this.buttonBotLvl1.UseVisualStyleBackColor = true;
            this.buttonBotLvl1.Click += new System.EventHandler(this.buttonBotLvl1_Click);
            // 
            // buttonBotLvl2
            // 
            this.buttonBotLvl2.DialogResult = System.Windows.Forms.DialogResult.No;
            this.buttonBotLvl2.Font = new System.Drawing.Font("Palatino Linotype", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonBotLvl2.Location = new System.Drawing.Point(12, 181);
            this.buttonBotLvl2.Name = "buttonBotLvl2";
            this.buttonBotLvl2.Size = new System.Drawing.Size(260, 60);
            this.buttonBotLvl2.TabIndex = 2;
            this.buttonBotLvl2.Text = "С ботом lvl 2";
            this.buttonBotLvl2.UseVisualStyleBackColor = true;
            this.buttonBotLvl2.Click += new System.EventHandler(this.buttonBotLvl2_Click);
            // 
            // buttonBotLvl3
            // 
            this.buttonBotLvl3.DialogResult = System.Windows.Forms.DialogResult.No;
            this.buttonBotLvl3.Font = new System.Drawing.Font("Palatino Linotype", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonBotLvl3.Location = new System.Drawing.Point(12, 247);
            this.buttonBotLvl3.Name = "buttonBotLvl3";
            this.buttonBotLvl3.Size = new System.Drawing.Size(260, 60);
            this.buttonBotLvl3.TabIndex = 2;
            this.buttonBotLvl3.Text = "С ботом lvl 3";
            this.buttonBotLvl3.UseVisualStyleBackColor = true;
            this.buttonBotLvl3.Click += new System.EventHandler(this.buttonBotLvl3_Click);
            // 
            // RegimeDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 318);
            this.ControlBox = false;
            this.Controls.Add(this.buttonBotLvl2);
            this.Controls.Add(this.buttonBotLvl3);
            this.Controls.Add(this.buttonBotLvl1);
            this.Controls.Add(this.buttonWithRealPerson);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RegimeDialog";
            this.Text = "Режим игры";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonWithRealPerson;
        private System.Windows.Forms.Button buttonBotLvl1;
        private System.Windows.Forms.Button buttonBotLvl2;
        private System.Windows.Forms.Button buttonBotLvl3;
    }
}