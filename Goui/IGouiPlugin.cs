using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Goui {
    public interface IGouiPlugin {
        HostedFile GetHostedFile(string path);
        void OnNodeCreated(Node node);
        void OnUIStart();
        void OnUIStop();
        void OnPublish(string path, Html.Element element);
        bool OnHttpRequest(HttpListenerContext listenerContext);
        bool OnRenderTemplate(TextWriter writer, string webSocketPath, string title, string initialHtml);
    }
}
