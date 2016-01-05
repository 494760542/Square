using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;

namespace SOHO.Floor.Common
{
    /// <summary>
    /// 拆分List类
    /// </summary>
    public static class SplitListHelper
    {
        #region 分割集合
        /// <summary>
        /// 分割以后的集合
        /// </summary>
        public static ConcurrentQueue<List<int>> SplitList = new ConcurrentQueue<List<int>>();
        public static void SplitDic(IEnumerable<int> dic, int step)
        {
            //SplitList =new ConcurrentQueue<List<int>>();
           // ConcurrentQueue<List<int>> SplitList = new ConcurrentQueue<List<int>>();
            if (dic.Count() > step)
            {
                IEnumerable<int> keyvaluelist = dic.Take(step);
                List<int> newdic = new List<int>();
                foreach (int s in keyvaluelist)
                {
                    newdic.Add(s);
                }
                SplitList.Enqueue(newdic);
                SplitDic(dic.Skip(step), step);

            }
            else
            {
                List<int> newdic = new List<int>();
                foreach (int s in dic)
                {
                    newdic.Add(s);
                }
                SplitList.Enqueue(newdic);
            }
            //return SplitList;
        }
        #endregion

        /// <summary>
        /// 获取子集合
        /// </summary>
        /// <param name="list">源List</param>
        /// <param name="step">步长</param>
        /// <param name="index">需要获取的子集合索引</param>
        /// <returns></returns>
        public static List<int> GetChildList(int index)
        {
            //SplitDic(list, step);
            if (SplitList.Count > index)
            {
                return SplitList.ToList()[index];
            }
            else if (index < 0)
            {
                return SplitList.ToList()[0];
            }
            else
            {
                return SplitList.ToList()[SplitList.Count - 1];
            }
        }

        /// <summary>
        /// 清空队列
        /// </summary>
        public static void ClearList()
        {
            SplitList = null;
            SplitList = new ConcurrentQueue<List<int>>();
        }
    }
}
