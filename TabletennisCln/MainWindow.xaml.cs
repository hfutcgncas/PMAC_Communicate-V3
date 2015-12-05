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

using System.Diagnostics;
using System.Threading;

using System.Windows.Threading;

namespace TabletennisCln
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Thread RcvThread;
        public MainWindow()
        {
            InitializeComponent();
            vision = new cVision();
            ZSPCt.BTC_WaitZSP(this, null);//自动打开端口，等待ZSP连接
        }
        //=======================================

        private void Window_Closed(object sender, EventArgs e)
        {
            if (RcvThread != null && RcvThread.IsAlive)
            {
                if (vision != null) { vision.newsock.Close(); }
                RcvThread.Abort();
            }    
   

                ZSPCt.UserControl_Unloaded(this, null);
                Thread.Sleep(100);

        }

        #region 视觉部分代码
        private readonly cVision vision; //接收视觉结果

        public void ConectToVision()
        {
            string notes;
            vision.initRcv();
            RcvThread = new Thread(new ThreadStart(delegate
            {
                while (true)
                {
                    vision.ReciveData();
                    notes = " X  = " + vision.hitPar[1].ToString() +
                                   "\n Y  = " + vision.hitPar[2].ToString() +
                                   "\n Z  = " + vision.hitPar[3].ToString() +
                                   "\n Vx = " + vision.hitPar[4].ToString() +
                                   "\n Vy = " + vision.hitPar[5].ToString() +
                                   "\n Vz = " + vision.hitPar[6].ToString() +
                                   "\n tg = " + vision.hitPar[12].ToString();
                    if (vision.hitPar[0] == 1)
                    {
                        double[] padPar;
                        SlovePadPram(vision.hitPar, out padPar);
                        notes += "\n PadV = " + padPar[3].ToString() +
                                 "\n PadS = " + padPar[4].ToString();
                        SetNotes(notes);


                        Control_Arm.Recv4Vision(vision.hitPar);
                        ZSPCt.SetZSP((int)vision.hitPar[3],(int)(padPar[4]),127);
                    }
                    Thread.Sleep(50);
                }
            }
            ));
            RcvThread.Start();

        }
   
        //------------------------------
        private void SetNotes(string notes)
        {
            if (Dispatcher.Thread != Thread.CurrentThread)
            {
                this.TBK_VisionOut.Dispatcher.Invoke(new Action(() =>
                {
                    this.TBK_VisionOut.Text = notes;
                }));
            }
            else
            {
                this.TBK_VisionOut.Text = notes;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ConectToVision();
        }
        #endregion

        
        #region 回球决策代码
        public static double lambda = 0.9;
        public static double length = 0.5;

        private static bool SlovePadPram(double[] inputBall, out double[] output)
        {
            
            output = new double[6];

            double padX = inputBall[1];
            double padY = inputBall[2];
            double padZ = inputBall[3]/1000; 
            double ballVx_In = inputBall[4] / 1000;
            double ballVy_In = inputBall[5] / 1000;
            double ballVz_In = inputBall[6] / 1000;

            double g = 9.802;

            //解方程 -1/2 * g * t^2 + Vz * t = -padZ
            double a = -0.5 * g; double b = lambda*ballVz_In; double c = padZ;
            double delta = b * b - 4 * a * c;

            if (delta < 0) return false;//解算失败

            double t1 = (-b - Math.Sqrt(delta)) / (2 * a);
            double t2 = (-b + Math.Sqrt(delta)) / (2 * a);

            double t = (t1 > t2) ? t1 : t2;

            double ballV_Out = length / t;

            double hitV = ballV_Out/lambda;

            double padVy = Math.Sqrt((hitV * hitV) - (ballVx_In * ballVx_In)) + ballVy_In;

            double padS = -Math.Acos(ballVx_In / hitV)/Math.PI * 180 - 90 + 127;

            output[0] = padX;
            output[1] = padY;
            output[2] = padZ;
            output[3] = padVy; //回球速度
            output[4] = padS;  //回球球拍偏转方向
            output[5] = 0;

            Console.WriteLine("t = "+t.ToString() );
            return true;
        }
        #endregion

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            double length_tmp = Convert.ToDouble(TB_lambda.Text);
            if (length_tmp == 0)
            {
                Console.WriteLine("Set length wrong");
                return;
            }
            length = length_tmp;
           
        }

    }
}
