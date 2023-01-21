using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using CustomMath;


namespace Calculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        double result = 0;
        char op = ' ';

        public MainWindow()
        {
            InitializeComponent();
        }

        /**
        * @brief These functions just write numbers into input.
        */
        #region number buttons
        private void N_0_Click(object sender, RoutedEventArgs e)
        {
            Input.Text += "0";
        }

        private void N_1_Click(object sender, RoutedEventArgs e)
        {
            Input.Text += "1";
        }
        private void N_2_Click(object sender, RoutedEventArgs e)
        {
            Input.Text += "2";
        }
        private void N_3_Click(object sender, RoutedEventArgs e)
        {
            Input.Text += "3";
        }
        private void N_4_Click(object sender, RoutedEventArgs e)
        {
            Input.Text += "4";
        }
        private void N_5_Click(object sender, RoutedEventArgs e)
        {
            Input.Text += "5";
        }
        private void N_6_Click(object sender, RoutedEventArgs e)
        {
            Input.Text += "6";
        }
        private void N_7_Click(object sender, RoutedEventArgs e)
        {
            Input.Text += "7";
        }
        private void N_8_Click(object sender, RoutedEventArgs e)
        {
            Input.Text += "8";
        }
        private void N_9_Click(object sender, RoutedEventArgs e)
        {
            Input.Text += "9";
        }
        #endregion
        #region operators

        /**
        * @brief Calculates the previous operation of temp result and actual input, adds add operator to it and prints it into temp result. Resets input.
        */
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                result = equals(result, op, double.Parse(Input.Text));
                op = '+';
                Result.Text = result.ToString() + "+";
                Input.Text = "";
            }
            catch { }
        }

        /**
        * @brief Calculates the previous operation of temp result and actual input adds sub operator to it and prints it into temp result. Resets output. If this is first operation it writes the sub in front of actual input.
        */
        private void Sub_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (op == ' '&&Input.Text=="")
                    Input.Text = "-";
                else
                {
                    result = equals(result, op, double.Parse(Input.Text));
                    op = '-';
                    Result.Text = result.ToString() + "-";
                    Input.Text = "";
                }
            }
            catch { }
        }

        /**
        * @brief Calculates the previous operation of temp result and actual input adds mul operator to it and prints it into temp result. Resets input.
        */
        private void Mul_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                result = equals(result, op, double.Parse(Input.Text));
                op = 'x';
                Result.Text = result.ToString() + "x";
                Input.Text = "";
            }
            catch { }
        }

        /**
        * @brief Calculates the previous operation of temp result and actual input adds div operator to it and prints it into temp result. Resets input.
        */
        private void Div_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                result = equals(result, op, double.Parse(Input.Text));
                op = '÷';
                Result.Text = result.ToString() + "÷";
                Input.Text = "";
            }
            catch { }
        }

        /**
        * @brief Calculates the previous operation of temp result and actual input adds pow operator to it and prints it into temp result. Resets input.
        */
        private void Pow_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                result = equals(result, op, double.Parse(Input.Text));
                op = '^';
                Result.Text = result.ToString() + "^";
                Input.Text = "";
            }
            catch { }
        }

        /**
        * @brief Calculates the previous operation of temp result and actual input adds mod operator to it and prints it into temp result. Resets input.
        */
        private void Mod_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                result = equals(result, op, double.Parse(Input.Text));
                op = '%';
                Result.Text = result.ToString() + "%";
                Input.Text = "";
            }
            catch { }

        }

        /**
        * @brief Calculates the root of actual number and prints it into input.
        */
        private void Root_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                result = equals(result, op, double.Parse(Input.Text));
                op = '√';
                Result.Text = result.ToString() + "√";
                Input.Text = "";
            }
            catch { }

        }

        /**
        * @brief Calculates the factorial of actual number and prints it into input.
        */
        private void Fac_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int whole_number = 0;
                double temp = 0;
                if (int.TryParse(Input.Text, out whole_number))
                {
                    temp = MathFunctions.Factorial(whole_number);
                }
                else
                    throw new Exception("Can't make a factorial of not fraction number.");
                Input.Text = temp.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /**
        * @brief Calculates the natural logarithm of actual number and prints it into input.
        */
        private void Ln_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                double temp;
                if (double.TryParse(Input.Text, out temp))
                {
                    temp = MathFunctions.Ln(temp);
                }
                Input.Text = temp.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        /**
        * @brief Helper function to calculate the temp result 
        */
        private double equals(double a, char op, double b)
        {
            try
            {
                switch (op)
                {
                    case '√':
                        return MathFunctions.Root(b, a);
                    case '+':
                        return MathFunctions.Add(a, b);
                    case '-':
                        return MathFunctions.Subtract(a, b);
                    case 'x':
                        return MathFunctions.Multiply(a, b);
                    case '÷':
                        return MathFunctions.Divide(a, b);
                    case '^':
                        return MathFunctions.Power(a, (int)b);
                    case '%':
                        return MathFunctions.Mod(a, b);
                    default:
                        return b;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return b;
            }
        }

        /**
        * @brief Calculates the equation and writes the result to input, resets operator and temp number
        */
        private void Equals_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                result = equals(result, op, double.Parse(Input.Text));
                Result.Text = "";
                Input.Text = result.ToString();
                result = 0;
                op = ' ';
            }
            catch { }
        }

        /**
       * @brief Adds dot to actual number
       */
        private void Dot_Click(object sender, RoutedEventArgs e)
        {
            Input.Text += ",";
        }

        /**
         * @brief Resets actual number
         */
        private void CE_Click(object sender, RoutedEventArgs e)
        {
            Input.Text = "";
        }

        /**
         * @brief Deletes one digit from actual number
         */
        private void DEL_Click(object sender, RoutedEventArgs e)
        {
            if(Input.Text.Length > 0)
                Input.Text = Input.Text.Substring(0, Input.Text.Length - 1);
        }

        /**
         * @brief resets everything in calculator to default
         */
        private void C_Click(object sender, RoutedEventArgs e)
        {
            Input.Text = "";
            Result.Text = "";
            op = ' ';
        }

        /**
         * @brief Shows messagebox with hint text 
         */
        private void Help_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("This calculator counts temporary results in the upper section of display.\nFunctions of factorial and ln() always modify the inputed number.\nButton C resets whole calculator while button CE resets only actual input.\nButton DEL removes one digit of actual input.\n\nThis application was made by Bigyteam as a part of university project to the course of Practical aspects of software design.\nCurrent version: 1.0");
        }


    }
}
