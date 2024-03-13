using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arvin.AITest
{
    /// <summary>
    /// MLNET框架帮助类
    /// 常用操作语法糖
    /// </summary>
    public class MLNETHelper
    {
        #region 构造函数
        public MLContext Context { get; private set; }
        public MLNETHelper()
        {
            this.Context = new MLContext();
        }
        public MLNETHelper(int seed)
        {
            this.Context = new MLContext(seed);
        }
        #endregion

        #region 常用操作
        /// <summary>
        /// 加载数据集
        /// </summary>
        /// <param name="dataPath"></param>
        public void LoadData(string dataPath)
        {

        }
        #endregion
    }
}
