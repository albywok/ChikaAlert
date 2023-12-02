using System;
using System.Net;
using System.Threading;
using System.Windows;

namespace ChikaAlert
{
    public class HttpServer : IDisposable
    {
        private HttpListener httpListener;
        private CancellationTokenSource cancellationTokenSource;

        private bool isOverlayWindowOpen = false;
        private OverlayWindow overlayWindowInstance;
        public void Start()
        {
            httpListener = new HttpListener();
            httpListener.Prefixes.Add("http://*:8291/");

            cancellationTokenSource = new CancellationTokenSource();
            ThreadPool.QueueUserWorkItem(HandleHttpRequests, cancellationTokenSource.Token);
        }

        private void HandleHttpRequests(object state)
        {
            var cancellationToken = (CancellationToken)state;

            try
            {
                httpListener.Start();

                while (!cancellationToken.IsCancellationRequested)
                {
                    HttpListenerContext context = httpListener.GetContext();
                    ThreadPool.QueueUserWorkItem(HandleRequest, context);
                }
            }
            catch (HttpListenerException ex)
            {
                Console.WriteLine($"HTTP Listener Exception: {ex.Message}");
            }
            catch (OperationCanceledException)
            {
                //do sum
            }
            finally
            {
                httpListener.Close();
            }
        }

        private void HandleRequest(object state)
        {
            var context = (HttpListenerContext)state;

            if (context.Request.HttpMethod.Equals("GET", StringComparison.OrdinalIgnoreCase))
            {
                if (context.Request.Url.LocalPath.Equals("/alert", StringComparison.OrdinalIgnoreCase))
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        overlayWindowInstance = new OverlayWindow();
                        overlayWindowInstance.Show();
                        overlayWindowInstance.WindowState = WindowState.Maximized;
                        isOverlayWindowOpen = true;
                    });
                }

                context.Response.StatusCode = (int)HttpStatusCode.OK;
                context.Response.Close();
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                context.Response.Close();
            }
        }


        public void Stop()
        {
            cancellationTokenSource?.Cancel();
        }

        public void Dispose()
        {
            Stop();
            cancellationTokenSource?.Dispose();
            httpListener?.Close();
        }
    }
}
