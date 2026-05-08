using System.Net;
using System.Text;

namespace Fhir.Auth.TokenServer;

/// <summary>本機 <c>http://127.0.0.1:{port}/...</c> 接收 OAuth redirect（桌面應用常用）。</summary>
public static class SmartLoopbackListener
{
    /// <summary>等待瀏覽器導向至與 <paramref name="redirectUri"/> 同主機／埠之請求，回傳完整 URL（含 query）。</summary>
    public static async Task<Uri?> WaitForRedirectAsync(string redirectUri, TimeSpan timeout, CancellationToken cancellationToken)
    {
        if (!Uri.TryCreate(redirectUri, UriKind.Absolute, out var expected))
            return null;

        if (!string.Equals(expected.Scheme, "http", StringComparison.OrdinalIgnoreCase))
            return null;

        if (expected.Host is not ("127.0.0.1" or "localhost"))
            return null;

        var port = expected.Port > 0 ? expected.Port : 80;
        var prefix = $"http://127.0.0.1:{port}/";

        using var listener = new HttpListener();
        listener.Prefixes.Add(prefix);

        try
        {
            listener.Start();
        }
        catch (HttpListenerException)
        {
            return null;
        }

        try
        {
            using var linked = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            linked.CancelAfter(timeout);

            var contextTask = listener.GetContextAsync();
            var ctx = await contextTask.WaitAsync(linked.Token).ConfigureAwait(false);

            await WriteSuccessPageAsync(ctx.Response).ConfigureAwait(false);

            return ctx.Request.Url;
        }
        catch (OperationCanceledException)
        {
            return null;
        }
        finally
        {
            try
            {
                listener.Stop();
            }
            catch
            {
                // ignore
            }
        }
    }

    private static async Task WriteSuccessPageAsync(HttpListenerResponse response)
    {
        const string html = "<html><head><meta charset=\"utf-8\"/><title>SMART</title></head><body><p>授權完成，可關閉此視窗。</p></body></html>";
        var buf = Encoding.UTF8.GetBytes(html);
        response.ContentType = "text/html; charset=utf-8";
        response.ContentLength64 = buf.Length;
        await response.OutputStream.WriteAsync(buf).ConfigureAwait(false);
        response.OutputStream.Close();
    }
}
