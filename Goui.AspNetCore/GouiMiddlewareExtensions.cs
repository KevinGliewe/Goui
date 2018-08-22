﻿using System;
using Goui.AspNetCore;

namespace Microsoft.AspNetCore.Builder
{
    public static class GouiMiddlewareExtensions
    {
        public static void UseGoui (this IApplicationBuilder app, string jsPath = "/Goui.js", string webSocketPath = "/Goui.ws", TimeSpan? sessionTimeout = null)
        {
            if (string.IsNullOrWhiteSpace (webSocketPath))
                throw new ArgumentException ("A path to be used for Goui web sockets must be specified", nameof (webSocketPath));
            
            if (string.IsNullOrWhiteSpace (jsPath))
                throw new ArgumentException ("A path to be used for Goui JavaScript must be specified", nameof (jsPath));

            WebSocketHandler.WebSocketPath = webSocketPath;

            if (sessionTimeout.HasValue) {
                WebSocketHandler.SessionTimeout = sessionTimeout.Value;
            }

            var webSocketOptions = new WebSocketOptions () {
                KeepAliveInterval = WebSocketHandler.SessionTimeout,
                ReceiveBufferSize = 4 * 1024
            };
            app.UseWebSockets (webSocketOptions);

            Goui.UI.ServerEnabled = false;

            app.Use (async (context, next) =>
            {
                var response = context.Response;

                if (context.Request.Path == jsPath) {
                    var clientJsBytes = Goui.UI.ClientJsBytes;
                    var clientJsEtag = Goui.UI.ClientJsEtag;
                    if (context.Request.Headers.TryGetValue ("If-None-Match", out var inms) && inms.Count > 0 && inms[0] == clientJsEtag) {
                        response.StatusCode = 304;
                    }
                    else {
                        response.StatusCode = 200;
                        response.ContentLength = clientJsBytes.Length;
                        response.ContentType = "application/javascript; charset=utf-8";
                        response.Headers.Add ("Cache-Control", "public, max-age=60");
                        response.Headers.Add ("Etag", clientJsEtag);
                        using (var s = response.Body) {
                            await s.WriteAsync (clientJsBytes, 0, clientJsBytes.Length).ConfigureAwait (false);
                        }
                    }
                }
                else if (context.Request.Path == WebSocketHandler.WebSocketPath) {
                    if (context.WebSockets.IsWebSocketRequest) {
                        await WebSocketHandler.HandleWebSocketRequestAsync (context).ConfigureAwait (false);
                    }
                    else {
                        context.Response.StatusCode = 400;
                    }
                }
                else if (Goui.UI.TryGetFileContentAtPath (context.Request.Path, out var file)) {
                    if (context.Request.Headers.TryGetValue ("If-None-Match", out var inms) && inms.Count > 0 && inms[0] == file.Etag) {
                        response.StatusCode = 304;
                    }
                    else {
                        response.StatusCode = 200;
                        response.ContentLength = file.Content.Length;
                        response.ContentType = file.ContentType;
                        response.Headers.Add ("Cache-Control", "public, max-age=60");
                        response.Headers.Add ("Etag", file.Etag);
                        using (var s = response.Body) {
                            await s.WriteAsync (file.Content, 0, file.Content.Length).ConfigureAwait (false);
                        }
                    }
                }
                else {
                    await next ().ConfigureAwait (false);
                }
            });
        }
    }
}
