using System;

namespace FlexiArchiveSystem.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class FlexiDataBinderAttribute : Attribute
    {
        public string GroupKey;
        public string SubKey;
		
        public FlexiDataBinderAttribute(string GroupKey, string SubKey)
        {
            this.GroupKey = GroupKey;
            this.SubKey = SubKey;
        }
    }
	
    [AttributeUsage(AttributeTargets.Class)]
    public class UsageFlexiDataContainerAttribute : Attribute
    {
        public UsageFlexiDataContainerAttribute() { }
    }
}