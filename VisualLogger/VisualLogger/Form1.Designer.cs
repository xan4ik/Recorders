
namespace VisualLogger
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
            this.components = new System.ComponentModel.Container();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.recordButton = new System.Windows.Forms.Button();
            this.stop = new System.Windows.Forms.Button();
            this.pathButton = new System.Windows.Forms.Button();
            this.setBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.comBox = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(200, 12);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(300, 287);
            this.textBox1.TabIndex = 0;
            // 
            // recordButton
            // 
            this.recordButton.Location = new System.Drawing.Point(12, 247);
            this.recordButton.Name = "recordButton";
            this.recordButton.Size = new System.Drawing.Size(145, 23);
            this.recordButton.TabIndex = 1;
            this.recordButton.Text = "Начать запись";
            this.recordButton.UseVisualStyleBackColor = true;
            this.recordButton.Click += new System.EventHandler(this.recordButton_Click);
            // 
            // stop
            // 
            this.stop.Enabled = false;
            this.stop.Location = new System.Drawing.Point(12, 276);
            this.stop.Name = "stop";
            this.stop.Size = new System.Drawing.Size(145, 23);
            this.stop.TabIndex = 2;
            this.stop.Text = "Закончить";
            this.stop.UseVisualStyleBackColor = true;
            this.stop.Click += new System.EventHandler(this.stop_Click);
            // 
            // pathButton
            // 
            this.pathButton.Location = new System.Drawing.Point(12, 115);
            this.pathButton.Name = "pathButton";
            this.pathButton.Size = new System.Drawing.Size(145, 23);
            this.pathButton.TabIndex = 3;
            this.pathButton.Text = "Выбрать путь";
            this.pathButton.UseVisualStyleBackColor = true;
            this.pathButton.Click += new System.EventHandler(this.pathButton_Click);
            // 
            // setBox
            // 
            this.setBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.setBox.FormattingEnabled = true;
            this.setBox.Items.AddRange(new object[] {
            "accuracy",
            "density",
            "medium",
            "default"});
            this.setBox.Location = new System.Drawing.Point(12, 33);
            this.setBox.Name = "setBox";
            this.setBox.Size = new System.Drawing.Size(145, 21);
            this.setBox.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(105, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Настройки камеры";
            // 
            // comBox
            // 
            this.comBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comBox.FormattingEnabled = true;
            this.comBox.Items.AddRange(new object[] {
            "COM1"});
            this.comBox.Location = new System.Drawing.Point(12, 77);
            this.comBox.Name = "comBox";
            this.comBox.Size = new System.Drawing.Size(145, 21);
            this.comBox.TabIndex = 9;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 61);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Порт GNSS";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.PaleTurquoise;
            this.ClientSize = new System.Drawing.Size(512, 308);
            this.Controls.Add(this.comBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.setBox);
            this.Controls.Add(this.pathButton);
            this.Controls.Add(this.stop);
            this.Controls.Add(this.recordButton);
            this.Controls.Add(this.textBox1);
            this.Name = "Form1";
            this.Text = "Запись";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button recordButton;
        private System.Windows.Forms.Button stop;
        private System.Windows.Forms.Button pathButton;
        private System.Windows.Forms.ComboBox setBox;
        private System.Windows.Forms.Label label1;
        private System.IO.Ports.SerialPort serialPort1;
        private System.Windows.Forms.ComboBox comBox;
        private System.Windows.Forms.Label label3;
    }
}

