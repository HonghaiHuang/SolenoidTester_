using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolenoidTester
{
    public interface Observer
    {
        /// <summary>
        /// 向注册对象更新数据
        /// </summary>
        /// <param name="data">更新数据</param>
        void Update(string[,] data);
    }
}
