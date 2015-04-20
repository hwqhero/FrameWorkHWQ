using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileClient
{
    public partial class Form1 : Form
    {
        public static Form1 insance;
        private Delegate d;
        public Form1()
        {
            InitializeComponent();
            insance = this;
            d = Delegate.CreateDelegate(typeof(Action<int, int>), this, "Show");
        }

        public void aaaa(params object[] obj)
        {
            Invoke(d, obj);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text) && string.IsNullOrEmpty(textBox2.Text))
            {
                return;
            }
            FileClient.Insance.Login(textBox1.Text, textBox2.Text);
        }

        public void Show(int current, int count)
        {
            progressBar1.Maximum = count;
            progressBar1.Minimum = 0;
            progressBar1.Value = current;
            float b = (float)current / (float)count;
            b *= 100;
            label2.Text = b.ToString();
        }
    }
}
