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
    public partial class Врачи : Form
    {
        public Врачи()
        {
            InitializeComponent();
        }
        OleDbConnection con;    //Строка соединения с БД
        OleDbCommand SqlCom;    //Переменная для Sql запросов       
        DataTable DT;           //Таблица для хранения результатов запроса
        OleDbDataAdapter DA;    //Адаптер для заполнения таблицы после запроса      
        bool ifcon = false;     //Флаг соединения с базой данных    

        private void ShowList()
        {
            //Процедура вывода списка в таблицу DataGridView1           
            DT = new DataTable();  //Создаем заново таблицу               
            // Указываем строку запроса и привязываем к соединению
            SqlCom = new OleDbCommand("SELECT * FROM [Врачи] ORDER BY [Фамилия]", con);
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
            TextBox5.Clear();
            TextBox6.Clear();
        }
        private bool IfNull()
        {
            //Функция обнаружения пустых полей
            if ((TextBox1.Text.Trim() == "") || (TextBox2.Text.Trim() == "") || (TextBox3.Text.Trim() == "") ||
                (TextBox4.Text.Trim() == "") || (TextBox5.Text.Trim() == ""))
            {             
                return true;   //Имеются пустые поля           
            }
            else
                return false;  //Пустые поля отсутствуют         
        }        
        private void Врачи_Load(object sender, EventArgs e)
        {
            try
            {
                con = new OleDbConnection("Provider=Microsoft.ACE.Oledb.12.0;Data Source=Policlinica.accdb");                        
                con.Open();     //Открыть базу данных
                ifcon = true;   //Флаг поднят. Соединение с базой данных прошло успешно.          
                ShowList();     //Вызов процедуры вывода списка
            }
            catch(System.Exception err)
            {
                MessageBox.Show(err.Message, "ОШИБКА ДОСТУПА К БАЗЕ ДАННЫХ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Врачи_FormClosing(object sender, FormClosingEventArgs e)
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
                SqlCom1.CommandText = "INSERT INTO [Врачи] (Фамилия, Имя, Отчество, Специальность, Категория) "+ 
                    "VALUES(@fam, @im, @ot, @sp, @kat)";
                SqlCom1.Parameters.Clear(); //Очистка параметров вызова
                SqlCom1.Parameters.AddWithValue("@fam", TextBox1.Text); //Первый параметр находится в текстовом поле TextBox1.Text
                SqlCom1.Parameters.AddWithValue("@im", TextBox2.Text);  //Второй параметр находится в текстовом поле TextBox2.Text
                SqlCom1.Parameters.AddWithValue("@ot", TextBox3.Text);  //Третий параметр находится в текстовом поле TextBox3.Text
                SqlCom1.Parameters.AddWithValue("@sp", TextBox4.Text);  //Четвертый параметр находится в текстовом поле TextBox4.Text
                SqlCom1.Parameters.AddWithValue("@kat", TextBox5.Text); //Пятый параметр находится в текстовом поле TextBox5.Text
                SqlCom1.Connection = con;
                SqlCom1.ExecuteScalar(); //Выполняем запрос
                ShowList();
                MessageBox.Show("Запись добавлена.", "ДОБАВЛЕНИЕ ЗАПИСИ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
                MessageBox.Show("ПУСТЫЕ ПОЛЯ НЕ ДОПУСТИМЫ!", "КОНТРОЛЬ ДАННЫХ", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    SqlCom1.CommandText = "UPDATE [Врачи] SET Фамилия = @fam, Имя = @im, Отчество = @ot, " + 
                        "Специальность = @sp, Категория = @kat WHERE Код_врача = @id";
                    SqlCom1.Parameters.Clear();
                    SqlCom1.Parameters.AddWithValue("@fam", TextBox1.Text);
                    SqlCom1.Parameters.AddWithValue("@im", TextBox2.Text);
                    SqlCom1.Parameters.AddWithValue("@ot", TextBox3.Text);
                    SqlCom1.Parameters.AddWithValue("@sp", TextBox4.Text);
                    SqlCom1.Parameters.AddWithValue("@kat", TextBox5.Text);
                    SqlCom1.Parameters.AddWithValue("@id", TextBox6.Text);
                    SqlCom1.Connection = con;
                    SqlCom1.ExecuteScalar(); //Выполняем запрос
                    ShowList();
                    MessageBox.Show("Изменения сохранены.", "ИЗМЕНЕНИЕ ЗАПИСИ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else 
                    MessageBox.Show("ПУСТЫЕ ПОЛЯ НЕ ДОПУСТИМЫ!", "КОНТРОЛЬ ДАННЫХ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            //Копирование строки в текстовые поля
            int i = DataGridView1.CurrentRow.Index;
            if (i >= 0)
            {
                TextBox6.Text = DataGridView1[0, i].Value.ToString();   //Скрытое поле для хранения id
                TextBox1.Text = DataGridView1[1, i].Value.ToString();   //Фамилия
                TextBox2.Text = DataGridView1[2, i].Value.ToString();   //Имя
                TextBox3.Text = DataGridView1[3, i].Value.ToString();   //Отчество
                TextBox4.Text = DataGridView1[4, i].Value.ToString();   //Специальность
                TextBox5.Text = DataGridView1[5, i].Value.ToString();   //Категория
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
                SqlCom1.CommandText = "DELETE FROM [Врачи] WHERE Код_врача = @id";
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
