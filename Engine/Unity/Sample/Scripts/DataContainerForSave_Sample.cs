using FlexiArchiveSystem.Extension;

namespace FlexiArchiveSystem.Sample
{
    public class FlexiDataSave : FlexiDataContainer
    {
        public string author = "温文";

        public FlexiDataSave(IFlexiDataArchiveManager dataArchiveManager) : base(dataArchiveManager)
        {
			
        }
    }
}