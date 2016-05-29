using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;

namespace Del3
{
    public partial class Form1 : Form
    {
        OleDbDataAdapter adapter = new OleDbDataAdapter();
        DataSet ds = new DataSet();
        OleDbConnection cn;
        public Form1()
        {
            InitializeComponent();
            cn = new OleDbConnection(@"Provider=Microsoft.Jet.OleDb.4.0 ;Data Source=base2_1.mdb;");//base.mdb db   , цена,ЦП.Имя ////Компьютеры.Код, Компьютеры.Имя, Компьютеры.цена, Компьютеры.ЦП,Компьютеры.Видеокарта  ,  Компьютеры.Материнка
            adapter.SelectCommand = new OleDbCommand(@"SELECT   *
                                                       FROM    Компьютеры   ", cn);     //INNER JOIN Материнка ON Компьютеры.Материнка =Материнка.код 
            
           var cb = new OleDbCommandBuilder(adapter);
           //cb.QuotePrefix = "[";           cb.QuoteSuffix = "]";
           adapter.UpdateCommand = cb.GetUpdateCommand();

            adapter.Fill(ds);
            dataGridView1.DataSource   = ds.Tables[0];
//"UPDATE Компьютеры SET Имя = ?, Цена = ?, ЦП = ? WHERE ((Код = ?) AND ((? = 1 AND Имя IS NULL) OR (Имя = ?)) AND ((? = 1 AND Цена IS NULL) OR (Цена = ?)) AND ((? = 1 AND ЦП IS NULL) OR (ЦП = ?)))"





            DataGridViewComboBoxColumn ComboBoxColumn;
            string[] arrayStr= new string[3];
            int a=0;//С какого начинается столбцы с джоином3
            arrayStr[0]="Код";                arrayStr[1]="Имя";                arrayStr[2]="Цена";
            
            for(int i =0;i<arrayStr.Length;i++)
            {
                if(dataGridView1.Columns.Contains(arrayStr[i]))
                    a++;
            }
            int x = dataGridView1.Columns.Count-a;//-3 ск-ко всего таких столбов
            for (int i = 0; i < x; i++)
            {
                string thisColumnName = dataGridView1.Columns[a].Name;// нужный нам столб всегда после "ЦЕНЫ" т.е третий
                int thisDatasetColumnIndex = dataGridView1.Columns[i+a].Index;
                Connect(ds.Tables[0].Columns[thisDatasetColumnIndex].ColumnName);
                ComboBoxColumn = new DataGridViewComboBoxColumn();
                ComboBoxColumn.DataSource = ds.Tables[i+1];
                dataGridView1.Columns.Remove(thisColumnName);//Удаляем колонку 3-ю(см. выше почему)
                ComboBoxColumn.Name = thisColumnName;
                ComboBoxColumn.DataPropertyName = thisColumnName;
                ComboBoxColumn.DisplayMember = "имя";
                ComboBoxColumn.ValueMember = "код";
                dataGridView1.Columns.Add(ComboBoxColumn);

            }
               /* dataGridView1.Columns.Remove(dataGridView1.Columns[4].Name);
          //var x=ds.Tables[0].Columns.Count;//непосредственно начинает считать с1-го а не 0-ля
          DataGridViewComboBoxColumn ComboBoxColumn = new DataGridViewComboBoxColumn();
// тут будет FOR подгрузка таблиц вместо JOINa
          Connect("SELECT *  FROM ЦП");//запись в датасет новой доп таблицы
          //ComboBoxColumn.DisplayIndex=3;//каким столбцом будетвыводлиться этот            //ComboBoxColumn.Items.AddRange("1", "c", "3", "3", "3");//так можно пост знач для выбора- аналог датасурсу
          
          
         
          ComboBoxColumn.DataSource = ds.Tables[1];
ComboBoxColumn.DataPropertyName = "ЦП";// dataGridView1.Columns[4].Name;
      //      ComboBoxColumn.
          ComboBoxColumn.DisplayMember = "имя";//ОБЯЗАТЕЛЬНА
ComboBoxColumn.ValueMember = "код";//чему должно соответствовать эти значения комбобокса из БД
          //ds.Tables[ds.Tables.Count - 1].Columns.Add( 
dataGridView1.Columns.Add(ComboBoxColumn);*/
            // dataGridView1.Columns.Insert(4, ComboBoxColumn);

    //        adapter.

   //select count(*) from INFORMATION_SCHEMA.TABLES

            GroupBox[] groupBoxArray = new GroupBox[ds.Tables[0].Columns.Count];
            groupBoxArray[0] = new GroupBox();
            groupBoxArray[0].Controls.Add(new TextBox());
            groupBoxArray[0].Controls.Add(new 




           // Pic[i].MouseClick += (b, eArgs) =>//запускаем прогу
                    {


                    }


        }

        void Connect(string tablName)
        {
            OleDbDataAdapter adapter = new OleDbDataAdapter("SELECT *  FROM " + tablName, cn);
            ds.Tables.Add();
            ds.Tables[ds.Tables.Count - 1].TableName = tablName;
            adapter.Fill(ds.Tables[ds.Tables.Count - 1]);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            /*using ( cn = new OleDbConnection(@"Provider=Microsoft.Jet.OleDb.4.0 ;Data Source=C:\\Users\\vc_05_208\\Desktop\\Del3\\Del3\\base.mdb;"))
            {
                if (cn.State == ConnectionState.Closed)
                    cn.Open();*/

                //adapter.SelectCommand = new OleDbCommand("select * from [изделя-имя;цена] ", cn);

                //adapter.UpdateCommand.Connection = cn;
                //adapter.UpdateCommand = new OleDbCommand("update [изделя-имя;цена] set *", cn);
               //adapter.

                //var cb = new OleDbCommandBuilder(adapter);
                //adapter.UpdateCommand = cb.GetUpdateCommand();
                //adapter.UpdateCommand.Connection = cn;
                adapter.Update(ds);

            

        }
        private void TEXTBoxS_Leave(object sender, EventArgs e)
        {         
            //прорерка на валидуцию...
            //
            //dataGridView1.Rows.Clear();
            var TextobjectSender = (TextBox)sender;

            DataSet dsFilter = new DataSet();
            dsFilter.Tables.Add("filterComp");
            for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
                dsFilter.Tables["filterComp"].Columns.Add(ds.Tables[0].Columns[i].ColumnName,ds.Tables[0].Columns[i].DataType);
            dataGridView1.DataSource = dsFilter.Tables["filterComp"];

            
            //     MessageBox.Show(((TextBox)sender).Text);
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                var FiltersParam=Convert.ToInt32(ds.Tables[0].Rows[i].ItemArray[Convert.ToInt32(TextobjectSender.Tag)]);
                if (FiltersParam > Int32.Parse(TextobjectSender.Text))
                {
                    //MessageBox.Show(FiltersParametr.ToString());
                    dsFilter.Tables["filterComp"].Rows.Add();
                    var count =dsFilter.Tables["filterComp"].Rows.Count-1;
                    dsFilter.Tables["filterComp"].Rows[count].ItemArray = ds.Tables[0].Rows[i].ItemArray;
                       // .ItemArray[Convert.ToInt32(TextobjectSender.Tag)]

                }
                    
                //dsFilter.Tables["filterComp"].Rows.
            }
           



            dataGridView1.DataSource = dsFilter.Tables["filterComp"];
            /*int x = int.Parse((sender as TextBox).Text);
            for(int i=0;i<dataGridView1.Rows.Count-1;i++)
            if ((decimal)dataGridView1.Rows[i].Cells["цена"].Value < x)
            dataGridView1.Rows.RemoveAt(i);
               */

        }

       
    }
}
