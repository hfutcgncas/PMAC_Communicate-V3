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

using System.Threading;

using Pmac_Comm;
namespace PMAC_Communicate
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : UserControl 
    {
        public Pmac_ModeView mdv;

        public UserControl1()
        {
            InitializeComponent();
            mdv = new Pmac_ModeView();
        }

        #region button
        private void BT_CNT_Click(object sender, RoutedEventArgs e)
        {
            mdv.pmacCard.m_isConnected =  mdv.pmacCard.ConectToPMAC();
            mdv.PMAC_msg = mdv.pmacCard.m_PMAC_msg;
            TBK_outMsg.Text = mdv.PMAC_msg;
        }

        private void BT_SrvOn_Click(object sender, RoutedEventArgs e)
        {
            mdv.pmacCard.ServoON();
        }
        //针对新程序的改动
        private void BT_RunPro_Click(object sender, RoutedEventArgs e)
        {
           // mdv.pmacCard.RunProgram(2, 10);
            mdv.pmacCard.RunProgram(3, 12);
            mdv.pmacCard.RunProgram(4, 13);
        }

        public void BT_HitOne_Click(object sender, RoutedEventArgs e)
        {
            mdv.pmacCard.SendCMD("P4 = 1");
        }

        private void BT_Stop_Click(object sender, RoutedEventArgs e)
        {
            mdv.pmacCard.SendCMD("P3 = 0");
            mdv.pmacCard.ServoOFF();
        }

        private void BT_Send_Click(object sender, RoutedEventArgs e)
        {
            mdv.pmacCard.SendCMD(TBX_inputMsg.Text);
            mdv.PMAC_msg = mdv.pmacCard.m_PMAC_msg;
            TBK_outMsg.Text = mdv.PMAC_msg;
           //MessageBox.Show(mdv.PMAC_msg);
        }
        #endregion

        public bool Recv4Vision(double[] a)
        {
            if(! mdv.pmacCard.m_isConnected)//若没有连上，则不做处理
            { return false; }


            float T = (float)a[12]*1000; //调节时间单位到mm
            SetNotes( T.ToString() );

            bool flag_AlreadySend = (a[11] == 1) ? true : false; //判断是否是这条轨迹中第一次收到数据

            if (false == mdv.QueryIsMoving() && false == flag_AlreadySend) //只在当前机构没有运动过，且对当前来球轨迹没有响应过时，才启动y轴
            {
                mdv.SetX((float)a[1]);//设置打球参数
                mdv.SetY((float)a[2]);
                //  mdv.SetT(T);
                mdv.pmacCard.SendCMD("P4 = 1"); //发送打球命令
                return true;
            }
            else
            {
                mdv.SetX((float)a[1]);//设置打球参数，实时更新x轴而不管y轴
                return false;                
            }


        }

        private void SetNotes(string notes)
        {
            if (Dispatcher.Thread != Thread.CurrentThread)
            {
                this.TBK_VisionMsg.Dispatcher.Invoke(new Action(() =>
                {
                    this.TBK_VisionMsg.Text = notes;
                }));
            }
            else
            {
                this.TBK_VisionMsg.Text = notes;
            }
        }
    }
}
