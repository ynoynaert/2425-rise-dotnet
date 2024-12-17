using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rise.Shared.Quotes;

public interface ITradedMachineryService
{
    Task<TradedMachineryResult.Create> CreateTradedMachineryAsync(TradedMachineryDto.Create tradedMachineryDto);
}
