using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UskyPlugsFrame.Interface;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media.Animation;
using System.ComponentModel.Composition;

namespace UskyPlugsFrame.BaseShow
{
    /// <summary>
    /// 用户子插件模块基类

    /// </summary>
    [InheritedExportAttribute(typeof(IUserControlPlugs))]
    public class BaseUserControl : UserControl, IUserControlPlugs, IDisposable
    {
        /// <summary>
        /// 主窗体可视化区域
        /// </summary>
        public Panel VisualPanel { get; set; }



        #region 返回主窗体动画

        /// <summary>
        /// 返回主窗体并播放动画
        /// </summary>
        public void NavigationToMainPage()
        {
            if (VisualPanel != null)
            {
                foreach (UIElement control in VisualPanel.Children)
                {
                    DoubleAnimation showda = new DoubleAnimation(1.0d, new Duration(TimeSpan.FromMilliseconds(1200)));
                    control.BeginAnimation(OpacityProperty, showda);
                }
                UserControl ucControl = (UserControl)this;
                DoubleAnimation da_ShowMainPage = new DoubleAnimation(0d, new Duration(TimeSpan.FromMilliseconds(1000)));
                da_ShowMainPage.Completed += new EventHandler(da_ShowMainPage_Completed);
                ucControl.BeginAnimation(OpacityProperty, da_ShowMainPage);
                this.Dispose();
            }

        }



        void da_ShowMainPage_Completed(object sender, EventArgs e)
        {
            //this.Dispose();
            VisualPanel.Children.Remove(this);
            BaseMainWindow bm = Application.Current.MainWindow as BaseMainWindow;
            if (bm != null)
            {
                bm.RefreshMainWindowData();
            }
        }

        public virtual void ShiftUI()
        { }

        public virtual void RefreshData()       //更新数据，切换界面时做的工作。
        {

        }
        #endregion

        public virtual void Dispose()
        {

        }
    }
}
