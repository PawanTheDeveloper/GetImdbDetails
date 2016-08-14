using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ImdbDetails;
using System.IO;
//using SqlLite;
//using Finisar.SQLite;

namespace Network_Sharing
{
    public partial class Form1 : Form
    {

        //private SQLiteDataReader sqlReader;
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            listView1.View = View.Details;
            listView1.GridLines = true;
            listView1.FullRowSelect = true;
            listView1.HideSelection = false;
            listView1.MultiSelect = false;
            listView1.Columns.Add("Movie Name", 300);
            listView1.Columns.Add("Year", 80);
            listView1.Columns.Add("IMDB Rating", 125);
            textBox1.Text = "YOU CAN DIRECTLY COPY THE PATH HERE";
        }

        private void CreateConnectionAndUpdateList(string movie)
        {
            GetImdbDetails gd = new GetImdbDetails();
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary = gd.GetDetailsByName(movie);
            ListViewItem item = new ListViewItem();
            if (dictionary.Count == 0)
            {
                item.Text = movie;
                listView1.Items.Add(item);
                return;
            }
            
            foreach (var param in dictionary)
            {
                if (String.Equals(param.Key, "title"))
                {
                    item.Text=param.Value;
                }
                if (String.Equals(param.Key, "year"))
                {
                    item.SubItems.Add(param.Value);
                }
                if (String.Equals(param.Key, "imdbRating"))
                {
                    item.SubItems.Add(param.Value);
                }                
            }
            listView1.Items.Add(item);
            return;
        }
        private void getDirsFiles(DirectoryInfo dir)
        {
            FileInfo[] files;
            string previous_movie_name = "Default";
            string new_movie_name = string.Empty;
            int index;
            files = dir.GetFiles("*.*");
            foreach (FileInfo file in files)
            {
                if (file.Extension.Equals(".avi") || file.Extension.Equals(".mkv") || file.Extension.Equals(".mp4") || file.Extension.Equals(".wmv") || file.Extension.Equals(".mpeg4") || file.Extension.Equals(".mov") || file.Extension.Equals(".vob") || file.Extension.Equals(".m4v"))
                {
                    if (file.Name.Length < 6)
                        continue;
                    if (!((file.FullName.ToLower().Contains("sample")) || (file.FullName.ToLower().Contains("trailer"))))
                    {
                        new_movie_name = file.FullName;
                        index = (new_movie_name.Length) / 2;
                        if (string.Compare(previous_movie_name, 0, new_movie_name, 0, index) == 0)
                        {
                            previous_movie_name = new_movie_name;
                            continue;
                        }
                        CreateConnectionAndUpdateList(file.Name);
                        previous_movie_name = new_movie_name;
                    }
                }

            }
            //get sub-folders for the current directory
            DirectoryInfo[] dirs = dir.GetDirectories("*.*");
            foreach (DirectoryInfo otherdir in dirs)
            {
                getDirsFiles(otherdir);
            }
        }
        private void button2_Click_1(object sender, EventArgs e)
        {
            if (textBox1.Text.Equals(""))
            {
                MessageBox.Show("Please Enter the Folder or Network Path");
            }
            else
            {
                DirectoryInfo dir;
                dir = new DirectoryInfo(textBox1.Text.ToString());
                getDirsFiles(dir);
                MessageBox.Show("Completed");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            richTextBox2.Clear();
            GetImdbDetails gd = new GetImdbDetails();
            string movie = richTextBox1.Text;
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary = gd.GetDetailsByName(movie);
            ListViewItem item = new ListViewItem();
            if (dictionary.Count == 0)
                richTextBox2.AppendText("Nothing");
            else
            {
                foreach (var param in dictionary)
                {
                    if (String.Equals(param.Key, "title"))
                    {
                        richTextBox2.AppendText(param.Key+ ":=>" +param.Value+"\n");
                    }
                    if (String.Equals(param.Key, "year"))
                    {
                        richTextBox2.AppendText(param.Key + ":=>" + param.Value + "\n");
                    }
                    if (String.Equals(param.Key, "imdbRating"))
                    {
                        richTextBox2.AppendText(param.Key + ":=>" + param.Value + "\n");
                    }
                }
            }
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            this.folderBrowserDialog1.ShowNewFolderButton = false;
            this.folderBrowserDialog1.RootFolder =System.Environment.SpecialFolder.MyComputer;
            DialogResult result = this.folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                textBox1.Text = folderBrowserDialog1.SelectedPath;                
            }
        }

        private string getpropermoviefilename()
        {
            int i = 1;
            string default_name = "movielistbak";
            while (true)
            {
                if (File.Exists(default_name + i + ".txt"))
                {
                    i++;
                    continue;
                }
                else
                {
                    string name = default_name + i + ".txt";
                    return name;
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (File.Exists("movielist.txt"))
            {
                string filename=getpropermoviefilename();
                File.Move("movielist.txt", filename);
            }
            StreamWriter writefile = new StreamWriter("movielist.txt");
            string name="NAME";
            string year="YEAR";
            string rating="RATING";
            writefile.WriteLine(string.Format("{0} {1} {2}", name.PadRight(25), year.PadRight(8), rating.PadRight(8)));
            writefile.WriteLine("\n");
            for (int i = 0; i < listView1.Items.Count; i++)
            {
                writefile.WriteLine(string.Format("{0} {1} {2}",listView1.Items[i].SubItems[0].Text.PadRight(25),listView1.Items[i].SubItems[1].Text.PadRight(8),listView1.Items[i].SubItems[2].Text.PadRight(8)));
            }
            writefile.Close();

            System.Diagnostics.Process.Start("explorer.exe",Directory.GetCurrentDirectory());
         }

        private void textBox1_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
              
    }
}
