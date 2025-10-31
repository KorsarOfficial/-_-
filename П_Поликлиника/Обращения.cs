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
    public partial class Обращения : Form
    {
        public Обращения()
        {
            InitializeComponent();
        }

        OleDbConnection con;    //Строка соединения с БД
        OleDbCommand SqlCom;    //Переменная для Sql-запросов
        OleDbCommand SqlComV;   //Врачи
        OleDbCommand SqlComP;   //Пациенты
        DataTable DT;           //Таблица для хранения результатов запроса
        OleDbDataAdapter DA;    // Адаптер для заполнения таблицы после запроса
        bool ifcon = false;     //Флаг срединения с базой данных  

        private void ShowVP()
        {
            //Вывод списка ФИО врачей в ComboBox1 и кодов в ComboBox2

            SqlComV = new OleDbCommand("SELECT * FROM [Врачи]", con);
            OleDbDataReader dataReaderV = SqlComV.ExecuteReader();     //Создать объект для чтения и выполнить команду
                                                                       //Таблица прочитана в dataReaderV (виртуальная таблица)
            ComboBox1.Items.Clear();    //Очистка списка ComboBox1 (ФИО врачей)
            ComboBox2.Items.Clear();    //Очистка списка ComboBox2 (Коды врачей)
            while (dataReaderV.Read())  //Пока не конец виртуальной таблицы
            {
                ComboBox1.Items.Add(dataReaderV.GetValue(1) + " " + dataReaderV.GetValue(2) + " " + dataReaderV.GetValue(3)); // Фамилия Имя Отчество врача
                ComboBox2.Items.Add(dataReaderV.GetValue(0)); //Код врача
            }
            dataReaderV.Close();    //Закрыть объект чтения

            //Вывод списка ФИО Пациентов в ComboBox3 и кодов в ComboBox4
            SqlComP = new OleDbCommand("SELECT * FROM [Пациенты]", con);
            OleDbDataReader dataReaderP = SqlComP.ExecuteReader();
            ComboBox3.Items.Clear();
            ComboBox4.Items.Clear();
            while (dataReaderP.Read())
            {
                ComboBox3.Items.Add(dataReaderP.GetValue(1) + " " + dataReaderP.GetValue(2) + " " + dataReaderP.GetValue(3)); // Фамилия Имя Отчество
                ComboBox4.Items.Add(dataReaderP.GetValue(0)); //Код пациента
            }
            dataReaderP.Close();
            //Исходная установка указателей списков
            ComboBox1.SelectedIndex = -1;
            ComboBox2.SelectedIndex = -1;
            ComboBox3.SelectedIndex = -1;
            ComboBox4.SelectedIndex = -1;
        }

        private void ShowList()
        {
            //Вывод списка в таблицу DataGridView  
            DT = new DataTable();  //Создаем заново таблицу       
            SqlCom.ExecuteNonQuery(); //Выполняем запрос
            DA = new OleDbDataAdapter(SqlCom); //Через адаптер получаем результаты запроса
            DA.Fill(DT); // Заполняем таблицу результами
            DataGridView1.DataSource = DT; // Привязываем DataGridView1 к источнику данных
            DataGridView1.Columns[0].Visible = false;    //Код обращения невидимый
            DataGridView1.Columns[10].Visible = false;  //Код врача невидимый
            DataGridView1.Columns[11].Visible = false;  //Код пациента невидимый
        }

        private void ClearAll()
        {
            // Процедура очистки текстовых полей
            TextBox1.Clear();
            TextBox2.Clear();
            TextBox6.Clear();

            ComboBox1.SelectedIndex = -1;
            ComboBox2.SelectedIndex = -1;
            ComboBox3.SelectedIndex = -1;
            ComboBox4.SelectedIndex = -1;

            DateTimePicker1.Value = DateTime.Today;
        }

        private bool IfNull()
        {
            //Функция определения некорректных данных
            int ncombo1 = ComboBox1.SelectedIndex;
            int ncombo2 = ComboBox3.SelectedIndex;
            if ((TextBox1.Text.Trim() == "") || (TextBox2.Text.Trim() == ""))
            {
                MessageBox.Show("ПУСТЫЕ ПОЛЯ НЕ ДОПУСТИМЫ!", "КОНТРОЛЬ ДАННЫХ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return true;
            }
            else
            {
                //Пустые поля отсутствуют
                if ((ncombo1 < 0) || (ncombo2 < 0))
                {
                    MessageBox.Show("Не выбран Врач или Пациент!", "КОНТРОЛЬ ДАННЫХ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return true;
                }
                else
                    return false;  //Все данные корректные!            
            }

        }

        private void Обращения_Load(object sender, EventArgs e)
        {
            //Загрузка таблицы ОБРАЩЕНИЯ
            try
            {
                con = new OleDbConnection("Provider=Microsoft.ACE.Oledb.12.0;Data Source=Policlinica.accdb");
                con.Open();     //Открыть базу данных
                ifcon = true;   //Флаг поднят. Соединение с базой данных прошло успешно.          
                ShowVP();       //Вызов процедуры формирования списков врачей и пациентов
                                // Указываем строку запроса и привязываем к соединению
                SqlCom = new OleDbCommand("SELECT Обращения.Код_обращения, Обращения.Дата_обращения, " +
                    "Врачи.Фамилия, Врачи.Имя, Врачи.Отчество, " +
                    "Пациенты.Фамилия, Пациенты.Имя, Пациенты.Отчество, " +
                    "Обращения.Диагноз, Обращения.Стоимость, " +
                    "Врачи.Код_врача, Пациенты.Код_пациента " +
                    "FROM Обращения, Врачи, Пациенты " +
                    "WHERE Обращения.Код_врача = Врачи.Код_врача AND Обращения.Код_пациента=Пациенты.Код_пациента " +
                    "ORDER BY Обращения.Дата_обращения, Врачи.Фамилия", con);
                ShowList(); //Вызов процедуры пручения списка обращений

            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message, "ОШИБКА ДОСТУПА К БАЗЕ ДАННЫХ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Обращения_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (ifcon) con.Close(); //Закрыть базу данных, если она была успешно открыта
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            // Добавить
            if (!IfNull())
            {
                int v = ComboBox1.SelectedIndex;    //Получить номер строки в списке врачей
                ComboBox2.SelectedIndex = v;        //Установить курсор на тот же номер строки в списке кодов врачей
                int idv = Convert.ToInt32(ComboBox2.SelectedItem);  //Получить код врача
                int p = ComboBox3.SelectedIndex;    //Получить номер строки в списке пациентов
                ComboBox4.SelectedIndex = p;    //Установить курсор на тот же номер строки в списке кодов пациентов
                int idp = Convert.ToInt32(ComboBox4.SelectedItem);  //Получить код пациента
                DateTime dat = DateTimePicker1.Value;
                //Приведение даты к формату dd.mm.yyyy
                int d = dat.Day;
                int m = dat.Month;
                int y = dat.Year;
                string dats = "";
                if (m < 10)
                    dats = Convert.ToString(d) + ".0" + Convert.ToString(m) + "." + Convert.ToString(y);
                else
                    dats = Convert.ToString(d) + "." + Convert.ToString(m) + "." + Convert.ToString(y);

                Decimal st = Convert.ToDecimal(TextBox2.Text);

                OleDbCommand SqlCom1 = new OleDbCommand();
                SqlCom1.CommandText = "INSERT INTO [Обращения] (Код_врача, Код_пациента, Дата_обращения, Диагноз, Стоимость) VALUES(@v, @p, @dat, @dia, @st)";
                SqlCom1.Parameters.Clear();
                SqlCom1.Parameters.AddWithValue("@v", idv);
                SqlCom1.Parameters.AddWithValue("@p", idp);
                SqlCom1.Parameters.AddWithValue("@dat", dats);
                SqlCom1.Parameters.AddWithValue("@dia", TextBox1.Text);
                SqlCom1.Parameters.AddWithValue("@st", st);
                SqlCom1.Connection = con;
                SqlCom1.ExecuteScalar(); //Выполняем запрос
                ShowList();
                ShowVP();
                //ClearAll();
                MessageBox.Show("Запись добавлена.", "ДОБАВЛЕНИЕ ЗАПИСИ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            //Изменить
            if (TextBox6.Text.Trim() == "")
                MessageBox.Show("Выберите строку в таблице или нажмите ДОБАВИТЬ.",
                    "ОШИБКА В ОПЕРАЦИИ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                if (!IfNull())
                {
                    int v = ComboBox1.SelectedIndex;    //Получить номер строки в списке врачей
                    ComboBox2.SelectedIndex = v;        //Установить курсор на тот же номер строки в списке кодов врачей
                    int idv = Convert.ToInt32(ComboBox2.SelectedItem);  //Получить код врача
                    int p = ComboBox3.SelectedIndex;    //Получить номер строки в списке пациентов
                    ComboBox4.SelectedIndex = p;    //Установить курсор на тот же номер строки в списке кодов пациентов
                    int idp = Convert.ToInt32(ComboBox4.SelectedItem);  //Получить код пациента
                    DateTime dat = DateTimePicker1.Value;
                    //Приведение даты к формату dd.mm.yyyy
                    int d = dat.Day;
                    int m = dat.Month;
                    int y = dat.Year;
                    string dats = "";
                    if (m < 10)
                        dats = Convert.ToString(d) + ".0" + Convert.ToString(m) + "." + Convert.ToString(y);
                    else
                        dats = Convert.ToString(d) + "." + Convert.ToString(m) + "." + Convert.ToString(y);

                    Decimal st = Convert.ToDecimal(TextBox2.Text);

                    OleDbCommand SqlCom1 = new OleDbCommand();
                    SqlCom1.CommandText = "UPDATE [Обращения] SET Код_врача = @v, Код_пациента = @p, Дата_обращения = @dat, " +
                        "Диагноз = @dia, Стоимость = @st WHERE Код_обращения = @id";
                    SqlCom1.Parameters.Clear();
                    SqlCom1.Parameters.AddWithValue("@v", idv);
                    SqlCom1.Parameters.AddWithValue("@p", idp);
                    SqlCom1.Parameters.AddWithValue("@dat", dats);
                    SqlCom1.Parameters.AddWithValue("@dia", TextBox1.Text);
                    SqlCom1.Parameters.AddWithValue("@st", st);
                    SqlCom1.Parameters.AddWithValue("@id", TextBox6.Text);
                    SqlCom1.Connection = con;
                    SqlCom1.ExecuteScalar(); //Выполняем запрос
                    ShowList();
                    ShowVP();
                    // ClearAll();
                    MessageBox.Show("Изменения сохранены.", "ИЗМЕНЕНИЕ ЗАПИСИ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            //Удалить
            if (TextBox6.Text.Trim() == "")
            {
                MessageBox.Show("Выберите строку для удаления!",
                    "ОШИБКА В ОПЕРАЦИИ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                OleDbCommand SqlCom1 = new OleDbCommand();
                SqlCom1.CommandText = "DELETE FROM [Обращения] WHERE Код_обращения = @id";
                SqlCom1.Parameters.Clear();
                SqlCom1.Parameters.AddWithValue("@id", TextBox6.Text);
                SqlCom1.Connection = con;
                SqlCom1.ExecuteScalar(); //Выполняем запрос
                ShowList();
                MessageBox.Show("Запись удалена.", "УДАЛЕНИЕ ЗАПИСИ",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            ClearAll();
        }

        private void DataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (DataGridView1.Rows.Count > 1) //таблица не пустая! Первая строка - это заголовок
            {
                //Копирование строки DataGridView в текстовые поля
                int i = DataGridView1.CurrentRow.Index; //Номер выбранной строки в DataGridView1
                string kod = "";
                if ((i >= 0)&&(i< DataGridView1.Rows.Count-1)) //Выделена строка в пределах таблицы
                {
                    //Установка курсора на нужную строку в списке врачей
                    kod = DataGridView1[10, i].Value.ToString(); //Код врача  
                    int j = 0;
                    int k = -1;
                    while (j < ComboBox2.Items.Count)
                    {                        
                        if (Convert.ToString(ComboBox2.Items[j]) == kod)
                        {
                            k = j;
                            break;
                        }
                        j = j + 1;
                    }
                    ComboBox1.SelectedIndex = k;
                    // Установка курсора на нужную строку в списке пациентов
                    kod = DataGridView1[11, i].Value.ToString(); //Код пациента
                    j = 0;
                    k = -1;
                    while (j < ComboBox4.Items.Count)
                    {                      
                        if (Convert.ToString(ComboBox4.Items[j]) == kod)
                        {
                            k = j;
                            break;
                        }
                        j = j + 1;
                    }
                    ComboBox3.SelectedIndex = k;
                    //Установка курсора на нужную дату в календаре
                    DateTimePicker1.Value = Convert.ToDateTime(DataGridView1[1, i].Value);
                    TextBox6.Text = DataGridView1[0, i].Value.ToString();    //Скрытое поле для хранения id обращения
                    TextBox1.Text = DataGridView1[8, i].Value.ToString();   //Поле диагноза 
                    TextBox2.Text = DataGridView1[9, i].Value.ToString();   //Поле стоимости
                }
            }
            else
            {
                MessageBox.Show("Таблица пустая!", "РАБОТА С ТАБЛИЦЕЙ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
     
}
