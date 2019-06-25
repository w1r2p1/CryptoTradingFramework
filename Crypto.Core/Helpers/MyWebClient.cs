﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace CryptoMarketClient.Common {
    public class MyWebClient : HttpClient {
        public MyWebClient() {
        }
        public string DownloadString(string address) {
            Task<string> t = this.GetStringAsync(address);
            t.Wait(10000);
            return t.Result;
        }
        public byte[] DownloadData(string adress) {
            Task<byte[]> t = this.GetByteArrayAsync(adress);
            try {
                t.Wait(10000);
                return t.Result;
            }
            catch(Exception e) {
                Telemetry.Default.TrackException(e);
                return t.Result;
            }
            //byte[] res = DownloadDataAsync(adress).Result;
            //return res;
        }
        public async Task<byte[]> DownloadDataAsync(string adress) {
            HttpRequestMessage msg = new HttpRequestMessage(HttpMethod.Get, adress);
            using(var response = await SendAsync(msg)) {
                try {
                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadAsByteArrayAsync();
                }
                catch(HttpRequestException ex) {
                    Telemetry.Default.TrackException(ex);
                    return await response.Content.ReadAsByteArrayAsync();
                    //throw CreateDomainExceptionFromDetails(errorDetails, ex);
                }
            }

            //Task<byte[]> t = this.GetByteArrayAsync(adress);
            //t.Wait(10000);
            //return t.Result;
        }
        public Task<string> DownloadStringTaskAsync(string address) {
            return this.GetStringAsync(address);
        }
        public byte[] UploadString(string address, string content) {
            var bytes = new ByteArrayContent(Encoding.UTF8.GetBytes(content));
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, address);
            request.Content = new StringContent(content, Encoding.UTF8, "application/json");

            Task<HttpResponseMessage> t = this.SendAsync(request);
            t.Wait(10000);
            Task<byte[]> tt = t.Result.Content.ReadAsByteArrayAsync();
            tt.Wait(10000);
            return tt.Result;
        }
        public byte[] UploadData(string address, string content) {
            var bytes = new ByteArrayContent(Encoding.UTF8.GetBytes(content));
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, address);
            request.Method = HttpMethod.Post;
            request.Content = bytes;
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //request.Headers.Add("X-Requested-With", "XMLHttpRequest");
            Task<HttpResponseMessage> t = this.SendAsync(request);
            t.Wait(10000);
            Task<byte[]> tt = t.Result.Content.ReadAsByteArrayAsync();
            tt.Wait(10000);
            return tt.Result;
        }
        public byte[] UploadValues(string address, HttpRequestParamsCollection coll) {
            var content = new FormUrlEncodedContent(coll);
            Task<HttpResponseMessage> t = this.PostAsync(address, content);
            t.Wait(10000);
            Task<byte[]> tt = t.Result.Content.ReadAsByteArrayAsync();
            tt.Wait(10000);
            return tt.Result;
        }
        public async Task<byte[]> UploadValuesTaskAsync(string address, HttpRequestParamsCollection coll) {
            var content = new FormUrlEncodedContent(coll);
            HttpResponseMessage message = await this.PostAsync(address, content);
            return await message.Content.ReadAsByteArrayAsync();
        }
        public HttpRequestHeaders Headers { get { return DefaultRequestHeaders; } }
    }

    public class HttpRequestParamsCollection : List<KeyValuePair<string, string>> {
        public void Add(string key, string value) {
            this.Add(new KeyValuePair<string, string>(key, value));
        }
    }

    //public class MyWebClient : WebClient {
    //    static MyWebClient() {
    //        ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate,
    //         X509Chain chain, SslPolicyErrors sslPolicyErrors)
    //        { return true; };
    //    }
    //    public MyWebClient() : base() {
            
    //    }
    //    protected override WebRequest GetWebRequest(Uri address) {
    //        WebRequest res = base.GetWebRequest(address);
    //        HttpWebRequest hres = res as HttpWebRequest;
    //        if(hres != null) {
    //            hres.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
    //            hres.ContentType = "application/x-www-form-urlencoded";
    //            hres.Accept = "application/xml, text/json, text/x-json, text/javascript, text/xml, application/json";
    //            hres.Headers.Add(HttpRequestHeader.AcceptLanguage, "en-us");
    //            hres.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
    //        }
    //        res.Timeout = 7000;
    //        return res;
    //    }
    //}
}
