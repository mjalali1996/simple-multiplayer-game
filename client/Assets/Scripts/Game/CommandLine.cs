namespace Game
{
    public static class CommandLine
    {
        public class Args
        {
            public string Mode;
            public string IpAddress;
            public ushort Port;
        }
        
        public static Args Arguments = null;
    }
}