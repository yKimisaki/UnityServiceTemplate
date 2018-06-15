using MessagePack;

namespace Tonari.UnityServiceTemplate
{
    [MessagePackObject]
    public class GlobalStream
    {
        [Key(0)]
        public string Message { get; set; }
    }
}
