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
    public partial class Пациенты : Form
    {
        public Пациенты()
        {
            InitializeComponent();
        }

        OleDbConnection con;    //Строка соединения с БД
        OleDbCommand SqlCom;    //Переменная для Sql запросов       
        DataTable DT;           //Таблица для хранения результатов запроса
        OleDbDataAdapter DA;    //Адаптер для заполнения таблицы после запроса      
        bool ifcon = false;     //Флаг срединения с базой данных    

        private void ShowList()
        {
            //Процедура вывода списка в таблицу DataGridView1           
            DT = new DataTable();  //Создаем заново таблицу               
            // Указываем строку запроса и привязываем к соединению
            SqlCom = new OleDbCommand("SELECT * FROM [Пациенты] ORDER BY [Фамилия]", con);
            SqlCom.ExecuteNonQuery();
            DA = new OleDbDataAdapter(SqlCom); //Через адаптер получаем результаты запроса
            DA.Fill(DT); // Заполняем таблицу результами
            DataGridView1.DataSource = DT;  //Привязываем DataGridView1 к источнику данных             
            DataGridView1.Columns[0].Visible = false; //Столбец с ID невидимый для пользователя            
        }

        private void ClearAll()
        {
            // Процедура очистки текстовых полей
            TextBox1.Clear();
            TextBox2.Clear();
            TextBox3.Clear();
            TextBox4.Clear();            
            TextBox6.Clear();
        }

        private bool IfNull()
        {
            //Функция обнаружения пустых полей
            if ((TextBox1.Text.Trim() == "") || (TextBox2.Text.Trim() == "") || (TextBox3.Text.Trim() == "") || (TextBox4.Text.Trim() == ""))
            {
                return true;   //Имеются пустые поля           
            }
            else
            {
                //Пустые поля отсутствуют
                int g;
                if (Int32.TryParse(TextBox4.Text, out g))
                {
                    if ((Convert.ToInt32(TextBox4.Text) > 1900)&& (Convert.ToInt32(TextBox4.Text) <= DateTime.Today.Year))
                        return false; //Год рождения - корректное число!
                    else
                    {
                        MessageBox.Show("Некорректный год рождения!", "КОНТРОЛЬ ДАННЫХ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return true; //Некорректный год рождения
                        
                    }
                }
                else
                {
                    MessageBox.Show("Год рождения - не число!", "КОНТРОЛЬ ДАННЫХ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return true;   //Год рождения - не число!                   
                }                         
            }
        }

        private void Пациенты_Load(object sender, EventArgs e)
        {
            try
            {
                con = new OleDbConnection("Provider=Microsoft.ACE.Oledb.12.0;Data Source=Policlinica.accdb");
                con.Open();     //Открыть базу данных
                ifcon = true;   //Флаг поднят. Соединение с базой данных прошло успешно.          
                ShowList();     //Вызов процедуры вывода списка
            }
            catch (System.Exception err)
            {
                MessageBox.Show(err.Message, "ОШИБКА ДОСТУПА К БАЗЕ ДАННЫХ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Пациенты_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (ifcon) con.Close(); //Закрыть базу данных, если она была успешно открыта
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            // Добавить
            if (!IfNull())  //Вызов функции проверки полей на пустые значения
            {
                //Пустые поля отсутствуют
                OleDbCommand SqlCom1 = new OleDbCommand();
                SqlCom1.CommandText = "INSERT INTO [Пациенты] (Фамилия, Имя, Отчество, Год_рождения) VALUES(@fam, @im, @ot, @gr)";
                SqlCom1.Parameters.Clear();
                SqlCom1.Parameters.AddWithValue("@fam", TextBox1.Text);
                SqlCom1.Parameters.AddWithValue("@im", TextBox2.Text);
                SqlCom1.Parameters.AddWithValue("@ot", TextBox3.Text);
                SqlCom1.Parameters.AddWithValue("@gr", TextBox4.Text);
                SqlCom1.Connection = con;
                SqlCom1.ExecuteScalar(); //Выполняем запрос
                ShowList();
                MessageBox.Show("Запись добавлена.", "ДОБАВЛЕНИЕ ЗАПИСИ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
                MessageBox.Show("Некорректные данные!", "КОНТРОЛЬ ДАННЫХ", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        private void Button2_Click(object sender, EventArgs e)
        {
            //  Изменить
            if (TextBox6.Text.Trim() == "")
                MessageBox.Show("Выберите строку в таблице или нажмите ДОБАВИТЬ.",
                    "ОШИБКА В ОПЕРАЦИИ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                if (!IfNull())
                {
                    OleDbCommand SqlCom1 = new OleDbCommand();
                    SqlCom1.CommandText = "UPDATE [Пациенты] SET Фамилия = @fam, Имя = @im, Отчество = @ot, " +
                        "Год_рождения = @gr WHERE Код_пациента = @id";
                    SqlCom1.Parameters.Clear();
                    SqlCom1.Parameters.AddWithValue("@fam", TextBox1.Text);
                    SqlCom1.Parameters.AddWithValue("@im", TextBox2.Text);
                    SqlCom1.Parameters.AddWithValue("@ot", TextBox3.Text);
                    SqlCom1.Parameters.AddWithValue("@sp", TextBox4.Text);
                    SqlCom1.Parameters.AddWithValue("@id", TextBox6.Text);
                    SqlCom1.Connection = con;
                    SqlCom1.ExecuteScalar(); //Выполняем запрос
                    ShowList();
                    MessageBox.Show("Изменения сохранены.", "ИЗМЕНЕНИЕ ЗАПИСИ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                    MessageBox.Show("Некорректные данные!", "КОНТРОЛЬ ДАННЫХ", MessageBoxButtons.OK, MessageBoxIcon.Error);            
            }
        }
        private void DataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            //Копирование строки в текстовые поля
            int i = DataGridView1.CurrentRow.Index;
            if (i >= 0)
            {
                TextBox6.Text = DataGridView1[0, i].Value.ToString();   //Скрытое поле для хранения id
                TextBox1.Text = DataGridView1[1, i].Value.ToString();    
                TextBox2.Text = DataGridView1[2, i].Value.ToString();    
                TextBox3.Text = DataGridView1[3, i].Value.ToString();    
                TextBox4.Text = DataGridView1[4, i].Value.ToString();                     
             }       
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            //Удалить
            if (TextBox6.Text.Trim() == "")
            {            MessageBox.Show("Выберите строку для удаления!",
                "ОШИБКА В ОПЕРАЦИИ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                OleDbCommand SqlCom1 = new OleDbCommand();
                SqlCom1.CommandText = "DELETE FROM [Пациенты] WHERE Код_пациента = @id";
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
            // Очистить все текстовые поля
            ClearAll();
        }
    }
}
