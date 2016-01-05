using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UskyPlugsFrame.Interface;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.ComponentModel.Composition;
using PixelLab.Wpf.Transitions;
using Microsoft.Surface.Presentation.Controls;

namespace UskyPlugsFrame.BaseShow
{
    /// <summary>
    /// 用户主窗体模块基类


    /// </summary>
    [InheritedExportAttribute(typeof(IMainWindowPlugs))]
    public class BaseMainWindow : SurfaceWindow, IMainWindowPlugs
    {
        public  Dictionary<string, string> ConfigDic = new Dictionary<string, string>();
        Transition[] transitions = null;
        public BaseMainWindow()
        {
            try
            {
                ResourceDictionary resourceDictionary = new ResourceDictionary();
                resourceDictionary.Source = new Uri("/UskyPlugsFrame.BaseShow;component/Themes/Generic.xaml", UriKind.RelativeOrAbsolute);
                transitions = (Transition[])resourceDictionary["Transitions"];
            }
            catch
            {

            }

        }

        /// <summary>
        /// 子插件模块列表

        /// </summary>
        public ObservableCollection<IUserControlPlugs> UserControlList { get; set; }

        /// <summary>
        /// 子插件序号和对应类名字典
        /// </summary>
        public Dictionary<int, string> UserControlNameDic = new Dictionary<int, string>();

        /// <summary>
        /// 当前用户控件
        /// </summary>
        public BaseUserControl CurrentUserControl { get; set; }

        /// <summary>
        /// 当前用户控件名

        /// </summary>
        public string CurrentUserControlName { get; set; }

        /// <summary>
        /// 动画切换枚举
        /// </summary>
        public TransitionName transitionName { get; set; }

        /// <summary>
        /// 根据子插件ID查找对应子插件类名


        /// </summary>
        /// <param name="id">子插件ID</param>
        /// <returns>对应的子插件类名</returns>
        protected string GetUserControlNameFromDic(int id)
        {
            if (UserControlNameDic.ContainsKey(id))
            {
                return UserControlNameDic[id];
            }
            else
            {
                return null;
            }
        }

        #region 加载子控件插件动画


        /// <summary>
        /// 加载子插件并播放动画
        /// </summary>
        /// <param name="userControlClassName">子插件模块完整类名</param>
        protected void NavigationToUserPage(string userControlClassName)
        {
            Panel parentVisualPanel = null;
            for (int i = 0; i < this.VisualChildrenCount; i++)
            {
                parentVisualPanel = (Panel)this.GetVisualChild(i);
                foreach (UIElement u in parentVisualPanel.Children)
                {
                    DoubleAnimation da = new DoubleAnimation(0d, new Duration(TimeSpan.FromMilliseconds(1000)));
                    u.BeginAnimation(OpacityProperty, da);
                }
            }
            foreach (var control in UserControlList)                                                //如果传过来的插件名字在插件列表中有有则进行清空容器，把这个插件加进去，并且播放动画

            {
                if (control.GetType().FullName.ToString().Equals(userControlClassName))
                {
                    BaseUserControl userControl = (BaseUserControl)control;
                    userControl.VisualPanel = parentVisualPanel;
                    userControl.Opacity = 0;
                    // parentVisualPanel.Children.Clear();
                    parentVisualPanel.Children.Add(userControl);
                    DoubleAnimation showda = new DoubleAnimation(1.0d, new Duration(TimeSpan.FromMilliseconds(1200)));
                    userControl.BeginAnimation(OpacityProperty, showda);

                }
            }
        }
        bool HaveTp_Content = false;
        Panel parentVisualPanel { get; set; }
        /// <summary>
        /// 将用户控件添加到主界面的某个区域内

        /// </summary>
        /// <param name="panel">主界面的区域</param>
        /// <param name="userControlClassName">用户控件名</param>
        protected void NavigationToUserPage(Panel panel, string userControlClassName)
        {
            parentVisualPanel = panel;
            TransitionPresenter tp_Content = new TransitionPresenter();
            //parentVisualPanel.Children.Clear();
            foreach (UIElement u in parentVisualPanel.Children)
            {
                if (u.Uid == "tp_Content")
                {
                    HaveTp_Content = true;
                    tp_Content = (TransitionPresenter)u;
                }
                //DoubleAnimation da = new DoubleAnimation(0d, new Duration(TimeSpan.FromMilliseconds(1000)));
                //u.BeginAnimation(OpacityProperty, da);
            }
            if (!HaveTp_Content)
            {
                tp_Content.Uid = "tp_Content";
                tp_Content.RenderSize = new System.Windows.Size(panel.ActualWidth, panel.ActualHeight);
                parentVisualPanel.Children.Add(tp_Content);
            }

            //tp_Content.Transition = transitions[2];
            foreach (var item in transitions)
            {
                Transition transition = (Transition)item;
                if (TextSearch.GetText(transition) == transitionName.ToString())
                {
                    tp_Content.Transition = transition;
                }
            }

            foreach (var control in UserControlList)
            {
                if (control.GetType().FullName.ToString().Equals(userControlClassName))
                {
                    BaseUserControl userControl = (BaseUserControl)control;
                    da_Completed();
                    CurrentUserControl = userControl;
                    CurrentUserControlName = userControlClassName;
                    userControl.VisualPanel = parentVisualPanel;
                    tp_Content.Content = userControl;
                    userControl.ShiftUI();                                          //每次执行切换界面必须执行这个函数
                }
            }
        }

        protected void UpdateData()
        {
            BaseUserControl userControl = CurrentUserControl as BaseUserControl;
            if (userControl == null) return;
            userControl.RefreshData();
        }


        /// <summary>
        /// 把主界面的插件控件资源释放，并移出去
        /// </summary>
        private void da_Completed()
        {
            for (int i = 0; i < parentVisualPanel.Children.Count; i++)
            {
                if (parentVisualPanel.Children[i] is BaseUserControl)
                {
                    BaseUserControl userControl = (BaseUserControl)parentVisualPanel.Children[i];
                    userControl.Dispose();
                    parentVisualPanel.Children.Remove(userControl);
                }


            }

        }
        //刷新主界面数据


        public virtual void RefreshMainWindowData()
        {

        }

        #endregion

    }
    public enum TransitionName
    {

        FadeWipe = 0,
        Star = 1,
        Melt = 2,
        HorizontalWipe = 3,
        VerticalWipe = 4,
        DiagonalWipe = 5,
        RotateWipe = 6,
        DoubleRotateWipe = 7,
        VerticalBlinds = 8,
        HorizontalBlinds = 9,
        Diamonds = 10,
        Checkerboard = 11,
        Dots = 12,
        FadeAndGrow = 13,
        Roll = 14,
        Translate2D = 15,
        Flip3D = 16,
        Door3D = 17,
        Rotate3D = 18,
        Spin3D = 19,
        Explosion3D = 20,
        Cloth = 21
    }
}
