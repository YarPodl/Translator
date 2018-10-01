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
            textBox1.Text = "";
            label2.Text = "";
            try
            {
                MyTranslator translator = new MyTranslator(richTextBox1.Text);
                var n = translator.LexicalAnalysis();
                foreach (var item in n)
                {
                    listBox1.Items.Add(item.ToString());
                }
                translator.Parse(listBox1);
                textBox1.Text = "Принадлежит";
            }
            catch (MyTranslator.TranslateExeption ex)
            {
                int offset = richTextBox1.TextLength - ex.offset;
                richTextBox1.Select(offset, ex.length);
                richTextBox1.SelectionBackColor = Color.Red;
                textBox1.Text = ex.Message;
                int line = richTextBox1.GetLineFromCharIndex(offset);
                int column = offset - richTextBox1.GetFirstCharIndexFromLine(line);
                textBox1.Text += $" (Строка {line + 1}, символ {column})";

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
