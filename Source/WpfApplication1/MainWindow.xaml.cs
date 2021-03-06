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
using Quobject.SocketIoClientDotNet.Client;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.ComponentModel;
using System.Configuration;
namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BanCo banco;
        public static int rowngaunhien, colngaunhien;
        private LuongGia eBoard; //Bảng lượng giá bàn cờ
        Node node = new Node();
        public static TextBox txt = new TextBox();
        public static TextBox txtMay = new TextBox();
        public static TextBox txtNguoiMay = new TextBox();
        public static string message;
        public static int dem = 0;
        public static int dem1 = 0;
        public static string name;
        public static bool newgame = false;
        public static bool newgame1 = false;
        public static Quobject.SocketIoClientDotNet.Client.Socket socket;
        public MainWindow()
        {
            this.InitializeComponent();
            banco = new BanCo(this, grdBanCo);
            BanCo.Option.WhoPlayWith = Player.MayOnline;
            banco.DrawGomokuBoard();
            grdBanCo.MouseDown += new System.Windows.Input.MouseButtonEventHandler(banco.grdBanCo_MouseDown);
            BanCo.WinEvent += new BanCo.WinEventHander(banco_WinEvent);
            BanCo.LoseEvent += new BanCo.LoseEventHander(banco_LoseEvent);

            txt.TextChanged += new TextChangedEventHandler(txt_change);
            txtMay.TextChanged += new TextChangedEventHandler(txtMay_Change);
            txtNguoiMay.TextChanged += new TextChangedEventHandler(txtNguoiMay_Change);
        }
         private void txtNguoiMay_Change(object sender, TextChangedEventArgs e)
        
        {
            btnmayonline.Content = "Change";
            BanCo.Option.GamePlay = LuatChoi.Vietnamese;
            BanCo.Option.WhoPlayWith = Player.MayOnline;
          


            if (BanCo.Option.WhoPlayWith == Player.MayOnline)
            {
                if (BanCo.currPlayer == Player.Com )//Nếu lượt đi là máy và trận đấu chưa kết thúc
                {

                    //Tìm đường đi cho máy

                    BanCo.DiNgauNhien();
                    connect.rw1 = rowngaunhien;
                    connect.cl1 = colngaunhien;
                    connect.guitoado(socket, rowngaunhien,colngaunhien);
                    BanCo.currPlayer = Player.Human;                   
                }
            }
        }
        private void txtMay_Change(object sender, TextChangedEventArgs e)
        {
            if (BanCo.Option.WhoPlayWith == Player.MayOnline)
            {

                
                if (BanCo.currPlayer == Player.Human && BanCo.end == Player.None)
                {

                    BanCo.board[BanCo.rows, BanCo.columns] = BanCo.currPlayer;//Lưu loại cờ vừa đánh vào mảng
                    BanCo.DrawDataBoard(BanCo.rows, BanCo.columns, true, true);
                    BanCo.end = BanCo.CheckEnd(BanCo.rows, BanCo.columns);//Kiểm tra xem trận đấu kết thúc chưa

                    if (BanCo.end == Player.Human)//Nếu người chơi 2 thắng
                    {
                        BanCo.OnWin();//Khai báo sư kiện Win
                        BanCo.OWinorLose();//Hiển thị 5 ô Win.
                        btnmayonline.Content = "New game";
                        newgame = true;
                    }
                    else
                    {
                        BanCo.currPlayer = Player.Com;//Thiết lập lại lượt chơi
                        BanCo.OnComDanhXong();// Khai báo sự kiện người chơi 2 đánh xong
                    }
                }
                if (BanCo.currPlayer == Player.Com && BanCo.end == Player.None)//Nếu lượt đi là máy và trận đấu chưa kết thúc
                {

                    //Tìm đường đi cho máy

                    BanCo.eBoard.ResetBoard();
                    BanCo.LuongGia(Player.Com);//Lượng giá bàn cờ cho máy
                    node = BanCo.eBoard.GetMaxNode();//lưu vị trí máy sẽ đánh
                    int r, c;
                    r = node.Row; c = node.Column;
                    connect.rw1 = r;
                    connect.cl1 = c;
                    BanCo.board[r, c] = BanCo.currPlayer; //Lưu loại cờ vừa đánh vào mảng
                    BanCo.DrawDataBoard(r, c, true, true); //Vẽ con cờ theo lượt chơi
                    connect.guitoado(socket, r, c);
                    BanCo.end = BanCo.CheckEnd(r, c);//Kiểm tra xem trận đấu kết thúc chưa

                    if (BanCo.end == Player.Com)//Nếu máy thắng
                    {
                        BanCo.OnLose();//Khai báo sư kiện Lose
                        BanCo.OWinorLose();//Hiển thị 5 ô Lose.
                        btnmayonline.Content = "New game";
                        newgame = true;
                    }
                    else if (BanCo.end == Player.None)
                    {
                        BanCo.currPlayer = Player.Human;//Thiết lập lại lượt chơi
                        BanCo.OnComDanhXong();// Khai báo sự kiện người đánh xong
                    }
                }
            }
        }
         private void txt_change(object sender, TextChangedEventArgs e)
        {
            if(BanCo.currPlayer==Player.Online && BanCo.end==Player.None)
            {
                BanCo.board[BanCo.rows, BanCo.columns] = BanCo.currPlayer;//Lưu loại cờ vừa đánh vào mảng
                BanCo.DrawDataBoard(BanCo.rows, BanCo.columns, true, true);
                BanCo.end = BanCo.CheckEnd(BanCo.rows, BanCo.columns);//Kiểm tra xem trận đấu kết thúc chưa

                if (BanCo.end == Player.Online)//Nếu người chơi 2 thắng
                {
                    BanCo.OnWin();//Khai báo sư kiện Win
                    BanCo.OWinorLose();//Hiển thị 5 ô Win.
                    Messege Messege = new Messege("Server", DateTime.Now.ToString("hh:mm:ss tt"), name+ " là người thắng");
                    chatBox.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                    chatBox.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                    chatBox.Items.Add(Messege);
                    btnonline.Content = "New game";
                    newgame1 = true;
                }
                else
                {
                    BanCo.currPlayer = Player.Human;//Thiết lập lại lượt chơi
                    BanCo.OnComDanhXong();// Khai báo sự kiện người chơi 2 đánh xong
                }
                    
               

            }
        }

        private void banco_LoseEvent()
        {
            if (BanCo.Option.WhoPlayWith == Player.Com && banco.End==Player.Com)
            {
                Messege Messege = new Messege("Server", DateTime.Now.ToString("hh:mm:ss tt"), "Cố lên nhé");
                chatBox.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                chatBox.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                chatBox.Items.Add(Messege);
            }
            if (banco.End == Player.Com && BanCo.Option.WhoPlayWith == Player.MayOnline)
            {
                string temp2 = txtYourName.Text + "là người chiến thắng";

                Messege Messege = new Messege("Server", DateTime.Now.ToString("hh:mm:ss tt"), temp2);
                chatBox.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                chatBox.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                chatBox.Items.Add(Messege);
            }

        }

       
        private void banco_WinEvent()
        {

            if (banco.End == Player.Human && BanCo.Option.WhoPlayWith == Player.Com|| banco.End == Player.Human && BanCo.Option.WhoPlayWith == Player.Human)
            {
                string temp1 =  BanCo.Option.PlayerAName + " " + "là người chiến thắng";

                    Messege Messege = new Messege("Server", DateTime.Now.ToString("hh:mm:ss tt"), temp1);
                    chatBox.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                    chatBox.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                    chatBox.Items.Add(Messege);
           }
            if (banco.End == Player.Com && BanCo.Option.WhoPlayWith == Player.Human)
            {
                string temp2 =  BanCo.Option.PlayerBName + "là người chiến thắng";

                    Messege Messege = new Messege("Server", DateTime.Now.ToString("hh:mm:ss tt"), temp2);
                    chatBox.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                    chatBox.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                    chatBox.Items.Add(Messege);
            }
            if (banco.End == Player.Human && BanCo.Option.WhoPlayWith == Player.Online)
            {
                btnonline.Content = "New game";
                string temp2 = txtYourName.Text + "là người chiến thắng";

                Messege Messege = new Messege("Server", DateTime.Now.ToString("hh:mm:ss tt"), temp2);
                chatBox.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                chatBox.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                chatBox.Items.Add(Messege);
            }
            if (banco.End == Player.Online && BanCo.Option.WhoPlayWith == Player.Online)
            {
                string temp2 = MainWindow.name + "là người chiến thắng";

                Messege Messege = new Messege("Server", DateTime.Now.ToString("hh:mm:ss tt"), temp2);
                chatBox.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                chatBox.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                chatBox.Items.Add(Messege);
            }
            if (banco.End == Player.Human && BanCo.Option.WhoPlayWith == Player.MayOnline)
            {
                string temp2 = MainWindow.name + "là người chiến thắng";

                Messege Messege = new Messege("Server", DateTime.Now.ToString("hh:mm:ss tt"), temp2);
                chatBox.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                chatBox.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                chatBox.Items.Add(Messege);
            }
           
     
           
        }
        public int test;
        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
           
            if (rdbhuman.IsChecked == false && rdbcomputer.IsChecked == false)
            {
                MessageBox.Show("Chọn chế độ chơi", "Thông báo");

            }
            else
            {
                if (test == 0 && txtname2.Text == "Máy" && txtname1.Text != "")
                {

                     BanCo.Option.PlayerAName = txtname1.Text;
                     BanCo.Option.PlayerBName = txtname2.Text;
                    Messege Messege = new Messege("Server", DateTime.Now.ToString("hh:mm:ss tt"), "Trò chơi bắt đầu");
                    chatBox.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                    chatBox.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                    chatBox.Items.Add(Messege);
                    banco.NewGame();

                }
                else if (txtname1.Text != "" && txtname2.Text != "" && test == 1)
                {
                    BanCo.Option.PlayerAName = txtname1.Text;
                    BanCo.Option.PlayerBName = txtname2.Text;
                    Messege Messege = new Messege("Server", DateTime.Now.ToString("hh:mm:ss tt"), "Trò chơi bắt đầu");
                    chatBox.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                    chatBox.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                    chatBox.Items.Add(Messege);
                    banco.NewGame();
                }
                else
                {
                    MessageBox.Show(" Chưa nhập tên người chơi", "Thông báo");
                }
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        
        private void rdbhuman_Checked(object sender, RoutedEventArgs e)
        {
             BanCo.Option.WhoPlayWith = Player.Human;
            test = 1;
            txtname1.Text = "";
            txtname2.Text = "";
        }

        private void rdbcomputer_Checked(object sender, RoutedEventArgs e)
        {
             BanCo.Option.WhoPlayWith = Player.Com;
            test = 0;
            txtname2.Text = "Máy";
            txtname1.Text = "";
        }
        public string Get_time()
        {
            string str = DateTime.Now.ToShortTimeString();
            return str;
        }
        private void btnPlayerAgain_Click(object sender, RoutedEventArgs e)
        {
                Messege Messege = new Messege("Server", DateTime.Now.ToString("hh:mm:ss tt"), "Ván mới");
                chatBox.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                chatBox.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                chatBox.Items.Add(Messege);   
            MessageBox.Show("Bắt đầu chơi !!!","Thông báo");
            banco.PlayAgain();
        }

        private void txtMessage_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtMessage.Text == "Type your message here...")
            {
                txtMessage.Text = "";
                txtMessage.Foreground = new SolidColorBrush(Colors.Black);
            }
        }
        private void txtYourName_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (txtYourName.Text == "")
            {
                txtYourName.Text = "Guest";
            }
        }
        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
              
            connect.sendmessage(socket, txtYourName.Text,txtMessage.Text);
        }

        private void txtMessage_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (txtMessage.Text == "")
            {
               
                txtMessage.Foreground = new SolidColorBrush(Color.FromRgb(135, 135, 135));
                txtMessage.Text = "Type your message here...";
            }
           
        }
       
       
        private void btnonline_Click(object sender, RoutedEventArgs e)
        {
            if (dem == 0)
            {
                socket = IO.Socket(ConfigurationManager.ConnectionStrings["chuoi"].ConnectionString);
                btnonline.Content = "Change";
                BanCo.Option.GamePlay = LuatChoi.Vietnamese;
                BanCo.Option.WhoPlayWith = Player.Online;
               
                 BanCo.currPlayer = Player.Online;
                 connect.connected(socket, txtYourName.Text.ToString());
                dem++;
            }
            else
            {
                connect.changname(socket, txtYourName.Text);
            }
            if (newgame1 == true)
            {
                Messege Messege = new Messege("Server", DateTime.Now.ToString("hh:mm:ss tt"), "Ván mới");
                chatBox.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                chatBox.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                chatBox.Items.Add(Messege);
                MessageBox.Show("Bắt đầu chơi !!!", "Thông báo");
                banco.PlayAgain();
                socket = IO.Socket(ConfigurationManager.ConnectionStrings["chuoi"].ConnectionString);
                btnonline.Content = "Change";
                BanCo.Option.GamePlay = LuatChoi.Vietnamese;
                BanCo.Option.WhoPlayWith = Player.Online;
                BanCo.currPlayer = Player.Online;
                connect.connected(socket, txtYourName.Text.ToString());
                connect.rw = -1;
                connect.cl = -1;
                newgame1 = false;
            }
          
        }

        private void chatBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
          
        }

        private void Message_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (MainWindow.name != null)
            {
                Messege Messege = new Messege(MainWindow.name, DateTime.Now.ToString("hh:mm:ss tt"), message);
                chatBox.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                chatBox.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                chatBox.Items.Add(Messege);
            }
            else
            {
                Messege Messege = new Messege("Server", DateTime.Now.ToString("hh:mm:ss tt"), message);
                chatBox.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                chatBox.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                chatBox.Items.Add(Messege);
            }
        }

        private void btnmayonline_Click(object sender, RoutedEventArgs e)
        {
            if (dem1 == 0)
            {
                socket = IO.Socket(ConfigurationManager.ConnectionStrings["chuoi"].ConnectionString);
                btnmayonline.Content = "Change";
                BanCo.Option.GamePlay = LuatChoi.Vietnamese;
                BanCo.Option.WhoPlayWith = Player.MayOnline;
                BanCo.currPlayer = Player.Human;
                connect.connected(socket, txtYourName.Text.ToString());

                dem1++;
            }
            else
            {
                connect.changname(socket, txtYourName.Text);

            }
            if (newgame == true)
            {
                Messege Messege = new Messege("Server", DateTime.Now.ToString("hh:mm:ss tt"), "Ván mới");
                chatBox.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                chatBox.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                chatBox.Items.Add(Messege);
                MessageBox.Show("Bắt đầu chơi !!!", "Thông báo");
                banco.PlayAgain();
                socket = IO.Socket(ConfigurationManager.ConnectionStrings["chuoi"].ConnectionString);
                btnmayonline.Content = "Change";
                BanCo.Option.GamePlay = LuatChoi.Vietnamese;
                BanCo.Option.WhoPlayWith = Player.MayOnline;
                BanCo.currPlayer = Player.Human;
                connect.connected(socket, txtYourName.Text.ToString());
                connect.rw1 = -1;
                connect.cl1 = -1;
                newgame = false;
            }
        }
    }
}
