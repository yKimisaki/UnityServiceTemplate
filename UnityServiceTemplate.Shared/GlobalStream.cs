using MessagePack;

namespace UnityServiceTemplate
{
    [MessagePackObject]
    public class GlobalStream
    {
        [Key(0)]
        public string Message { get; set; }
    }
}
