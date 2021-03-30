
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
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.recordButton = new System.Windows.Forms.Button();
            this.stop = new System.Windows.Forms.Button();
            this.pathButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(124, 12);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            //this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(300, 287);
            this.textBox1.TabIndex = 0;
            // 
            // recordButton
            // 
            this.recordButton.Location = new System.Drawing.Point(12, 247);
            this.recordButton.Name = "recordButton";
            this.recordButton.Size = new System.Drawing.Size(94, 23);
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
            this.stop.Size = new System.Drawing.Size(94, 23);
            this.stop.TabIndex = 2;
            this.stop.Text = "Закончить";
            this.stop.UseVisualStyleBackColor = true;
            this.stop.Click += new System.EventHandler(this.stop_Click);
            // 
            // pathButton
            // 
            this.pathButton.Location = new System.Drawing.Point(12, 13);
            this.pathButton.Name = "pathButton";
            this.pathButton.Size = new System.Drawing.Size(94, 23);
            this.pathButton.TabIndex = 3;
            this.pathButton.Text = "Выбрать путь";
            this.pathButton.UseVisualStyleBackColor = true;
            this.pathButton.Click += new System.EventHandler(this.pathButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.PaleTurquoise;
            this.ClientSize = new System.Drawing.Size(436, 308);
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
    }
}

