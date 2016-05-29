using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Data.OleDb;

namespace Del3
{
    public partial class Form1 : Form
    {
        Form form2;
        bool boo = false;
        public Form1()
        {
            InitializeComponent();

            Form form2= new Form();//показываем новую форму
            //Thread th=new Thread(()=>{Form1})
            //this.
            form2.Show();
            

            List<string> comboList =new List<string>();
             comboList.Add("выберите пользователя");
            comboList.Add("Администратор");
            comboList.Add("Гость-Покупатель");

            ComboBox Combo = new ComboBox();
            Combo.DataSource=comboList;
            Combo.Location = new Point(80, 40);//координаты
            Combo.Width = 150;
            Combo.SelectedIndexChanged+= (object sender, EventArgs e) =>{
                if (Combo.SelectedIndex == 1) 
                { 
                    admin a = new admin(this, dataGridView1, contextMenuStrip1,button1);
                    textBox1.Visible = false;
                    textBox2.Visible = false;
                    label1.Visible = false;
                    label2.Visible = false;


                    this.Visible = true;
                    boo = true;
                    form2.Visible = false;
                }
                if (Combo.SelectedIndex == 2)
                {
                    user b = new user(this, dataGridView1, textBox1,textBox2);
                    button1.Visible = false;
                    dataGridView1.ReadOnly = true;


                    this.Visible = true;
                    boo = true;
                    form2.Visible = false;
                }
            };

            form2.Controls.Add(Combo);
        }

        private void Form1_Activated(object sender, EventArgs e)
        {
            if (boo != true)
                this.Visible = false;
        }
    }
    public class human
    {
        protected OleDbDataAdapter adapter = new OleDbDataAdapter();
        protected DataSet ds = new DataSet();
        protected OleDbConnection cn;
        protected List<Control> FilterControls;
        protected bool[,] IndicesUnvisibleRowForFiltercontrol;

        protected void GetNewTable(string tablName)
        {
            OleDbDataAdapter adapter = new OleDbDataAdapter("SELECT *  FROM " + tablName, cn);
            ds.Tables.Add();
            adapter.Fill(ds.Tables[ds.Tables.Count - 1]);
        }

    }
    public class user : human
    {
        DataGridView dataGridView1;

        public user(Form MyForm, DataGridView dataGridView1,  TextBox textBox1, TextBox textBox2)
        {
            this.dataGridView1 = dataGridView1;



            cn = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0 ;Data Source=base2_1.mdb;");//Microsoft.Jet.OleDb.4.0base.mdb db   , цена,ЦП.Имя ////Компьютеры.Код, Компьютеры.Имя, Компьютеры.цена, Компьютеры.ЦП,Компьютеры.Видеокарта  ,  Компьютеры.Материнка
            adapter.SelectCommand = new OleDbCommand(@"SELECT   *
                                                       FROM    Компьютеры   ", cn);     //INNER JOIN Материнка ON Компьютеры.Материнка =Материнка.код 

            adapter.Fill(ds);
            dataGridView1.DataSource = ds.Tables[0];
            //////////////////////////////////////////////////////////ЗАМЕНЯЕМ последние СТОЛБЦЫ НА ComboBox-ы////////////////////////
            DataGridViewComboBoxColumn ComboBoxColumn;
            string[] arrayStr = new string[3];
            int a = 0;//С какого начинается столбцы с джоином3
            arrayStr[0] = "Код"; arrayStr[1] = "Имя"; arrayStr[2] = "Цена";
            for (int i = 0; i < arrayStr.Length; i++)
            {
                if (dataGridView1.Columns.Contains(arrayStr[i]))
                    a++;
            }
            int x = dataGridView1.Columns.Count - a;//-3 ск-ко всего таких столбов
            for (int i = 0; i < x; i++)
            {
                string thisColumnName = dataGridView1.Columns[a].Name;// нужный нам столб всегда после "ЦЕНЫ" т.е третий - a=3
                int thisDatasetColumnIndex = dataGridView1.Columns[i + a].Index;
                GetNewTable(ds.Tables[0].Columns[thisDatasetColumnIndex].ColumnName);
                ComboBoxColumn = new DataGridViewComboBoxColumn();
                ComboBoxColumn.DataSource = ds.Tables[i + 1];
                dataGridView1.Columns.Remove(thisColumnName);//Удаляем колонку 3-ю(см. выше почему)
                ComboBoxColumn.Name = thisColumnName;
                ComboBoxColumn.DataPropertyName = thisColumnName;
                ComboBoxColumn.DisplayMember = "имя";
                ComboBoxColumn.ValueMember = "код";//чему должно соответствовать эти значения комбобокса из БД
                ComboBoxColumn.MinimumWidth = 80;
                dataGridView1.Columns.Add(ComboBoxColumn);
            }
            //select count(*) from INFORMATION_SCHEMA.TABLES
            //dataGridView1.AutoSize = true;



            //Дано: у нас уже есть таблица!!!!
            ///////ищем (x,y)-координаты для CheckedListBox-ов/////////////////////////////////////////////////
            x = dataGridView1.Location.X;//
            var y = dataGridView1.Location.Y;
            var width = dataGridView1.Width;//ширина таблицы
            x = x + width + 20;//20-промежуток между контролами

            //TextBox[] abvg = new TextBox[3];
            //CheckedListBox[] abvgd = new CheckedListBox[3];
            FilterControls = new List<Control>();//массив наших фильтров

            for (int i = 0; i < 3; i++)
            {
                FilterControls.Add(new CheckedListBox());
                MyForm.Controls.Add(FilterControls[i]);
                FilterControls[i].Location = new System.Drawing.Point(x, y + 30);//x,y

                (FilterControls[i] as CheckedListBox).DataSource = ds.Tables[i + 1];
                (FilterControls[i] as CheckedListBox).DisplayMember = "имя";
                //(FilterControls[i] as CheckedListBox).ValueMember = "код";
                //(FilterControls[i] as CheckedListBox).Width = 200;
                (FilterControls[i] as CheckedListBox).CheckOnClick = true;//чтоб сразу чекал бокс а не после 2-ой попытки
                //(FilterControls[i] as CheckedListBox).Size = (FilterControls[i] as CheckedListBox).PreferredSize;

                (FilterControls[i] as CheckedListBox).SelectedIndexChanged += (object sender, EventArgs e) =>
                {

                    var CheckedIndicesForCheckedListBox = (sender as CheckedListBox).CheckedIndices;
                    int IndexOfFilterControls = FilterControls.IndexOf((sender as Control));

                    if (CheckedIndicesForCheckedListBox.Count != 0)
                    {
                        if (IndicesUnvisibleRowForFiltercontrol == null)//инициальзируем нашу булеву матриуц
                        {
                            int t = 0;
                            for (int z = 0; z < FilterControls.Count; z++)
                            {
                                if (t < (FilterControls[z] as CheckedListBox).Items.Count)
                                    t = (FilterControls[z] as CheckedListBox).Items.Count;//ещё цена же
                            }
                            IndicesUnvisibleRowForFiltercontrol = new bool[ds.Tables[0].Rows.Count + 10, t + 1];//всен равны false//проверено опытно
                        }
                        for (int j = 0; j < ds.Tables[0].Rows.Count; j++)//определяем ячейки матрицы в каждой строке (строк столько же ско-ко и строк в таблице главной)
                        {
                            //bool unVisibleRow = false;
                            for (int k = 0; k < CheckedIndicesForCheckedListBox.Count; k++)
                            {
                                if (ds.Tables[0].Rows[j].ItemArray[3 + IndexOfFilterControls].ToString() != (CheckedIndicesForCheckedListBox[k] + 1).ToString())
                                    IndicesUnvisibleRowForFiltercontrol[j, IndexOfFilterControls] = true;
                                else
                                {
                                    IndicesUnvisibleRowForFiltercontrol[j, IndexOfFilterControls] = false;
                                    break;
                                }
                            }

                            for (int k = 0; k < FilterControls.Count + 2; k++)//ещё текст же    ////смотрим в "булеву матрицу" и считаем кто остаётся видимый а кто нет
                            {
                                dataGridView1.CurrentCell = null;
                                if (IndicesUnvisibleRowForFiltercontrol[j, k] == true)
                                {
                                    dataGridView1.Rows[j].Visible = false;
                                    break;
                                }
                                else
                                    dataGridView1.Rows[j].Visible = true;
                            }
                        }
                    }
                };
                y = y + 100;
            }
            textBox1.Leave += TEXTBoxS_Leave;
            textBox2.Leave += TEXTBoxS_Leave;
        }

        void TEXTBoxS_Leave(object sender, EventArgs e)//потеря фокусаи текстовых фильтров
        {
            var TextobjectSender = (sender as TextBox);
            if (IndicesUnvisibleRowForFiltercontrol == null)
            {
                int t = 0;
                for (int z = 0; z < FilterControls.Count; z++)
                {
                    if (t < (FilterControls[z] as CheckedListBox).Items.Count)
                        t = (FilterControls[z] as CheckedListBox).Items.Count;//ещё цена же
                }
                IndicesUnvisibleRowForFiltercontrol = new bool[ds.Tables[0].Rows.Count + 10, t + 1];//всен равны false//проверено опытно
            }

            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                var dsFiltersStrParam = Convert.ToInt32(ds.Tables[0].Rows[i].ItemArray[2]);
                //TextobjectSender.Tag=TextobjectSender.Tag.ToString()
                //if (TextobjectSender.Text.IndexOf(
                //////////////валидация
                if (TextobjectSender.Text == "") break;//на пустую строку
                bool Tobreak = false;
                for (int k = 0; k < TextobjectSender.Text.Length; k++)//на НЕчисла
                    if (!Char.IsDigit(TextobjectSender.Text[k]))
                        Tobreak = true;
                if (Tobreak)//если не числа то не фильтруем
                {
                    MessageBox.Show("Вводите только цифры ");
                    break;
                }
                if (TextobjectSender.Tag.ToString() != "above")//above в переводе с англ - до
                {
                    if (dsFiltersStrParam < Int32.Parse(TextobjectSender.Text))//от
                    {
                        IndicesUnvisibleRowForFiltercontrol[i, 3] = true;//наша "булева матрица"
                    }
                    else IndicesUnvisibleRowForFiltercontrol[i, 3] = false;
                }
                else
                {
                    if (dsFiltersStrParam > Int32.Parse(TextobjectSender.Text))//до
                    {
                        IndicesUnvisibleRowForFiltercontrol[i, 4] = true;
                    }
                    else IndicesUnvisibleRowForFiltercontrol[i, 4] = false;
                }
                for (int k = 0; k < FilterControls.Count + 2; k++)//ещё текст же //
                {
                    dataGridView1.CurrentCell = null;
                    if (IndicesUnvisibleRowForFiltercontrol[i, k])
                    {
                        dataGridView1.Rows[i].Visible = false;
                        break;
                    }
                    else
                        dataGridView1.Rows[i].Visible = true;
                }
            }
        }
    }
    public class admin : human
    {
        DataGridView dataGridView1;

        public admin(Form MyForm, DataGridView dataGridView1, ContextMenuStrip contextMenuStrip1,Button button1)
        {


            cn = new OleDbConnection(@"Provider=Provider=Microsoft.ACE.OLEDB.12.0 ;Data Source=base2_1.mdb;");//base.mdb db   , цена,ЦП.Имя ////Компьютеры.Код, Компьютеры.Имя, Компьютеры.цена, Компьютеры.ЦП,Компьютеры.Видеокарта  ,  Компьютеры.Материнка
            adapter.SelectCommand = new OleDbCommand(@"SELECT   *
                                                       FROM    Компьютеры   ", cn);     //INNER JOIN Материнка ON Компьютеры.Материнка =Материнка.код 

            adapter.Fill(ds);
            dataGridView1.DataSource = ds.Tables[0];
            /* ds.Tables[0].RowChanged += (object sender, DataRowChangeEventArgs e) =>
             {
                 adapter.Update(ds);
             };*/
            /////////////////////////////////////////////////////////   
            dataGridView1.ContextMenuStrip = contextMenuStrip1;

            //////////////////////////////////////////////////////////ЗАМЕНЯЕМ последние СТОЛБЦЫ НА ComboBox-ы////////////////////////
            DataGridViewComboBoxColumn ComboBoxColumn;
            string[] arrayStr = new string[3];
            int a = 0;//С какого начинается столбцы с джоином3
            arrayStr[0] = "Код"; arrayStr[1] = "Имя"; arrayStr[2] = "Цена";
            for (int i = 0; i < arrayStr.Length; i++)
            {
                if (dataGridView1.Columns.Contains(arrayStr[i]))
                    a++;
            }
            int x = dataGridView1.Columns.Count - a;//-3 ск-ко всего таких столбов
            for (int i = 0; i < x; i++)
            {
                string thisColumnName = dataGridView1.Columns[a].Name;// нужный нам столб всегда после "ЦЕНЫ" т.е третий - a=3
                int thisDatasetColumnIndex = dataGridView1.Columns[i + a].Index;
                GetNewTable(ds.Tables[0].Columns[thisDatasetColumnIndex].ColumnName);
                ComboBoxColumn = new DataGridViewComboBoxColumn();
                ComboBoxColumn.DataSource = ds.Tables[i + 1];
                dataGridView1.Columns.Remove(thisColumnName);//Удаляем колонку 3-ю(см. выше почему)
                ComboBoxColumn.Name = thisColumnName;
                ComboBoxColumn.DataPropertyName = thisColumnName;
                ComboBoxColumn.DisplayMember = "имя";
                ComboBoxColumn.ValueMember = "код";//чему должно соответствовать эти значения комбобокса из БД
                ComboBoxColumn.MinimumWidth = 80;
                dataGridView1.Columns.Add(ComboBoxColumn);

            }

            button1.Click += button1_Click;
            dataGridView1.CellMouseEnter+= dataGridView1_CellMouseEnter ;
            contextMenuStrip1.ItemClicked += contextMenuStrip1_ItemClicked;
        }

        void button1_Click(object sender, EventArgs e)
        {
            var cb = new OleDbCommandBuilder(adapter);
            //cb.QuotePrefix = "[";           cb.QuoteSuffix = "]";
            adapter.UpdateCommand = cb.GetUpdateCommand();
            adapter.InsertCommand = cb.GetInsertCommand();
            adapter.Update(ds);
        }
       
        Point point = new Point();
        public void dataGridView1_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            point.X = e.ColumnIndex;
            point.Y = e.RowIndex;
        }
        public void contextMenuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            ds.Tables[0].Rows[point.Y].Delete();
        }
    }
}
