﻿using System;
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

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for Messege.xaml
    /// </summary>
    public partial class Messege : UserControl
    {
        private string time;

        public string Time
        {
            get { return time; }
            set { time = value; }
        }


        private string message;

        public string Message
        {
            get { return message; }
            set { message = value; }
        }
        public Messege()
        {
            InitializeComponent();
        }
         public Messege(string playerName, string time, string message)
        {
            InitializeComponent();
            lblName.Content = playerName;
            this.time = time;
            lblTime.Content = time;
            this.message = message;
            lblMessage.Content = message;
        }
    }
}
