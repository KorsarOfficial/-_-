namespace П_Поликлиника
{
    partial class Главная_форма
    {
        /// <summary>
        /// Требуется переменная конструктора.
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
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.MenuStrip1 = new System.Windows.Forms.MenuStrip();
            this.ВрачиToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ПациентыToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ОбращенияToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ОтчетыToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ВыходToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // MenuStrip1
            // 
            this.MenuStrip1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.MenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ВрачиToolStripMenuItem,
            this.ПациентыToolStripMenuItem,
            this.ОбращенияToolStripMenuItem,
            this.ОтчетыToolStripMenuItem,
            this.ВыходToolStripMenuItem});
            this.MenuStrip1.Location = new System.Drawing.Point(0, 0);
            this.MenuStrip1.Name = "MenuStrip1";
            this.MenuStrip1.Padding = new System.Windows.Forms.Padding(8, 2, 0, 2);
            this.MenuStrip1.Size = new System.Drawing.Size(446, 25);
            this.MenuStrip1.TabIndex = 1;
            this.MenuStrip1.Text = "MenuStrip1";
            // 
            // ВрачиToolStripMenuItem
            // 
            this.ВрачиToolStripMenuItem.Name = "ВрачиToolStripMenuItem";
            this.ВрачиToolStripMenuItem.Size = new System.Drawing.Size(56, 21);
            this.ВрачиToolStripMenuItem.Text = "Врачи";
                
            // 
            // ПациентыToolStripMenuItem
            // 
            this.ПациентыToolStripMenuItem.Name = "ПациентыToolStripMenuItem";
            this.ПациентыToolStripMenuItem.Size = new System.Drawing.Size(78, 21);
            this.ПациентыToolStripMenuItem.Text = "Пациенты";
            this.ПациентыToolStripMenuItem.Click += new System.EventHandler(this.ПациентыToolStripMenuItem_Click);
            // 
            // ОбращенияToolStripMenuItem
            // 
            this.ОбращенияToolStripMenuItem.Name = "ОбращенияToolStripMenuItem";
            this.ОбращенияToolStripMenuItem.Size = new System.Drawing.Size(92, 21);
            this.ОбращенияToolStripMenuItem.Text = "Обращения";
            this.ОбращенияToolStripMenuItem.Click += new System.EventHandler(this.ОбращенияToolStripMenuItem_Click);
            // 
            // ОтчетыToolStripMenuItem
            // 
            this.ОтчетыToolStripMenuItem.Name = "ОтчетыToolStripMenuItem";
            this.ОтчетыToolStripMenuItem.Size = new System.Drawing.Size(63, 21);
            this.ОтчетыToolStripMenuItem.Text = "Отчеты";
            this.ОтчетыToolStripMenuItem.Click += new System.EventHandler(this.ОтчетыToolStripMenuItem_Click);
            // 
            // ВыходToolStripMenuItem
            // 
            this.ВыходToolStripMenuItem.Name = "ВыходToolStripMenuItem";
            this.ВыходToolStripMenuItem.Size = new System.Drawing.Size(57, 21);
            this.ВыходToolStripMenuItem.Text = "Выход";
            this.ВыходToolStripMenuItem.Click += new System.EventHandler(this.ВыходToolStripMenuItem_Click);
            // 
            // Главная_форма
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(446, 213);
            this.Controls.Add(this.MenuStrip1);
            this.Name = "Главная_форма";
            this.Text = "ПЛАТНАЯ ПОЛИКЛИНИКА";
            this.MenuStrip1.ResumeLayout(false);
            this.MenuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.MenuStrip MenuStrip1;
        internal System.Windows.Forms.ToolStripMenuItem ВрачиToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem ПациентыToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem ОбращенияToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem ОтчетыToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem ВыходToolStripMenuItem;
    }
}

