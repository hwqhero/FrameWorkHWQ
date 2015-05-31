using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileServer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            dataGridView1.Columns.Add("文件路径", "文件路径");
            dataGridView1.Columns.Add("文件名称", "文件名称");
            dataGridView1.Columns.Add("文件大小", "文件大小");
            foreach (MyFile fileName in FileServer.Insance.files)
            {
                DataGridViewRow dgvr = new DataGridViewRow();
                dgvr.Cells.Add(new DataGridViewTextBoxCell() { Value = fileName.filePath });
                dgvr.Cells.Add(new DataGridViewTextBoxCell() { Value = fileName.fileName });
                dgvr.Cells.Add(new DataGridViewTextBoxCell() { Value = fileName.datas.Length.ToString() });
                dataGridView1.Rows.Add(dgvr);
            }

        }
    }
}
