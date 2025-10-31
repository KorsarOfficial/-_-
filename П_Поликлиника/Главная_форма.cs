using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace П_Поликлиника
{
    public partial class Главная_форма : Form
    {
        public Главная_форма()
        {
            InitializeComponent();
        }
       
        private void ВрачиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Врачи f = new Врачи();
            f.Show();
        }

        private void ПациентыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Пациенты f = new Пациенты();
            f.Show();
        }

        private void ОбращенияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Обращения f = new Обращения();
            f.Show();
        }

        private void ОтчетыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Отчеты f = new Отчеты();
            f.Show();
        }

        private void ВыходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
