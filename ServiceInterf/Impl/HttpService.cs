﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using TakeStock.Dtos;
using TakeStock.Static;

namespace TakeStock.ServiceInterf.Impl
{
    public class HttpService: IHttpService
    {
        /// <summary>
        /// 读写器类
        /// </summary>
        public ReaderComponent readerComponent { set; get; }

        private const string NotFoundResponse = "<!doctype html><html><body>Resource not found</body></html>";
        private readonly HttpListener httpListener;
        private readonly CancellationTokenSource cts = new CancellationTokenSource();
        private readonly string prefixPath;

        private Task processingTask;

        //httpListener.Prefixes.Add("http://localhost:8820/");
        public HttpService(string listenerUriPrefix)
        {
            this.prefixPath = ParsePrefixPath(listenerUriPrefix);
            this.httpListener = new HttpListener();
            this.httpListener.Prefixes.Add(listenerUriPrefix);
        }

        private static string ParsePrefixPath(string listenerUriPrefix)
        {
            var match = Regex.Match(listenerUriPrefix, @"http://(?:[^/]*)(?:\:\d+)?/(.*)");
            if (match.Success)
            {
                return match.Groups[1].Value.ToLowerInvariant();
            }
            else
            {
                return string.Empty;
            }
        }

        public void Start()
        {
            this.httpListener.Start();
            this.processingTask = Task.Factory.StartNew(async () => await ProcessRequests(), TaskCreationOptions.LongRunning);
        }

        private async Task ProcessRequests()
        {
            while (!this.cts.IsCancellationRequested)
            {
                try
                {
                    var context = await this.httpListener.GetContextAsync();
                    try
                    {
                        await ProcessRequest(context).ConfigureAwait(false);
                        context.Response.Close();
                    }
                    catch (Exception ex)
                    {
                        context.Response.StatusCode = 500;
                        context.Response.StatusDescription = "Internal Server Error";
                        context.Response.Close();
                        Console.WriteLine("Error processing HTTP request\n{0}", ex);
                    }
                }
                catch (ObjectDisposedException ex)
                {
                    if ((ex.ObjectName == this.httpListener.GetType().FullName) && (this.httpListener.IsListening == false))
                    {
                        return; // listener is closed/disposed
                    }
                    Console.WriteLine("Error processing HTTP request\n{0}", ex);
                }
                catch (Exception ex)
                {
                    HttpListenerException httpException = ex as HttpListenerException;
                    if (httpException == null || httpException.ErrorCode != 995)// IO operation aborted
                    {
                        Console.WriteLine("Error processing HTTP request\n{0}", ex);
                    }
                }
            }
        }

        /// <summary>
        /// 启动读写器
        /// </summary>
        private void readerComponentStart()
        {
            this.readerComponent.Usbconnect();
            this.readerComponent.GeteAntennaNo(4);
            this.readerComponent.StartReadEpc();
        }
        /// <summary>
        /// 关闭读写器
        /// </summary>
        private void readerComponentClose()
        {
            StaticEntity.Pool.Clear();
            this.readerComponent.StopReadEpc();
            this.readerComponent.CloseNowConnect();
        }
        private Task ProcessRequest(HttpListenerContext context)
        {
            if (context.Request.HttpMethod.ToUpperInvariant() != "GET")
            {
                return WriteNotFound(context);
            }

            var urlPath = context.Request.RawUrl.Substring(this.prefixPath.Length)
                .ToLowerInvariant();

            string actionName = urlPath.Split('?')[0];

            //switch (urlPath)
            switch (actionName)
            {
                case "/":return WriteString(context, JsonConvert.SerializeObject(new CommondOutPut { success = false, msg= StaticMsg.NoCom }), "application/json; charset=UTF-8");
                case "/allstart" :
                    string allstartMsg = StaticMsg.Success;
                    bool allstartBool = true;
                    try
                    {
                        if (!StaticEntity.MachineWork) {
                            this.readerComponentStart();
                        }
                        StaticEntity.MachineWork = true;
                        StaticEntity.MqttPushWork = true;
                    }
                    catch {
                        allstartMsg = StaticMsg.ComponentFailed;
                        allstartBool = false;
                    }
                    return WriteString(context, JsonConvert.SerializeObject(new CommondOutPut { success = allstartBool, msg = allstartMsg }), "application/json; charset=UTF-8");

                case "/allstop":
                    string allstopMsg = StaticMsg.Success;
                    bool allstopBool = true;
                    try
                    {
                        if (StaticEntity.MachineWork)
                        {
                            this.readerComponentClose();
                        }
                        StaticEntity.MachineWork = false;
                        StaticEntity.MqttPushWork = false;
                    }
                    catch
                    {
                        allstopMsg = StaticMsg.ComponentCloseFailed;
                        allstopBool = false;
                    }
                    return WriteString(context, JsonConvert.SerializeObject(new CommondOutPut { success = allstopBool, msg = allstopMsg }), "application/json; charset=UTF-8");

                case "/pushstart":
                    StaticEntity.MqttPushWork = true;
                    return WriteString(context, JsonConvert.SerializeObject(new CommondOutPut { success = true, msg = StaticMsg.Success }), "application/json; charset=UTF-8");
                case "/pushstop":
                    StaticEntity.MqttPushWork = false;
                    return WriteString(context, JsonConvert.SerializeObject(new CommondOutPut { success = true, msg = StaticMsg.Success }), "application/json; charset=UTF-8");
                case "/start":
                    string startMsg = StaticMsg.Success;
                    bool startBool = true;
                    try
                    {
                        if (!StaticEntity.MachineWork)
                        {
                            this.readerComponentStart();
                        }
                        StaticEntity.MachineWork = true;
                    }
                    catch
                    {
                        startMsg = StaticMsg.ComponentFailed;
                        startBool = false;
                    }
                    return WriteString(context, JsonConvert.SerializeObject(new CommondOutPut { success = startBool, msg = startMsg }), "application/json; charset=UTF-8");
                case "/stop":
                    string stopMsg = StaticMsg.Success;
                    bool stopBool = true;
                    try
                    {
                        if (StaticEntity.MachineWork)
                        {
                            this.readerComponentClose();
                        }
                        StaticEntity.MachineWork = false;
                    }
                    catch
                    {
                        stopMsg = StaticMsg.ComponentCloseFailed;
                        stopBool = false;
                    }
                    return WriteString(context, JsonConvert.SerializeObject(new CommondOutPut { success = stopBool, msg = stopMsg }), "application/json; charset=UTF-8");

                //return WriteString(context, ResStr, "application/json; charset=UTF-8");

                //if (!context.Request.Url.ToString().EndsWith("/"))
                //{
                //    string Commond = context.Request.QueryString["Commond"];
                //    Console.WriteLine(Commond);
                //    context.Response.Redirect(context.Request.Url + "/");
                //    context.Response.Close();
                //    return Task.FromResult(0);
                //}
                //else
                //{
                //    return WriteString(context, "Hello World!", "application/json; charset=UTF-8");
                //    //return WriteString(context, "Hello World!", "text/plain");
                //}
                case "/favicon.ico":return WriteFavIcon(context);
                case "/status": return WriteString(context, JsonConvert.SerializeObject(new MachineStatusOutPut { MachineWork = StaticEntity.MachineWork, MqttPushWork= StaticEntity.MqttPushWork }), "application/json; charset=UTF-8"); ;
                case "/ping": return WritePong(context);
                default:
                    return WriteString(context, JsonConvert.SerializeObject(new CommondOutPut { success = false, msg= StaticMsg.NoCom }), "application/json; charset=UTF-8");
                //case "/pay": return OnPayResult(context);
            }
            //return WriteNotFound(context);
        }

        private static Task WritePong(HttpListenerContext context)
        {
            return WriteString(context, "ok", "text/plain");
        }

        //private static async Task OnPayResult(HttpListenerContext context)
        //{
        //    var ret = "failed";
        //    try
        //    {
        //        string postData;
        //        using (var br = new BinaryReader(context.Request.InputStream))
        //        {
        //            postData =
        //                Encoding.UTF8.GetString(
        //                    br.ReadBytes(int.Parse(context.Request.ContentLength64.ToString())));
        //        }
        //        if (!string.IsNullOrEmpty(postData))
        //        {
        //            //Console.WriteLine("postData=[{0}]", postData);

        //            //var request = JsonConvert.DeserializeObject<PayResult>(postData);
        //            //if (null != request)
        //            //{
        //            //    var parmas = request.data.orderNo.Split(',');

        //            //    var id = long.Parse(parmas[0]);

        //            //    var playerProxy = GrainClient.GrainFactory.GetGrain<IPlayerProxy>(id);
        //            //    var success =
        //            //        await playerProxy.OnPlayerPayResult(request.data.orderNo, request.code, request.msg);
        //            //    if (success)
        //            //    {
        //            //        ret = "success";
        //            //    }
        //            //}
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e);
        //    }

        //    await WriteString(context, ret, "text/plain");
        //}

        private static async Task WriteFavIcon(HttpListenerContext context)
        {
            context.Response.ContentType = "image/png";
            context.Response.StatusCode = 200;
            context.Response.StatusDescription = "OK";
            using (var stream = File.Open("icon.png", FileMode.Open))
            {
                var output = context.Response.OutputStream;
                await stream.CopyToAsync(output);
            }
        }

        private static Task WriteNotFound(HttpListenerContext context)
        {
            return WriteString(context, NotFoundResponse, "text/plain", 404, "NOT FOUND");
        }

        private static async Task WriteString(HttpListenerContext context, string data, string contentType,
            int httpStatus = 200, string httpStatusDescription = "OK")
        {
            AddCORSHeaders(context.Response);
            AddNoCacheHeaders(context.Response);

            context.Response.ContentType = contentType;
            context.Response.StatusCode = httpStatus;
            context.Response.StatusDescription = httpStatusDescription;

            var acceptsGzip = AcceptsGzip(context.Request);
            if (!acceptsGzip)
            {
                using (var writer = new StreamWriter(context.Response.OutputStream, Encoding.UTF8, 4096, true))
                {
                    await writer.WriteAsync(data).ConfigureAwait(false);
                }
            }
            else
            {
                context.Response.AddHeader("Content-Encoding", "gzip");
                using (GZipStream gzip = new GZipStream(context.Response.OutputStream, CompressionMode.Compress, true))
                using (var writer = new StreamWriter(gzip, Encoding.UTF8, 4096, true))
                {
                    await writer.WriteAsync(data).ConfigureAwait(false);
                }
            }
        }


        private static bool AcceptsGzip(HttpListenerRequest request)
        {
            string encoding = request.Headers["Accept-Encoding"];
            if (string.IsNullOrEmpty(encoding))
            {
                return false;
            }

            return encoding.Contains("gzip");
        }

        private static void AddNoCacheHeaders(HttpListenerResponse response)
        {
            response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
            response.Headers.Add("Pragma", "no-cache");
            response.Headers.Add("Expires", "0");
        }

        private static void AddCORSHeaders(HttpListenerResponse response)
        {
            response.Headers.Add("Access-Control-Allow-Origin", "*");
            response.Headers.Add("Access-Control-Allow-Headers", "Origin, X-Requested-With, Content-Type, Accept");
        }

        private void Stop()
        {
            cts.Cancel();
            if (processingTask != null && !processingTask.IsCompleted)
            {
                processingTask.Wait();
            }
            if (this.httpListener.IsListening)
            {
                this.httpListener.Stop();
                this.httpListener.Prefixes.Clear();
            }
        }

        public void Dispose()
        {
            if (StaticEntity.MachineWork)
            {
                try
                {
                    this.readerComponentClose();
                }
                catch { 

                }
            }
            StaticEntity.MachineWork = false;
            StaticEntity.MqttPushWork = false;

            this.Stop();
            this.httpListener.Close();
            using (this.cts) { }
            using (this.httpListener) { }
        }
    }
}
