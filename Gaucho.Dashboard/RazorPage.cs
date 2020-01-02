using System.Net;
using System.Text;
using Gaucho.Server.Monitoring;

namespace Gaucho.Dashboard
{
    public abstract class RazorPage
    {
        private readonly StringBuilder _content = new StringBuilder();
        private string _body;

        protected RazorPage()
        {
            Html = new HtmlHelper(this);
        }

        public HtmlHelper Html { get; set; }

        public RazorPage Layout { get; protected set; }

        public UrlHelper Url { get; set; }

        public DashboardContext Context { get; set; }

        public DashboardResponse Response { get; set; }

        public DashboardRequest Request { get; set; }

        public IServerMonitor ServerMonitor { get; private set; }

        public DashboardOptions DashboardOptions { get; private set; }

        /// <exclude />
        public void Assign(RazorPage parentPage)
        {
            Context = parentPage.Context;
            Request = parentPage.Request;
            Response = parentPage.Response;
            //AppPath = parentPage.AppPath;
            ServerMonitor = parentPage.ServerMonitor;
            DashboardOptions = parentPage.DashboardOptions;
            Url = parentPage.Url;

            //GenerationTime = parentPage.GenerationTime;
            //_statisticsLazy = parentPage._statisticsLazy;
        }

        internal void Assign(DashboardContext context)
        {
            Context = context;
            Request = context.Request;
            Response = context.Response;

            //AppPath = context.Options.AppPath;
            ServerMonitor = context.ServerMonitor;
            DashboardOptions = context.Options;
            Url = new UrlHelper(context);

            //_statisticsLazy = new Lazy<StatisticsDto>(() =>
            //{
            //    var monitoring = Storage.GetMonitoringApi();
            //    return monitoring.GetStatistics();
            //});
        }

        /// <exclude />
        public abstract void Execute();

        protected virtual object RenderBody()
        {
            return new NonEscapedString(_body);
        }

        /// <exclude />
        protected void WriteLiteral(string textToAppend)
        {
            if (string.IsNullOrEmpty(textToAppend))
                return;
            _content.Append(textToAppend);
        }

        /// <exclude />
        protected virtual void Write(object value)
        {
            if (value == null)
                return;
            var html = value as NonEscapedString;
            WriteLiteral(html?.ToString() ?? Encode(value.ToString()));
        }

        private static string Encode(string text)
        {
            return string.IsNullOrEmpty(text) ? string.Empty : WebUtility.HtmlEncode(text);
        }

        private string TransformText(string body)
        {
            _body = body;

            Execute();

            if (Layout != null)
            {
                Layout.Assign(this);
                return Layout.TransformText(_content.ToString());
            }

            return _content.ToString();
        }

        public override string ToString()
        {
            return TransformText(null);
        }
    }
}
