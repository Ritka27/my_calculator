using System.Data;

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


        private void button11_DockChanged(object sender, EventArgs e)
        {
            string text = textBox1.Text;
            string lastNumber = "";
            int startIndex = 0;

            // Идём с конца строки и собираем число
            for (int i = text.Length - 1; i >= 0; i--)
            {
                char c = text[i];

                if (c == '+' || c == '-' || c == '×' || c == '÷')
                {
                    startIndex = i + 1;
                    break;
                }

                lastNumber = c + lastNumber; // добавляем один раз!
            }

            // Переводим число и делим на 100
            double number = Convert.ToDouble(lastNumber);
            number = number / 100;

            // Заменяем в строке
            textBox1.Text = text.Substring(0, startIndex) + number.ToString();

        }


        private void button22_Click(object sender, EventArgs e)
        {
            textBox1.Text += "^";
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
        
        private void textBox1_Resize(object sender, EventArgs e)
        {

        }

        private void button17_Click(object sender, EventArgs e)
        {
            string inputText = textBox1.Text;

            // Заменяем символы умножения и деления
            string expression = inputText.Replace("×", "*").Replace("÷", "/").Replace(",", ".");

            try
            {
                // Обработка корней
                int index = expression.IndexOf("√");// ищу индекс √
                while (index != -1)
                {
                    int start = index + 1;
                    string numberStr = "";

                    // Собираем число после √
                    while (start < expression.Length && (char.IsDigit(expression[start]) || expression[start] == '.'))
                    {
                        numberStr += expression[start];
                        start++;
                    }

                    if (!string.IsNullOrEmpty(numberStr))
                    {
                        double number = Convert.ToDouble(numberStr, System.Globalization.CultureInfo.InvariantCulture);
                        double res = Math.Sqrt(number);

                        // Заменяем √число на результат
                        expression = expression.Substring(0, index) + res.ToString(System.Globalization.CultureInfo.InvariantCulture) + expression.Substring(start);
                    }

                    index = expression.IndexOf("√", index + 1);
                }

                // Обработка ^ (только число^число)
                int i = expression.IndexOf("^");
                while (i != -1)
                {
                    // Лево
                    int left = i - 1;
                    string a = "";
                    while (left >= 0 && (char.IsDigit(expression[left]) || expression[left] == '.'))
                    {
                        a = expression[left] + a;
                        left--;
                    }

                    // Право
                    int right = i + 1;
                    string b = "";
                    while (right < expression.Length && (char.IsDigit(expression[right]) || expression[right] == '.'))
                    {
                        b += expression[right];
                        right++;
                    }

                    if (a != "" && b != "")
                    {
                        double baseNum = Convert.ToDouble(a, System.Globalization.CultureInfo.InvariantCulture);
                        double exponent = Convert.ToDouble(b, System.Globalization.CultureInfo.InvariantCulture);
                        double res = Math.Pow(baseNum, exponent);

                        expression = expression.Substring(0, left + 1) + res.ToString(System.Globalization.CultureInfo.InvariantCulture) + expression.Substring(right);
                    }

                    i = expression.IndexOf("^");
                }

                // Вычисляем итоговое выражение
                var d = new DataTable();
                var result = d.Compute(expression, null).ToString();

                textBox1.Text = result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка в выражении: " + ex.Message);
            }
        }



        private void button11_Click(object sender, EventArgs e)
        {
            string s = textBox1.Text;
            if (s.Length > 0) // Проверяем, что строка не пустая
            {
                s = s.Substring(0,s.Length - 1); // Удаляем последний символ
                textBox1.Text = s; // Обновляем текст в TextBox
            }

        }

        // очищение поля полностью 
        private void button12_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
        }
    }
}
