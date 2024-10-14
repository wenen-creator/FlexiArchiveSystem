using FlexiArchiveSystem.DataType.Base;

namespace FlexiArchiveSystem
{
    public class DataLong : AbstractDataTypeWrapper<long>
    {
        public DataLong(string dataStr) : base(dataStr)
        {
                
        }

        public override bool Equals(long another)
        {
            return another.Equals(data);
        }
    }
}