namespace Fhir.QueryBuilder.Platform;

/// <summary>以預設瀏覽器或新分頁開啟 URL（桌面／Web 各殼實作）。</summary>
public interface IExternalUriLauncher
{
    void OpenUri(string url);
}
