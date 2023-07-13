using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace HYTETestTool
{
    /// <summary>
    /// ControlPage.xaml 的互動邏輯
    /// </summary>
    public partial class ControlPage : Page
    {
        public ControlPage()
        {
            InitializeComponent();
            UIHelper.Instance.RegistrySnNumberUI(UpdateSn);
        }

        private void UpdateSn(string update)
        {
            Dispatcher.Invoke(() =>
            {
                SNTB.Text = update;
                if (update == "Can Write")
                {
                    WriteBtn.IsEnabled = true;
                }
                else
                {
                    WriteBtn.IsEnabled = false;
                }
            });
        }

        private void WriteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (InputTb.Text.Length == 8)
            {
                string Result = "FGQ60BNLAPKCN" + InputTb.Text + "-0000";
                UIHelper.Instance.WriteSnNumber(Result);
            }
            else
            {
                MessageBox.Show("輸入數量不等於8");
            }
        }

        private void ResetBtn_Click(object sender, RoutedEventArgs e)
        {
            UIHelper.Instance.ResetBnNumber();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            // 使用正則表達式驗證輸入的內容，只允許數字字符
            string pattern = @"^[0-9]+$";
            if (!Regex.IsMatch(textBox.Text, pattern))
            {
                textBox.Text = ""; // 清空文本框
            }
        }
    }
}
