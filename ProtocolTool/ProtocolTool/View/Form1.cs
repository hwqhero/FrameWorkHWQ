using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ProtocolTool.Model;
using System.IO;
using ProtocolTool.View;

namespace ProtocolTool
{

    public partial class Form1 : Form
    {
        public string[] typeList = new string[] 
        { 
            "Boolean","Byte","SByte","Char","Decimal","Double" ,"Single" ,"Int32" ,"UInt32","Int64","UInt64","Int16","UInt16","String","DateTime"
        };
        
        DataClass cd;
        private Form form;
        private CustomClass currentClass;
        public Form1()
        {
            InitializeComponent();
            cd = new DataClass();
            cd.classList = new List<CustomClass>();
            //Form2 f2 = new Form2();
            //f2.MdiParent = this;
            //f2.InitializeDevice();
            //this.AddOwnedForm(f2);
            //f2.Show();
        }

        /// <summary>
        /// 添加字段
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox3.Text) || string.IsNullOrEmpty(comboBox1.Text))
            {
                return;
            }
            if (currentClass != null)
            {
                CustomField cf = new CustomField();
                cf.name = textBox3.Text;
                cf.isList = checkBox1.Checked;
                cf.desc = textBox4.Text;
                cf.type = comboBox1.Text;
                cf.isRelation = cd.classList.Find(obj => obj.name == comboBox1.Text) != null;
                cf.relationList = new List<CustomRelation>();
                currentClass.fieldList.Add(cf);
                ResetField();
                textBox3.Text = string.Empty;
                checkBox1.Checked = false;
                textBox4.Text = string.Empty;
            }
        }

        private void ResetClass()
        {
            comboBox1.Items.Clear();
            comboBox2.Items.Clear();
            foreach (CustomClass cc in cd.classList)
            {
                if (cc.name != currentClass.name)
                {
                    comboBox1.Items.Add(cc.name);
                    comboBox2.Items.Add(cc.name);
                }
                  
            }
            comboBox1.Items.AddRange(typeList);
            comboBox2.Items.AddRange(typeList);
        }

        private void ResetTreeView()
        {
            treeView1.Nodes.Clear();
            foreach (CustomClass cc in cd.classList)
            {
                TreeNode tn = new TreeNode();
                tn.Name = cc.GetHashCode().ToString();
                tn.Text = cc.name;
                foreach (CustomField cf in cc.fieldList)
                {
                    TreeNode nn = new TreeNode(cf.ToString());
                    tn.Nodes.Add(nn);
                }
                treeView1.Nodes.Add(tn);
            }
        }

        private void ResetField()
        {
            if (currentClass != null)
            {
                comboBox3.Items.Clear();
                if(!string.IsNullOrEmpty(currentClass.baseName))
                {
                    CustomClass cc = cd.classList.Find(obj => obj.name == currentClass.baseName);
                    if (cc != null)
                    {
                        foreach (CustomField cf in cc.fieldList)
                        {
                            comboBox3.Items.Add(cf.name);
                        }
                    }
                }
                foreach (CustomField cf in currentClass.fieldList)
                {
                    comboBox3.Items.Add(cf.name);
                }
            }
        }

        /// <summary>
        /// 添加外键
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(comboBox4.Text) || string.IsNullOrEmpty(comboBox8.Text))
            {
                return;
            }
            if (currentClass != null)
            {
                CustomField cf = currentClass.fieldList.Find(obj => obj.name == comboBox3.Text);
                if (cf != null)
                {
                    CustomRelation cr = new CustomRelation();
                    cr.myFieldName = comboBox4.Text;
                    cr.targetFieldName = comboBox8.Text;
                    cf.relationList.Add(cr);
                    R1();
                    R();
                }
            }
        }

        /// <summary>
        /// 确定修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button6_Click(object sender, EventArgs e)
        {
            if (currentClass != null)
            {
                if (!cd.classList.Contains(currentClass))
                {
                    cd.classList.Add(currentClass);
                }
                List<Control> cList = new List<Control>();
                foreach (Control c in splitContainer1.Panel2.Controls)
                {
                    cList.AddRange(FindControl(c));

                }
                foreach (Control c in cList)
                {
                    ComboBox cb = c as ComboBox;
                    if (cb != null)
                    {
                        cb.Items.Clear();
                    }
                    TextBox tb = c as TextBox;
                    if (tb != null)
                    {
                        tb.Text = string.Empty;
                    }
                }
                cList.Clear();
                ResetTreeView();
                ResetBaseName();
                cd.ToBinary(textBox6.Text);
                currentClass = null;
            }
                
        }

        /// <summary>
        /// 添加类
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button7_Click(object sender, EventArgs e)
        {
            if (currentClass != null)
            {
                currentClass.baseName = comboBox6.Text;
                currentClass.name = textBox1.Text;
                currentClass.desc = textBox2.Text;
            }
            else
            {
                currentClass = new CustomClass();
                currentClass.baseName = comboBox6.Text;
                currentClass.name = textBox1.Text;
                currentClass.desc = textBox2.Text;
                currentClass.relationList = new List<CustomRelation>();
                currentClass.fieldList = new List<CustomField>();
            }

            ResetClass();
        }


        private void button3_Click(object sender, EventArgs e)
        {
            if (currentClass != null)
            {
                CustomField cf = currentClass.fieldList.Find(obj => obj.name == comboBox3.Text);
                if (cf != null)
                {
                    currentClass.fieldList.Remove(cf);
                    ResetField();
                    ResetClass();
                    R1();
                    R();
                }
                    
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            if (currentClass != null)
            {
                CustomField cf = currentClass.fieldList.Find(obj => obj.name == comboBox3.Text);
                if (cf != null)
                {
                    CustomRelation cr = cf.relationList.Find(obj => obj.myFieldName == comboBox4.Text && obj.targetFieldName == comboBox9.Text);
                    if (cr != null)
                    {
                        cf.relationList.Remove(cr);
                        R1();
                        R();
                    }
                }
            }
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            R1();
        }

        private void R1()
        {
            comboBox4.Items.Clear();
            comboBox8.Items.Clear();
            if (currentClass != null)
            {
                CustomField cf = currentClass.fieldList.Find(obj => obj.name == comboBox3.Text);
                if (cf != null)
                {
                    textBox7.Text = comboBox3.Text;
                }
                if (cf != null && cf.isRelation)
                {
                    CustomClass targetClass = cd.classList.Find(obj => obj.name == cf.type);
                    if (targetClass == null)
                        return;
                    foreach (CustomField ff in currentClass.fieldList)
                    {
                        if (ff.isRelation)
                            continue;
                        //if (cf.relationList.Find(obj => obj.myFieldName == ff.name) == null)
                        {
                            comboBox4.Items.Add(ff.name);
                        }
                    }
                    CustomClass cc = cd.classList.Find(obj => obj.name == targetClass.baseName);
                    if (cc != null)
                    {
                        foreach (CustomField cff in cc.fieldList)
                        {
                            if (cff.isRelation)
                                continue;
                            if (cf.relationList.Find(obj => obj.targetFieldName == cff.name) == null)
                            {
                                comboBox8.Items.Add(cff.name);
                            }
                        }
                    }
                    foreach (CustomField ff in targetClass.fieldList)
                    {
                        if (ff.isRelation)
                            continue;
                        if (cf.relationList.Find(obj => obj.targetFieldName == ff.name) == null)
                        {
                            comboBox8.Items.Add(ff.name);
                        }
                    }
                }
            }
        }

        private void R()
        {
            comboBox9.Items.Clear();
            CustomField cf = currentClass.fieldList.Find(obj => obj.name == comboBox3.Text);
            if (cf != null && comboBox4.SelectedItem != null)
            {
                foreach (CustomRelation ccr in cf.relationList)
                {
                    if (ccr.myFieldName == comboBox4.Text)
                    {
                        comboBox9.Items.Add(ccr.targetFieldName);
                    }
                }
            }
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            R();
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                if (e.Node.Level == 0)
                {
                    treeView1.SelectedNode = treeView1.GetNodeAt(e.Location);
                    contextMenuStrip1.Show(treeView1, e.Location);
                }
            }
        }

        private void ResetBaseName()
        {
            comboBox6.Items.Clear();
            foreach (CustomClass cc in cd.classList)
            {
                if (cc.name != currentClass.name)
                    comboBox6.Items.Add(cc.name);
            }

        }

        private void button5_Click(object sender, EventArgs e)
        {
            List<Control> cList = new List<Control>();
            foreach (Control c in splitContainer1.Panel2.Controls)
            {
                cList.AddRange(FindControl(c));

            }
            foreach (Control c in cList)
            {
                ComboBox cb = c as ComboBox;
                if (cb != null)
                {
                    cb.Items.Clear();
                }
                TextBox tb = c as TextBox;
                if (tb != null)
                {
                    tb.Text = string.Empty;
                }
            }
            cList.Clear();
            currentClass = null;
        }

        private List<Control> FindControl(Control c)
        {
            List<Control> cList = new List<Control>();
            foreach (Control c1 in c.Controls)
            {
                if (c1.HasChildren)
                {
                    cList.AddRange(FindControl(c1));
                }
                cList.Add(c1);
            }
            return cList;
        }

        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode tn = treeView1.SelectedNode;
            if (tn != null)
            {
                CustomClass cc = cd.classList.Find(obj => obj.name == tn.Text);
                if (cc == null)
                {
                    return;
                }
                cd.classList.Remove(cc);
                cc = null;
                ResetTreeView();
                cd.ToBinary(textBox6.Text);
            }
        }

        private void 编辑ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode tn = treeView1.SelectedNode;
            if (tn != null)
            {
                CustomClass cc = cd.classList.Find(obj => obj.name == tn.Text);
                if (cc == null)
                {
                    return;
                }
                currentClass = cc;
                textBox1.Text = currentClass.name;
                textBox2.Text = currentClass.desc;
                ResetBaseName();
                comboBox6.Text = currentClass.baseName;
                ResetClass();
                ResetField();
                R1();
                R();
            }
  
        }

        private void button8_Click(object sender, EventArgs e)
        {
            cd.nameSpcae = textBox5.Text;
            cd.ToBinary(textBox6.Text);
        }


        private void GenerateCode()
        {
           
            SaveFileDialog sf = new SaveFileDialog();
            sf.DefaultExt = "dll";
            sf.Filter = "dll(*.dll) | *.dll";
            sf.InitialDirectory =Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            sf.SupportMultiDottedExtensions = true;
            sf.Title = "保存文件";
            DialogResult dr = sf.ShowDialog();
            if (dr == System.Windows.Forms.DialogResult.OK)
            {
                FileStream s = sf.OpenFile() as FileStream;
                string p = s.Name;
                s.Close();
                cd.GenerateCode(p);
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.S))
            {
                GenerateCode();
            }
            else if (keyData == (Keys.Alt | Keys.D))
            {
                if (form == null)
                {
                    form = new ReadExcel();
                    this.AddOwnedForm(form);
                    form.FormClosed += form_FormClosed;
                    form.Show();

                }
                else
                {
                    form.Activate();
                }
            }
            return false;
        }

        void form_FormClosed(object sender, FormClosedEventArgs e)
        {
            form = null;
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Link; //重要代码：表明是链接类型的数据，比如文件路径
            else e.Effect = DragDropEffects.None;
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Effect == DragDropEffects.Link)
            {
                string path = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
                string ex = Path.GetExtension(path);
                if (ex == ".ProtocolHWQ")
                {
                    if (File.Exists(path))
                    {
                        byte[] temp = File.ReadAllBytes(path);
                        cd.Load(temp);
                        textBox5.Text = cd.nameSpcae;
                        ResetTreeView();
                    }
                    textBox6.Text = path;
                }
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (currentClass == null)
                return;
            CustomField cf = currentClass.fieldList.Find(obj => obj.name == comboBox3.Text);
            if (!string.IsNullOrEmpty(currentClass.baseName))
            {
                CustomClass cc = cd.classList.Find(obj => obj.name == currentClass.baseName);
                cf = cc.fieldList.Find(obj => obj.name == textBox3.Text);
            }

            if (cf != null)
            {
                cf.name = textBox7.Text;
                cf.type = comboBox2.Text;
            }
        }
    }
}
