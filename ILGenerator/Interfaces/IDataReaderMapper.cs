using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILGenerator.Interfaces
{
    public interface IDataRowMapper
    {
        void ReadFromDataRow(System.Data.IDataRecord record);
        void WriteToDataRow(System.Data.IDataRecord record);

    }

    
}
