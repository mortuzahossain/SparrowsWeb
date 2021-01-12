using System;

namespace GlobalEntities.Entities
{
    public class SPParameter
    {
        public Int32 Id { get; set; }
        public String Name { get; set; }
        public Int32 Length { get; set; }
        public String Type { get; set; }
        public String Value { get; set; } 
    }
    public class CommonObject
    {
        public string Id { get; set; }
        public string  Name { get; set; }
        public string Description { get; set; }
    }
}
