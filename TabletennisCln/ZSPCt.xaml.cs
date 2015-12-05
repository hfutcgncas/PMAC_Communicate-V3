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

using System.ComponentModel;
using System.Threading;

namespace TabletennisCln
{
    /// <summary>
    /// Interaction logic for ZSPCt.xaml
    /// </summary>
    public partial class ZSPCt : UserControl
    {
        public ZSPCt()
        {
            InitializeComponent();

            d.ZSP_Statue = "Push to Connect";
        }




        public void BTC_WaitZSP(object sender, RoutedEventArgs e)
        {
            d.ZSP_Statue = "Wait for Connect";
            d.statueThread.Start(); //检测按键变化
            d.ZSP_Sender.StartWaitting();
        }


        public void BTC_HomeZSP(object sender, RoutedEventArgs e)
        {
            d.SendZSP_HomeCmd();
        }

        public void BTC_SevZ(object sender, RoutedEventArgs e)
        {
            d.ZSP_Sender.SendMsg("ServoZ");// Z轴开/关伺服命令
        }

        public void BTC_CloseZSP(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("here");
        
            d.ZSP_Sender.SendMsg("CloseZSP");// 关闭远端程序命令
        }

        public void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            BTC_CloseZSP(this, null);

            d.statueThread.Abort();

            d.ZSP_Sender.CloseSocket();

        }


        #region 外部接口
        public void SetZSP(int z, int s, int p)
        {
            d.Hit_Z = z;
            d.Hit_S = s;
            d.Hit_P = p;
        }
        #endregion

    }

    public class data  : INotifyPropertyChanged//实现接口，详细表述MSDN
    {
        //INotifyPropertyChanged Members 不需更改---------------------------------------------------------
        #region INotifyPropertyChanged Members

        /// <summary>
        /// Event to which the view's controls will subscribe.
        /// This will enable them to refresh themselves when the binded property changes provided you fire this event.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// When property is changed call this method to fire the PropertyChanged Event
        /// </summary>
        /// <param name="propertyName"></param>
        public void OnPropertyChanged(string propertyName)
        {
            //Fire the PropertyChanged event in case somebody subscribed to it
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        public Thread statueThread;

        public data()
        {
            ZSP_Sender = new cTCPListenor();
            Hit_Z = 300;
            Hit_S = 127;
            Hit_P = 127;

            statueThread = new Thread(new ThreadStart(ThreadWork));
            

        }

        public void ThreadWork()
        {
            while(ZSP_Sender!=null)
            {
                if (ZSP_Sender.isConected)
                {
                    ZSP_Statue = "Connected";
                }
                else
                {
                    ZSP_Statue = "Wait for Connect";
                }
                Thread.Sleep(1000);
            }
        }


        #region MAX-MIN
        public  int Z_MAX 
        {
            get { return 425; }
            set {}
        }

        public  int Z_MIN
        {
            get { return 245; }
            set { }
        }

        public  int S_MAX 
        {
            get { return 195; }
            set {}
        }
        public int S_MIN
        {
            get { return 85; }
            set { }
        }

        public  int P_MAX
        {
            get { return 195; }
            set { }
        }
        public int P_MIN
        {
            get { return 65; }
            set { }
        }
        #endregion

        public cTCPListenor ZSP_Sender;

        private int hit_Z;
        public int Hit_Z
        {
            get { return hit_Z; }
            set
            {
                hit_Z = value;
                this.OnPropertyChanged("Hit_Z");
                SendZSP_PosCmd();
            }
        }
        private int hit_S;
        public int Hit_S
        {
            get { return hit_S; }
            set
            {
                hit_S = value;
                OnPropertyChanged("Hit_S");
                SendZSP_PosCmd();
            }
        }
        private int hit_P;
        public int Hit_P
        {
            get { return hit_P; }
            set
            {
                hit_P = value;
                OnPropertyChanged("Hit_P");
                SendZSP_PosCmd();
            }
        }

        private string zsp_statue;
        public string ZSP_Statue
        {
            get { return zsp_statue; }
            set
            {
                zsp_statue = value;
                OnPropertyChanged("ZSP_Statue");
            }
        }




        public void SendZSP_PosCmd()
        {
            //if (Hit_Z > Z_MAX || Hit_Z < Z_MIN
            //|| Hit_S > S_MAX || Hit_S < S_MIN
            //|| Hit_P > P_MAX || Hit_P < P_MIN)
            //{
            //    Console.WriteLine("ZSP 超限");
            //    return;
            //}
            if (Hit_Z > Z_MAX) { Hit_Z = Z_MAX; }
            if (Hit_Z < Z_MIN) { Hit_Z = Z_MIN; }
            if (Hit_S > S_MAX) { Hit_S = S_MAX; }
            if (Hit_S < S_MIN) { Hit_S = S_MIN; }
            if (Hit_P > P_MAX) { Hit_P = P_MAX; }
            if (Hit_P < P_MIN) { Hit_P = P_MAX; }

            ZSP_Sender.SendMsg("z" + (hit_Z - Z_MIN).ToString() + "s" + hit_S.ToString() + "p" + hit_P.ToString() + "E");//这里- Constants.ZMin是为了满足UCHAR的类型范围。

        }
       
        public void SendZSP_HomeCmd()
        {
            ZSP_Sender.SendMsg("ZHomeE");//Z轴复位命令
            Hit_Z = 300;
            Hit_S = 127;
            Hit_P = 127;
        }
    }
}
