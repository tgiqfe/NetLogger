
namespace NetLogger.Logs
{
    internal class LogbodyBase
    {
        /// <summary>
        /// ログファイル名の接頭部分
        /// 継承するクラス側で、同じ名前で定数指定する。
        /// パラメータ指定の強制方法について、良いアイディアが見当たらないので。。。
        /// </summary>
        public const string Name = "_";

        public long Serial { get; set; }
        public DateTime Date { get; set; }
        public string HostName { get; set; }
        public string UserName { get; set; }
        public LogLevel Level { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }


    }
}
