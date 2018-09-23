using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using Goui.Html;

namespace Goui.Plugin.Maps {
    public class MapsPlugin : IGouiPlugin {
 

        private static Dictionary<string, HostedFile> HostedFiles = new Dictionary<string, HostedFile>();
        static MapsPlugin() {
            var asm = typeof(MapsPlugin).Assembly;
            HostedFiles.Add("/GoogleMaps.js", HostedFile.LoadFromResource(asm, "Goui.Plugin.Maps.Js.GoogleMaps.js"));
        }

        public HostedFile GetHostedFile(string path) {
            return HostedFiles.ContainsKey(path) ? HostedFiles[path] : null;
        }

        public bool OnHttpRequest(HttpListenerContext listenerContext) {
            return true;
        }

        public void OnNodeCreated(Node node) {
        }

        public void OnPublish(string path, Element element) {
        }

        public bool OnRenderTemplate(TextWriter writer, string webSocketPath, string title, string initialHtml) {
            return true;
        }

        public void OnUIStart() {
        }

        public void OnUIStop() {
        }
    }
}
