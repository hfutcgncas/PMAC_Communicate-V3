using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

using System.Windows;
using Pmac_Comm;

namespace PMAC_Communicate
{
    public class Pmac_ModeView:INotifyPropertyChanged
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

        public string PMAC_msg
        {
            get { return pmacCard.m_PMAC_msg; }
            set
            {
                OnPropertyChanged("PMAC_msg");
            }
        }

        public cPMAC pmacCard = new cPMAC();

        #region cmd
        public void SetT(float T)//ms
        {
            pmacCard.SendCMD("P91 = " + T.ToString());
        }
        //配合rapid程序
        public void SetX(float X)//mm
        {
            pmacCard.SendCMD("&3 !x " + X.ToString());
        }
        public void SetY(float Y)//mm
        {
            pmacCard.SendCMD("P7 = " + Y.ToString());
            pmacCard.SendCMD("P8 = " + (Y + 100).ToString());
        }
        public void SetVy(float Vy)
        {
            pmacCard.SendCMD("P71 = " + Vy.ToString());
        }

        public bool QueryIsMoving()
        {
            pmacCard.SendCMD("P4");
            if (1 == Convert.ToDouble(  pmacCard.m_PMAC_msg))
            { return true; }
            else
            { return false; }
        }
        #endregion
        

    }
}
