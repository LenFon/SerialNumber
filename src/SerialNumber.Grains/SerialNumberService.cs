using Orleans;
using Orleans.Core;
using Orleans.Providers;
using Orleans.Runtime;
using SerialNumber.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SerialNumber.Grains
{
    [StorageProvider(ProviderName = "ef")]
    public class SerialNumberService : Grain<SerialNumber.EntityFrameworkCore.SerialNumber>, ISerialNumberService
    {
        public async Task<List<long>> GetMultiSerialNumber(int number)
        {
            if (number <= 0) { number = 1; }

            var list = new List<long>();
            for (int i = 1; i <= number; i++)
            {
                State.Number += 1L;

                list.Add(State.Number);
            }

            await WriteStateAsync();

            return list;
        }

        public async Task<long> GetSerialNumber()
        {
            State = State ?? new EntityFrameworkCore.SerialNumber()
            {
                Name = this.GetPrimaryKeyString(),
            };

            State.Number += 1;

            await WriteStateAsync();
            Console.WriteLine("hashcode:" + GetHashCode());
            return State.Number;
        }
    }
}
