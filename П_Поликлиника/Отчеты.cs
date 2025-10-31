using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
//-------------- Подключить библиотеку для работы с БД ---------------------------------
using System.Data.OleDb;

namespace П_Поликлиника
{
    public partial class Отчеты : Form
    {
        public Отчеты()
        {
            InitializeComponent();
        }
        OleDbConnection con; 
        OleDbCommand SqlCom;
        OleDbDataReader dataReader;
        OleDbDataReader dataReader2;
        bool ifcon = false;     //Флаг соединения с базой данных  
        bool diagram = true;

        private void Button1_Click(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            //В этой таблице заказываем две колонки "Врач" и "Объем услуг":
            dt.Columns.Add("Врач");
            dt.Columns.Add("Объем услуг");
            DateTime dat1 = dateTimePicker1.Value;
            DateTime dat2 = dateTimePicker2.Value;
            //Приведение дат к формату dd.mm.yyyy
            //Дата 1
            int d1 = dat1.Day;
            int m1 = dat1.Month;
            int y1 = dat1.Year;
            string dats1 = "";
            if(m1 < 10)
                dats1 = "0" + Convert.ToString(m1) + "/" + Convert.ToString(d1) + "/" + Convert.ToString(y1);
            else
                dats1 = Convert.ToString(m1) + "/" + Convert.ToString(d1) + "/" + Convert.ToString(y1);
        
            dats1 = "#" + dats1 + "#";
            
            //Дата 2
            int d2 = dat2.Day;
            int m2 = dat2.Month;
            int y2 = dat2.Year;
            string dats2 = "";
            if(m2 < 10)
                dats2 = "0" + Convert.ToString(m2) + "/" + Convert.ToString(d2) + "/" + Convert.ToString(y2);
            else
                dats2 = Convert.ToString(m2) + "/" + Convert.ToString(d2) + "/" + Convert.ToString(y2);
        
            dats2 = "#" + dats2 + "#";             
            
            SqlCom = new OleDbCommand("SELECT Код_врача, SUM(Стоимость) FROM Обращения " + 
                "WHERE Дата_обращения>= " + dats1 + " AND Дата_обращения<= " + dats2 +
                " GROUP BY Код_врача", con);
            dataReader = SqlCom.ExecuteReader();           
            long idv;
            Decimal st;
            string fio;
            OleDbCommand SqlCom2 = new OleDbCommand();            
            decimal sto = 0.0M;
            bool flag = false;
            while (dataReader.Read())
            {
                flag = true;    //Есть данные за указанный период!
                idv = Convert.ToInt64(dataReader.GetValue(0));  //Код врача
                st = Convert.ToDecimal(dataReader.GetValue(1)); //Стоимость
                sto = sto + st; //Накопление суммы
                //Поиск ФИО по коду врача
                SqlCom2 = new OleDbCommand("SELECT * FROM [Врачи] WHERE Код_врача = " + idv, con);
                dataReader2 = SqlCom2.ExecuteReader();
                dataReader2.Read();
                //Получение фамилии и инициалов врача
                fio = Convert.ToString(dataReader2.GetValue(1)) + " " + Convert.ToString(dataReader2.GetValue(2)).Substring(0, 1) + "." +
                    Convert.ToString(dataReader2.GetValue(3)).Substring(0, 1) + ".";
                dt.Rows.Add(fio, st); //Добавить строку в dt
            }
            dataReader.Close();
            label3.Visible = true;

            if (flag)   //Есть ли данные?
            {
                dataReader2.Close();
                label3.ForeColor = Color.Blue;               
                label3.Text = "ОБЩИЙ ОБЪЕМ ОКАЗАННЫХ УСЛУГ ЗА УКАЗАННЫЙ ПЕРИОД: " + Convert.ToString(sto) + " руб.";
            }
            else
            {
                label3.ForeColor = Color.DarkRed;
                label3.Text = "ОТСУТСТВУЮТ ОКАЗАННЫЕ УСЛУГИ ЗА УКАЗАННЫЙ ПЕРИОД!";
            }
            DataGridView1.DataSource = dt;  //Привязываем DataGridView1 к источнику данных dt

            //Составленную таблицу указываем в качестве источника данных:
            Chart1.DataSource = dt;
            //По горизонтальной оси откладываем названия месяцев:
            Chart1.Series["Series1"].XValueMember = "Врач";
            //'По вертикальной оси откладываем объемы продаж:
            Chart1.Series["Series1"].YValueMembers = "Объем услуг";
            //Название графика (диаграммы):
            Chart1.Titles.Clear();  //Очистка названия
            Chart1.Titles.Add("Объемы услуг по врачам");
            //Задаем тип диаграммы по умолчанию - столбиковая диаграмма:
            Chart1.Series["Series1"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Column;
            //Тип диаграммы может быть другим, например: Pie, Line и др.
            // Перечень типов диаграмм можно увидеть в свойствах chart1, далее: series\series1\Diagram\ChartType
            //Задаем цвет диаграммы - Aqua. Рекомендуются цвета: Aqua, LightGreen, Yellow, LightBlue
            Chart1.Series["Series1"].Color = Color.Aqua;                      
        }

        private void Отчеты_Load(object sender, EventArgs e)
        {
            label3.Visible = false;
            try
            {
                con = new OleDbConnection("Provider=Microsoft.ACE.Oledb.12.0;Data Source=Policlinica.accdb");
                con.Open();     //Открыть базу данных
                ifcon = true;   //Флаг поднят. Соединение с базой данных прошло успешно.                     
            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message, "ОШИБКА ДОСТУПА К БАЗЕ ДАННЫХ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }            
        }

        private void Отчеты_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (ifcon) con.Close();
        }

        private void Chart1_Click(object sender, EventArgs e)
        {
            diagram = !diagram;
                if (diagram)
                Chart1.Series["Series1"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Column;
                else
                Chart1.Series["Series1"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Pie;        
        }
        
    }
}
