using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EmailSender.Views
{
    /// <summary>
    /// Interaction logic for AutoAnswerView.xaml
    /// </summary>
    public partial class AutoAnswerView : UserControl
    {
        DoubleAnimation widthRiceAnimationTextBox;
        DoubleAnimation heighRiceAnimationTextBox;


        public AutoAnswerView()
        {
            InitializeComponent();

            widthRiceAnimationTextBox = new DoubleAnimation();
            widthRiceAnimationTextBox.From = 300;
            widthRiceAnimationTextBox.To = 400;
            widthRiceAnimationTextBox.Duration = TimeSpan.FromSeconds(1);

            heighRiceAnimationTextBox = new DoubleAnimation();
            heighRiceAnimationTextBox.From = 100;
            heighRiceAnimationTextBox.To = 400;
            heighRiceAnimationTextBox.Duration = TimeSpan.FromSeconds(1);

            answTitlesTextBox.GotFocus += OnFocus;
            answBodyTextBox.GotFocus += OnFocus;
            filterBodyTextBox.GotFocus += OnFocus;
            filterTitleTextBox.GotFocus += OnFocus;

            answTitlesTextBox.LostFocus += OnLostFocus;
            answBodyTextBox.LostFocus += OnLostFocus;
            filterBodyTextBox.LostFocus += OnLostFocus;
            filterTitleTextBox.LostFocus += OnLostFocus;
        }


        private void OnFocus(object sender, EventArgs e)
        {
            if(sender is TextBox textbox)
            {
                textbox.BeginAnimation(TextBox.WidthProperty, widthRiceAnimationTextBox);
                textbox.BeginAnimation(TextBox.HeightProperty, heighRiceAnimationTextBox);
            }
            
            
        }

        private void OnLostFocus(object sender, EventArgs e)
        {
            if (sender is TextBox textbox)
            {
                textbox.BeginAnimation(TextBox.HeightProperty, null);
                textbox.BeginAnimation(TextBox.WidthProperty, null);
            }
        }
    }
}
