using System.Data;
using System.DirectoryServices.ActiveDirectory;
using System.Globalization;
using System.Text.RegularExpressions;

namespace WinFormsApp010
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void buttonClick(object sender, EventArgs e)
        {
            var currentButton = sender as Button;
            textBox1.Text += currentButton.Text; // добавляем текст кнопки в поле ввода
        }


        private void DivideLastBy100(object sender, EventArgs e)
        {
            string text = textBox1.Text;

            int cl = -1;
            int op = -1;

            // Ищем закрывающую скобку с конца
            for (int i = text.Length - 1; i >= 0; i--)
            {
                if (text[i] == ')')
                {
                    cl = i;
                    break;
                }
            }

            // Если нашли закрывающую скобку
            if (cl != -1)
            {
                // Ищем открывающую скобку перед ней
                for (int i = cl; i >= 0; i--)
                {
                    if (text[i] == '(')
                    {
                        op = i;
                        break;
                    }
                }

                // Если нашли обе скобки
                if (op != -1)
                {
                    string expr = text.Substring(op + 1, cl - op - 1);
                    expr = expr.Replace("×", "*").Replace("÷", "/");

                    var result = new System.Data.DataTable().Compute(expr, null);
                    double val = Convert.ToDouble(result) / 100;

                    textBox1.Text = text.Substring(0, op) + val.ToString() + text.Substring(cl + 1);
                    return;
                }
            }

            // Если скобок нет — обычное число с конца строки
            string lastNumber = "";
            int startIndex = 0;

            for (int i = text.Length - 1; i >= 0; i--)
            {
                char c = text[i];

                if (c == '+' || c == '-' || c == '×' || c == '÷')
                {
                    startIndex = i + 1;
                    break;
                }

                lastNumber = c + lastNumber;
            }

            if (!string.IsNullOrEmpty(lastNumber))
            {
                double number = Convert.ToDouble(lastNumber) / 100;
                textBox1.Text = text.Substring(0, startIndex) + number.ToString();
            }
        }



        private void buttonPower_Click(object sender, EventArgs e)
        {
            textBox1.Text += "^";
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text == "Делить на ноль нельзя!")
                return;



        }

        private void textBox1_Resize(object sender, EventArgs e)
        {

        }

        private void buttonCalculate_Click(object sender, EventArgs e)
        {
            string inputText = textBox1.Text;

            // Заменяем специальные символы
            string expression = inputText.Replace("×", "*")
                                         .Replace("÷", "/")
                                         .Replace(",", ".");

            try
            {
                // Обработка корня √
                int index = expression.IndexOf("√");

                // Добавляем * перед √, если перед ним стоит число или скобка
                expression = Regex.Replace(expression, @"(\d|\)|\.)√", "$1*√");

                while (index != -1)
                {
                    int start = index + 1;
                    string numberStr = "";

                    while (start < expression.Length && (char.IsDigit(expression[start]) || expression[start] == '.'))
                    {
                        numberStr += expression[start];
                        start++;
                    }

                    if (!string.IsNullOrEmpty(numberStr))
                    {
                        double number = Convert.ToDouble(numberStr, CultureInfo.InvariantCulture);
                        double res = Math.Sqrt(number);
                        expression = expression.Substring(0, index) + res.ToString(CultureInfo.InvariantCulture) + expression.Substring(start);
                    }

                    index = expression.IndexOf("√", index + 1);
                }

                // Обработка степени ^
                int i = expression.IndexOf("^");
                while (i != -1)
                {
                    int left = i - 1;
                    string a = "";
                    while (left >= 0 && (char.IsDigit(expression[left]) || expression[left] == '.'))
                    {
                        a = expression[left] + a;
                        left--;
                    }

                    int right = i + 1;
                    string b = "";
                    while (right < expression.Length && (char.IsDigit(expression[right]) || expression[right] == '.'))
                    {
                        b += expression[right];
                        right++;
                    }

                    if (a != "" && b != "")
                    {
                        double baseNum = Convert.ToDouble(a, CultureInfo.InvariantCulture);
                        double exponent = Convert.ToDouble(b, CultureInfo.InvariantCulture);
                        double res = Math.Pow(baseNum, exponent);
                        expression = expression.Substring(0, left + 1) + res.ToString(CultureInfo.InvariantCulture) + expression.Substring(right);
                    }

                    i = expression.IndexOf("^");
                }

                // Умножение на скобку без знака (например, 2(3+4) => 2*(3+4))
                expression = Regex.Replace(expression, @"(\d|\))\(", "$1*(");

                string resultStr;
                var dt = new DataTable();

                try
                {
                    resultStr = dt.Compute(expression, null).ToString();
                }
                catch (OverflowException)
                {
                    // Если переполнение — добавляем .0 к длинным числам
                    string safeExpression = Regex.Replace(expression, @"\b\d{10,}\b", m => m.Value + ".0");
                    resultStr = dt.Compute(safeExpression, null).ToString();
                }


                // Если результат недопустим — кидаем исключение
                if (resultStr == "∞" || resultStr == "Infinity" || resultStr == "-Infinity" || resultStr == "NaN")
                {
                    throw new DivideByZeroException(); 
                }
                double result = Convert.ToDouble(resultStr, CultureInfo.InvariantCulture);
                textBox1.Text = result.ToString(CultureInfo.InvariantCulture);

            }
            catch (DivideByZeroException)
            {
                MessageBox.Show("Ошибка!!! Делить на ноль нельзя!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка в выражении: " + ex.Message);
            }
        }



        private void buttonBackspace(object sender, EventArgs e)
        {
            string s = textBox1.Text;
            if (s.Length > 0) // Проверяем, что строка не пустая
            {
                s = s.Substring(0, s.Length - 1); // Удаляем последний символ
                textBox1.Text = s; // Обновляем текст в TextBox
            }

        }

        // очищение поля полностью 
        private void buttonClear_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            char[] allowed = { '+', '-', '*', '/', '^' };
            char ch = e.KeyChar;
            bool isAllowed = false;
            // backspace(разрешение использовать эту клавишу)
            if (ch == 8)
            {
                isAllowed = true;
            }
            foreach (char c in allowed)
            {
                if (c == ch)
                {
                    isAllowed = true;
                    break;
                }
            }
            if (char.IsDigit(ch))
            {
                isAllowed = true;  // Разрешаем цифры
            }
            if (!isAllowed)
            {
                e.Handled = true; // запрещаем ввод других символов
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            // Если нажата клавиша Enter
            if (e.KeyCode == Keys.Enter)
            {
                // Блокируем стандартное поведение Enter (не будет переходить на новую строку)
                e.SuppressKeyPress = true;

                // Выполняем действие кнопки
                buttonCalculate_Click(sender, e);
            }

            
        }
    }
}
