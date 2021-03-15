/*************************************************************************************************************
 *  Користувач вказує два файли, які містять елементи цілочислових матриць. Виконати наступні завдання:
 *      1.	Виділити кольором ті стовпці першої матриці, кожний елемент яких не менший за відповідний йому 
 *          елемент другої матриці, розташований у тій самій позиції.
 *      2.  Зобразити гістограму, поставивши кожній матриці у відповідність окремий ряд даних. На осі абсцис 
 *          треба відкласти натуральні числа від 0 до М ‒ 1, а на осі ординат — кількість різних елементів у 
 *          відповідному рядку матриці (М — кількість рядків матриці).
 **************************************************************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MatrixSample
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private (List<string[]>, bool) GetRows(string fileName = "")
        {
            var rows = new List<string[]>();
            bool isValidData = false;
            openFileDialog.InitialDirectory = Application.StartupPath;
            //openFileDialog.InitialDirectory = @"d:\SampleData";
            openFileDialog.FileName = fileName;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                isValidData = true;
                using (var reader = new StreamReader(openFileDialog.FileName))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var row = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                        if (rows.Count > 0 && row.Length != rows[0].Length)
                        {
                            MessageBox.Show($"Invalid data in {openFileDialog.FileName}, line {rows.Count + 1}", "Error");
                            isValidData = false;
                            break;
                        }
                        rows.Add(row);
                    }
                }
            }
            return (rows, isValidData);
        }

        private void FillGrid(List<string[]> rows, DataGridView grid)
        {
            grid.Rows.Clear();
            grid.ColumnCount = rows[0].Length;
            foreach (var row in rows)
            {
                grid.Rows.Add(row);
            }
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void loadButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Select data for the first matrix", "Information", MessageBoxButtons.OK);
            var (rows1, isValid1) = GetRows("matrix1.txt");
            MessageBox.Show("Select data for the double matrix", "Information", MessageBoxButtons.OK);
            var (rows2, isValid2) = GetRows("matrix2.txt");
            if (isValid1 && isValid2 && rows1.Count == rows2.Count && rows1.Count > 0 &&
                rows1[0].Length == rows2[0].Length)
            {
                FillGrid(rows1, matrix1Grid);
                FillGrid(rows2, matrix2Grid);
            }
            task1Button.Enabled = task2Button.Enabled = true;
            saveFirstButton.Enabled = saveSecondButton.Enabled = true;
        }

        private bool TryParseGrid(DataGridView grid, out int[,] matrix)
        {
            matrix = new int[grid.RowCount, grid.ColumnCount];
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    try
                    {
                        matrix[i, j] = Convert.ToInt32(grid[j, i].Value);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show($"Invalid value in cell ({i}, {j})");
                        grid[j, i].Selected = true;
                        return false;
                    }
                }
            }
            return true;
        }

        private void task1Button_Click(object sender, EventArgs e)
        {
            if (TryParseGrid(matrix1Grid, out var matrix1) && TryParseGrid(matrix2Grid, out var matrix2))
            {
                var m = matrix1.GetLength(0);
                var n = matrix1.GetLength(1);
                for (int j = 0; j < n; j++)
                {
                    var selectionRequired = true;
                    for (int i = 0; i < m; i++)
                    {
                        if (matrix1[i, j] < matrix2[i, j])
                        {
                            selectionRequired = false;
                            break;
                        }
                    }
                    if (selectionRequired)
                        matrix1Grid.Columns[j].DefaultCellStyle.BackColor = Color.Chartreuse;
                    else
                        matrix1Grid.Columns[j].DefaultCellStyle.BackColor = Color.White;
                }
            }
        }

        private void task2Button_Click(object sender, EventArgs e)
        {
            if (TryParseGrid(matrix1Grid, out var matrix1) && TryParseGrid(matrix2Grid, out var matrix2))
            {
                var chartForm = new ChartForm(matrix1, matrix2);
                chartForm.Show();
            }
        }

        private void saveFirstButton_Click(object sender, EventArgs e)
        {
            if (sender is Button button)
                SaveGrid(button == saveFirstButton? matrix1Grid: matrix2Grid);
        }

        private void SaveGrid(DataGridView grid)
        {
            saveFileDialog.InitialDirectory = Application.StartupPath;
            //saveFileDialog.InitialDirectory = @"d:\SampleData";
            saveFileDialog.FileName = "";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                using (var writer = new StreamWriter(saveFileDialog.FileName))
                {
                    foreach (DataGridViewRow row in grid.Rows)
                    {
                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            writer.Write(cell.Value.ToString() + "\t");
                        }
                        writer.WriteLine();
                    }
                }
            }
        }
    }
}
