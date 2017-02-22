using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolenoidTester
{
    public interface Subject
    {
        /// <summary>
        /// 注册观察者
        /// </summary>
        /// <param name="o">观察者对象</param>
        void RegisterObserver(Observer o);

        /// <summary>
        /// 注销观察者
        /// </summary>
        /// <param name="o">观察者对象</param>
        void RemoveObserver(Observer o);

        /// <summary>
        /// 通知观察者
        /// </summary>
        void NotifyObservers();
    }

}
