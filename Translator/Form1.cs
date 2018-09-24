using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Translator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            richTextBox1.SelectAll();
            richTextBox1.SelectionBackColor = SystemColors.Window;
            richTextBox1.DeselectAll();
            label1.Text = "";
            label2.Text = "";
            try
            {
                MyTranslator translator = new MyTranslator(richTextBox1.Text);
                var n = translator.LexicalAnalysis(richTextBox1.Text);
                foreach (var item in n)
                {
                    listBox1.Items.Add(item.ToString());
                }
                var b = translator.Parse(n, listBox1);
                label1.Text = b ? "Принадлежит" : "Не принадлежит";
            }
            catch (MyTranslator.TranslateExeption ex)
            {
                int offset = richTextBox1.TextLength - ex.offset;
                int endLine = richTextBox1.Text.IndexOf('\n', offset);
                richTextBox1.Select(offset, endLine - offset);
                richTextBox1.SelectionBackColor = Color.Red;
                label1.Text = ex.Message;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
