namespace FlexiArchiveSystem
{
    public class DataLong : AbstractDataType<long>
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