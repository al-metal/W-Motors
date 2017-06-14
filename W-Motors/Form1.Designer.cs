namespace W_Motors
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.tbKeywords = new System.Windows.Forms.TextBox();
            this.tbDescription = new System.Windows.Forms.TextBox();
            this.tbTitle = new System.Windows.Forms.TextBox();
            this.rtbFullText = new System.Windows.Forms.RichTextBox();
            this.rtbMiniText = new System.Windows.Forms.RichTextBox();
            this.btnPrice = new System.Windows.Forms.Button();
            this.btnImages = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tbPasswords = new System.Windows.Forms.TextBox();
            this.tbLogin = new System.Windows.Forms.TextBox();
            this.btnSaveTemplate = new System.Windows.Forms.Button();
            this.ofdLoadPrice = new System.Windows.Forms.OpenFileDialog();
            this.SuspendLayout();
            // 
            // tbKeywords
            // 
            this.tbKeywords.Location = new System.Drawing.Point(11, 316);
            this.tbKeywords.Margin = new System.Windows.Forms.Padding(2);
            this.tbKeywords.Name = "tbKeywords";
            this.tbKeywords.Size = new System.Drawing.Size(523, 20);
            this.tbKeywords.TabIndex = 9;
            // 
            // tbDescription
            // 
            this.tbDescription.Location = new System.Drawing.Point(11, 293);
            this.tbDescription.Margin = new System.Windows.Forms.Padding(2);
            this.tbDescription.Name = "tbDescription";
            this.tbDescription.Size = new System.Drawing.Size(523, 20);
            this.tbDescription.TabIndex = 8;
            // 
            // tbTitle
            // 
            this.tbTitle.Location = new System.Drawing.Point(11, 271);
            this.tbTitle.Margin = new System.Windows.Forms.Padding(2);
            this.tbTitle.Name = "tbTitle";
            this.tbTitle.Size = new System.Drawing.Size(523, 20);
            this.tbTitle.TabIndex = 7;
            // 
            // rtbFullText
            // 
            this.rtbFullText.Location = new System.Drawing.Point(11, 141);
            this.rtbFullText.Margin = new System.Windows.Forms.Padding(2);
            this.rtbFullText.Name = "rtbFullText";
            this.rtbFullText.Size = new System.Drawing.Size(523, 130);
            this.rtbFullText.TabIndex = 6;
            this.rtbFullText.Text = "";
            // 
            // rtbMiniText
            // 
            this.rtbMiniText.Location = new System.Drawing.Point(11, 11);
            this.rtbMiniText.Margin = new System.Windows.Forms.Padding(2);
            this.rtbMiniText.Name = "rtbMiniText";
            this.rtbMiniText.Size = new System.Drawing.Size(523, 130);
            this.rtbMiniText.TabIndex = 5;
            this.rtbMiniText.Text = "";
            // 
            // btnPrice
            // 
            this.btnPrice.Location = new System.Drawing.Point(539, 12);
            this.btnPrice.Name = "btnPrice";
            this.btnPrice.Size = new System.Drawing.Size(145, 23);
            this.btnPrice.TabIndex = 10;
            this.btnPrice.Text = "Обработать прайс";
            this.btnPrice.UseVisualStyleBackColor = true;
            this.btnPrice.Click += new System.EventHandler(this.btnPrice_Click);
            // 
            // btnImages
            // 
            this.btnImages.Location = new System.Drawing.Point(539, 41);
            this.btnImages.Name = "btnImages";
            this.btnImages.Size = new System.Drawing.Size(145, 23);
            this.btnImages.TabIndex = 11;
            this.btnImages.Text = "Обработать картинки";
            this.btnImages.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(539, 106);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 13);
            this.label2.TabIndex = 16;
            this.label2.Text = "Пароль:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(539, 67);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 15;
            this.label1.Text = "Логин:";
            // 
            // tbPasswords
            // 
            this.tbPasswords.Location = new System.Drawing.Point(539, 122);
            this.tbPasswords.Name = "tbPasswords";
            this.tbPasswords.Size = new System.Drawing.Size(145, 20);
            this.tbPasswords.TabIndex = 14;
            this.tbPasswords.UseSystemPasswordChar = true;
            // 
            // tbLogin
            // 
            this.tbLogin.Location = new System.Drawing.Point(539, 83);
            this.tbLogin.Name = "tbLogin";
            this.tbLogin.Size = new System.Drawing.Size(145, 20);
            this.tbLogin.TabIndex = 13;
            // 
            // btnSaveTemplate
            // 
            this.btnSaveTemplate.Location = new System.Drawing.Point(539, 316);
            this.btnSaveTemplate.Margin = new System.Windows.Forms.Padding(2);
            this.btnSaveTemplate.Name = "btnSaveTemplate";
            this.btnSaveTemplate.Size = new System.Drawing.Size(145, 20);
            this.btnSaveTemplate.TabIndex = 17;
            this.btnSaveTemplate.Text = "Сохранить шаблон";
            this.btnSaveTemplate.UseVisualStyleBackColor = true;
            this.btnSaveTemplate.Click += new System.EventHandler(this.btnSaveTemplate_Click);
            // 
            // ofdLoadPrice
            // 
            this.ofdLoadPrice.FileName = "ofdOpenPrice";
            this.ofdLoadPrice.Filter = "Excel|*.xlsx";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(691, 346);
            this.Controls.Add(this.btnSaveTemplate);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbPasswords);
            this.Controls.Add(this.tbLogin);
            this.Controls.Add(this.btnImages);
            this.Controls.Add(this.btnPrice);
            this.Controls.Add(this.tbKeywords);
            this.Controls.Add(this.tbDescription);
            this.Controls.Add(this.tbTitle);
            this.Controls.Add(this.rtbFullText);
            this.Controls.Add(this.rtbMiniText);
            this.Name = "Form1";
            this.Text = "w-motors";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbKeywords;
        private System.Windows.Forms.TextBox tbDescription;
        private System.Windows.Forms.TextBox tbTitle;
        private System.Windows.Forms.RichTextBox rtbFullText;
        private System.Windows.Forms.RichTextBox rtbMiniText;
        private System.Windows.Forms.Button btnPrice;
        private System.Windows.Forms.Button btnImages;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbPasswords;
        private System.Windows.Forms.TextBox tbLogin;
        private System.Windows.Forms.Button btnSaveTemplate;
        private System.Windows.Forms.OpenFileDialog ofdLoadPrice;
    }
}

