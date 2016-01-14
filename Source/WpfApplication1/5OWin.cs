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
namespace WpfApplication1
{
    class _5OWin
    {
         //5 ô liên tiếp
        public Node[] GiaTri;
        int thuTu;
        public _5OWin()
        {
            GiaTri = new Node[10];
            for (int i = 0; i < 10; i++)
            {
                GiaTri[i] = new Node();
            }
            thuTu = 0;
        }
        public void Add(Node n)
        {
            GiaTri[thuTu] = n;
            thuTu++;
        }
        public void Reset()
        {
            thuTu = 0;
            GiaTri = new Node[10];
            for (int i = 0; i < 10; i++)
            {
                GiaTri[i] = new Node();
            }
        }
    }
}
