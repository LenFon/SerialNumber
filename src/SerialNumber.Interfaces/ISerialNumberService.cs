using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SerialNumber.Interfaces
{
    public interface ISerialNumberService : Orleans.IGrainWithStringKey
    {
        /// <summary>
        /// 获取多个流水号
        /// </summary>
        /// <param name="number">数量</param>
        /// <returns></returns>
        Task<List<long>> GetMultiSerialNumber(int number);


        /// <summary>
        /// 获取一个流水号
        /// </summary>
        /// <returns></returns>
        Task<long> GetSerialNumber();
    }
}
