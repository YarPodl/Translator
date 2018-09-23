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
            MyTranslator translator = new MyTranslator(richTextBox1.Text);
            var n = translator.LexicalAnalysis(richTextBox1.Text);
            foreach (var item in n)
            {
                listBox1.Items.Add(item.ToString());
            }
        }
    }
}
