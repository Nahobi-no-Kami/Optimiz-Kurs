﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Optimiz_Kurs
{
    public partial class ResearchForm : Form
    {
        public string name;
        public ResearchForm(string name)
        {
            InitializeComponent();
            this.name = name;
            label1.Text = "С возвращением, " + name;
            LoadVariants();

        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void MinimButton_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        void LoadVariants()
        {
            using (Calculation.TaskContext dbase = new Calculation.TaskContext())
            {
                var task = dbase.Tasks;
                foreach (var variants in task)
                    VariantCombobox.Items.Add(variants.Variant);
            }
        }

        private void VariantChanged(object sender, EventArgs e)
        {
            using (Calculation.TaskContext dbase = new Calculation.TaskContext())
            {
                string variant = VariantCombobox.SelectedItem.ToString();
                var task = dbase.Tasks.Where(p => p.Variant.ToString() == variant);
                TaskTextTextbox.Text = task.FirstOrDefault().TaskText;
                FunctionTextbox.Text = task.FirstOrDefault().Function;
                MinT1Textbox.Text = task.FirstOrDefault().MinT1.ToString();
                MaxT1Textbox.Text = task.FirstOrDefault().MaxT1.ToString();
                MinT2Textbox.Text = task.FirstOrDefault().MinT2.ToString();
                MaxT2Textbox.Text = task.FirstOrDefault().MaxT2.ToString();
                SecondTypeTextbox.Text = task.FirstOrDefault().ConditionsSecType;
                AccuracyNumeric.Value = (decimal)task.FirstOrDefault().Accuracy;
            }
        }
        DataTable table = new DataTable();

        private void CalculateButton_Click(object sender, EventArgs e)
        {
            try { 
                if(MethodCombobox.SelectedItem.ToString() == "Метод Сканирования" && VariantCombobox.SelectedItem.ToString() == "5")
                using (Calculation.TaskContext dbase = new Calculation.TaskContext())
                {
                    string variant = VariantCombobox.SelectedItem.ToString();
                    var task = dbase.Tasks.Where(p => p.Variant.ToString() == variant);
                    Calculation.Calculations calc = new Calculation.Calculations();
                    table = calc.Calculate(task.FirstOrDefault().MinT1, task.FirstOrDefault().MinT2, task.FirstOrDefault().MaxT1, task.FirstOrDefault().MaxT2, task.FirstOrDefault().Function, task.FirstOrDefault().ConditionsSecType, task.FirstOrDefault().Accuracy);
                    dataGridView1.DataSource = table;
                    dataGridView1.Sort(dataGridView1.Columns["Value"], ListSortDirection.Ascending);
                        ResultLabel.Text = "Оптимальное значение целевой функции " + dataGridView1.Rows[0].Cells[2].Value.ToString() + " достигается при T1 = " + dataGridView1.Rows[0].Cells[0].Value.ToString() + " и T2 = " + dataGridView1.Rows[0].Cells[1].Value.ToString();
                    }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Вы не выбрали вариант или метод решения");
            }
        }

        private void Build2DChartButton_Click(object sender, EventArgs e)
        {
            Chart2DForm form = new Chart2DForm(table);
            this.Hide();
            form.ShowDialog();
            this.Show();
        }
    }
       
}