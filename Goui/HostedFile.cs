using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;

namespace Goui {
    public class HostedFile {
        public byte[] Data { get; private set; }
        public string Etag { get; private set; }
        public string ContentType { get; private set; }
        public HostedFile(byte[] data, string contentType = "application/javascript") {
            Data = data;
            Etag = "\"" + Utilities.Hash(data) + "\"";
            ContentType = contentType;
        }

        public void Respond(HttpListenerContext listenerContext) {
            var response = listenerContext.Response;
            var inm = listenerContext.Request.Headers.Get("If-None-Match");
            if (string.IsNullOrEmpty(inm) || inm != Etag) {
                response.StatusCode = 200;
                response.ContentLength64 = Data.LongLength;
                response.ContentType = ContentType;
                response.ContentEncoding = Encoding.UTF8;
                response.AddHeader("Cache-Control", "public, max-age=60");
                response.AddHeader("Etag", Etag);
                using (var s = response.OutputStream) {
                    s.Write(Data, 0, Data.Length);
                }
                response.Close();
            } else {
                response.StatusCode = 304;
                response.Close();
            }
        }

        public static HostedFile LoadFromResource(Assembly asm, string resource, string contentType = "application/javascript") {
            using (var s = asm.GetManifestResourceStream(resource)) {
                if (s == null)
                    throw new Exception("Missing " + resource);
                using (var r = new StreamReader(s)) {
                    var data = Encoding.UTF8.GetBytes(r.ReadToEnd());
                    return new HostedFile(data, contentType);
                }
            }
        }
    }
}
