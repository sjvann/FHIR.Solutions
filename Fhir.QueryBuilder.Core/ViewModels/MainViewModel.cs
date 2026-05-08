using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Fhir.QueryBuilder.AdvancedSearch;
using Fhir.QueryBuilder.Configuration;
using Fhir.QueryBuilder.Models;
using Fhir.QueryBuilder.Platform;
using Fhir.QueryBuilder.QueryBuilders.FluentApi;
using Fhir.QueryBuilder.SearchUi;
using Fhir.Auth.TokenServer;
using Fhir.QueryBuilder.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Fhir.QueryBuilder.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly IFhirQueryService _fhirQueryService;
    private readonly IValidationService _validationService;
    private readonly Func<IFhirQueryBuilder> _queryBuilderFactory;
    private readonly IErrorHandlingService _errorHandlingService;
    private readonly IProgressService _progressService;
    private readonly ILogger<MainViewModel> _logger;
    private readonly QueryBuilderAppSettings _options;
    private readonly ITokenServer _tokenServer;
    private readonly IClipboardTextService? _clipboardText;
    private readonly IFilePickerSaveTextService? _filePickerSave;
    private CancellationTokenSource? _cancellationTokenSource;

    /// <summary>WinForms UI（<see cref="SynchronizationContext"/>）。</summary>
    private SynchronizationContext? _uiSynchronizationContext;

    /// <summary>Avalonia 等環境：將動作送 UI 訊息迴圈（優先於 <see cref="_uiSynchronizationContext"/>）。</summary>
    private Action<Action>? _uiThreadPost;

    [ObservableProperty]
    private string _serverUrl = string.Empty;

    [ObservableProperty]
    private bool _isConnected;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private string _selectedResource = string.Empty;

    [ObservableProperty]
    private string _queryUrl = string.Empty;

    [ObservableProperty]
    private string _queryResult = string.Empty;

    [ObservableProperty]
    private bool _canExecuteQuery;

    [ObservableProperty]
    private bool _canSaveResult;

    [ObservableProperty]
    private bool _supportOAuth;

    [ObservableProperty]
    private string _tokenUrl = string.Empty;

    public ObservableCollection<string> SupportedResources { get; } = new();
    public ObservableCollection<SearchParamModel> SearchParameters { get; } = new();
    public ObservableCollection<string> SearchIncludes { get; } = new();
    public ObservableCollection<string> SearchRevIncludes { get; } = new();

    /// <summary>Avalonia：使用者編輯的查詢條件行（與 WinForms 的 Lib_ParameterList 對應），於 <see cref="BuildComposedQueryCommand"/> 合併。</summary>
    public ObservableCollection<string> DraftSearchParameters { get; } = new();

    /// <summary>_include 可選路徑（與 <see cref="SearchIncludes"/> 同步列舉）。</summary>
    public ObservableCollection<SelectableIncludeRow> IncludeChoices { get; } = new();

    /// <summary>_revinclude 可選路徑。</summary>
    public ObservableCollection<SelectableIncludeRow> RevIncludeChoices { get; } = new();

    [ObservableProperty]
    private bool _includeTakeAll;

    [ObservableProperty]
    private bool _revIncludeTakeAll;

    [ObservableProperty]
    private SearchParamModel? _selectedSearchParameter;

    [ObservableProperty]
    private string _pendingDraftLine = string.Empty;

    [ObservableProperty]
    private string? _selectedDraftParameter;

    /// <summary>依型別組句輸入（Avalonia／VM）。</summary>
    public TypedSearchDraft TypedSearch { get; } = new();

    /// <summary>進階搜尋（chain／composite／filter／結果控制）。</summary>
    public AdvancedSearchDraft Advanced { get; } = new();

    /// <summary>結果修飾（<c>_contained</c>、<c>_elements</c>、<c>_sort</c> 等，對齊 WinForms <c>ModifyResult</c>）。</summary>
    public ModifyingSearchDraft Modifying { get; } = new();

    /// <summary>手動貼上的 Bearer access token（會傳給 <see cref="IFhirQueryService.ExecuteQueryAsync"/>）。</summary>
    [ObservableProperty]
    private string _bearerAccessToken = "";

    /// <summary>SMART OAuth authorize endpoint（若伺服器／服務實作回傳）。</summary>
    [ObservableProperty]
    private string _authorizeUrl = "";

    /// <summary>SMART 登入後 token 回應中的 patient（若有）。</summary>
    [ObservableProperty]
    private string _smartSessionPatient = "";

    /// <summary>SMART client_id（可覆寫 appsettings）。</summary>
    [ObservableProperty]
    private string _smartClientId = "";

    /// <summary>SMART scope 字串。</summary>
    [ObservableProperty]
    private string _smartScopes = "";

    /// <summary>機密客戶端 JWT assertion 用私鑰 PEM（RSA/EC）。</summary>
    [ObservableProperty]
    private string _smartPrivateKeyPem = "";

    /// <summary>查詢結果 JSON 樹狀根節點（供 Avalonia TreeView）。</summary>
    public ObservableCollection<JsonTreeItem> QueryResultTreeRoots { get; } = new();

    /// <summary>true：顯示樹狀；false：顯示文字（對齊 WinForms Tree View 切換）。</summary>
    [ObservableProperty]
    private bool _showQueryResultAsTree;

    /// <summary>結果區切換按鈕標題（Tree view ↔ Message view）。</summary>
    public string ResultViewToggleLabel => ShowQueryResultAsTree ? "Message view" : "Tree view";

    [ObservableProperty]
    private bool _editorStringVisible;

    [ObservableProperty]
    private bool _editorTokenVisible;

    [ObservableProperty]
    private bool _editorUriVisible;

    [ObservableProperty]
    private bool _editorReferenceVisible;

    [ObservableProperty]
    private bool _editorNumberVisible;

    [ObservableProperty]
    private bool _editorDateVisible;

    [ObservableProperty]
    private bool _editorQuantityVisible;

    [ObservableProperty]
    private bool _editorUnsupportedVisible;

    public ObservableCollection<string> QueryParameters { get; } = new();
    public ObservableCollection<string> ModifyingParameters { get; } = new();
    public ObservableCollection<string> ValidationErrors { get; } = new();

    /// <summary>是否有驗證／錯誤訊息可顯示（供 Avalonia 顯示錯誤區塊）。</summary>
    public bool HasValidationErrors => ValidationErrors.Count > 0;

    public MainViewModel(
        IFhirQueryService fhirQueryService,
        IValidationService validationService,
        Func<IFhirQueryBuilder> queryBuilderFactory,
        IErrorHandlingService errorHandlingService,
        IProgressService progressService,
        ILogger<MainViewModel> logger,
        IOptions<QueryBuilderAppSettings> options,
        ITokenServer tokenServer,
        IClipboardTextService? clipboardText = null,
        IFilePickerSaveTextService? filePickerSave = null)
    {
        _fhirQueryService = fhirQueryService;
        _validationService = validationService;
        _queryBuilderFactory = queryBuilderFactory;
        _errorHandlingService = errorHandlingService;
        _progressService = progressService;
        _logger = logger;
        _options = options.Value;
        _tokenServer = tokenServer;
        _clipboardText = clipboardText;
        _filePickerSave = filePickerSave;

        ServerUrl = _options.DefaultServerUrl;
        SmartClientId = _options.Smart.ClientId;
        SmartScopes = _options.Smart.DefaultScopes;

        _errorHandlingService.ErrorOccurred += OnErrorOccurred;
        _progressService.ProgressChanged += OnProgressChanged;
        _progressService.OperationCompleted += OnOperationCompleted;
        _progressService.OperationFailed += OnOperationFailed;

        Modifying.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(ModifyingSearchDraft.SelectedElementHint))
            {
                ModifyingAddElementCommand.NotifyCanExecuteChanged();
                ModifyingAddSortCommand.NotifyCanExecuteChanged();
            }
        };

        ValidationErrors.CollectionChanged += (_, _) =>
            OnPropertyChanged(nameof(HasValidationErrors));
    }

    /// <summary>由主視窗於首次顯示時呼叫，避免 async 接續在非 UI 執行緒更新 <see cref="SupportedResources"/> 等集合。</summary>
    public void SetUiSynchronizationContext(SynchronizationContext? context)
        => _uiSynchronizationContext = context;

    /// <summary>
    /// Avalonia 請於視窗 Loaded 設定（例如 <c>Dispatcher.UIThread.Post</c>）；
    /// WinForms 可不呼叫，改用 <see cref="SetUiSynchronizationContext"/>。
    /// </summary>
    public void SetUiThreadPost(Action<Action> postAction)
        => _uiThreadPost = postAction ?? throw new ArgumentNullException(nameof(postAction));

    private Task RunOnUiAsync(Action action)
    {
        var tcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        void Run()
        {
            try
            {
                action();
                tcs.TrySetResult();
            }
            catch (Exception ex)
            {
                tcs.TrySetException(ex);
            }
        }

        if (_uiThreadPost != null)
            _uiThreadPost(Run);
        else if (_uiSynchronizationContext != null)
            _uiSynchronizationContext.Post(_ => Run(), null);
        else
            Run();

        return tcs.Task;
    }

    /// <summary>
    /// 將動作送 UI 訊息迴圈（不等待完成）。供進度／錯誤等非 async 事件使用，避免跨執行緒更新繫結（Avalonia：invalid thread）。
    /// </summary>
    private void RunOnUi(Action action)
    {
        if (_uiThreadPost != null)
            _uiThreadPost(action);
        else if (_uiSynchronizationContext != null)
            _uiSynchronizationContext.Post(_ => action(), null);
        else
            action();
    }

    partial void OnIsConnectedChanged(bool value) => NotifySmartCommands();

    partial void OnSupportOAuthChanged(bool value) => NotifySmartCommands();

    partial void OnIsLoadingChanged(bool value) => NotifySmartCommands();

    partial void OnSmartClientIdChanged(string value) => NotifySmartCommands();

    partial void OnSmartPrivateKeyPemChanged(string value) => NotifySmartCommands();

    private void NotifySmartCommands()
    {
        RunSmartBrowserLoginCommand.NotifyCanExecuteChanged();
        RefreshSmartTokenCommand.NotifyCanExecuteChanged();
        RunSmartClientCredentialsJwtCommand.NotifyCanExecuteChanged();
    }

    private bool CanRunSmartBrowserLogin() =>
        IsConnected && SupportOAuth && !IsLoading && !string.IsNullOrWhiteSpace(SmartClientId);

    private bool CanRefreshSmartToken() =>
        IsConnected && SupportOAuth && !IsLoading && !string.IsNullOrWhiteSpace(_tokenServer.SessionRefreshToken);

    private bool CanRunSmartClientCredentialsJwt() =>
        IsConnected && SupportOAuth && !IsLoading &&
        !string.IsNullOrWhiteSpace(SmartClientId) &&
        !string.IsNullOrWhiteSpace(SmartPrivateKeyPem);

    [RelayCommand(CanExecute = nameof(CanRunSmartBrowserLogin))]
    private async Task RunSmartBrowserLoginAsync()
    {
        if (!CanRunSmartBrowserLogin())
            return;

        IsLoading = true;
        StatusMessage = "SMART：等待瀏覽器完成授權…";

        try
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(12));
            var pem = string.IsNullOrWhiteSpace(SmartPrivateKeyPem) ? null : SmartPrivateKeyPem.Trim();
            var cid = string.IsNullOrWhiteSpace(SmartClientId) ? null : SmartClientId.Trim();
            var r = await _tokenServer.RunAuthorizationCodeFlowWithBrowserAsync(
                SmartScopes.Trim(), pem, cid, cts.Token);

            if (r.Success && !string.IsNullOrEmpty(r.AccessToken))
            {
                BearerAccessToken = r.AccessToken;
                SmartSessionPatient = r.Patient ?? _tokenServer.SessionPatientId ?? "";
                StatusMessage = string.IsNullOrEmpty(SmartSessionPatient)
                    ? "SMART 授權成功。"
                    : $"SMART 授權成功（patient：{SmartSessionPatient}）。";
            }
            else
                StatusMessage = $"SMART 授權失敗：{r.Error} {r.ErrorDescription}".Trim();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SMART browser login failed");
            StatusMessage = $"SMART 錯誤：{ex.Message}";
        }
        finally
        {
            IsLoading = false;
            NotifySmartCommands();
        }
    }

    [RelayCommand(CanExecute = nameof(CanRefreshSmartToken))]
    private async Task RefreshSmartTokenAsync()
    {
        if (!CanRefreshSmartToken())
            return;

        IsLoading = true;
        StatusMessage = "SMART：刷新 token…";

        try
        {
            var pem = string.IsNullOrWhiteSpace(SmartPrivateKeyPem) ? null : SmartPrivateKeyPem.Trim();
            var cid = string.IsNullOrWhiteSpace(SmartClientId) ? null : SmartClientId.Trim();
            var r = await _tokenServer.RefreshAccessTokenAsync(pem, cid);

            if (r.Success && !string.IsNullOrEmpty(r.AccessToken))
            {
                BearerAccessToken = r.AccessToken;
                StatusMessage = "SMART token 已刷新。";
            }
            else
                StatusMessage = $"SMART 刷新失敗：{r.Error} {r.ErrorDescription}".Trim();
        }
        catch (Exception ex)
        {
            StatusMessage = $"SMART 刷新錯誤：{ex.Message}";
        }
        finally
        {
            IsLoading = false;
            NotifySmartCommands();
        }
    }

    [RelayCommand(CanExecute = nameof(CanRunSmartClientCredentialsJwt))]
    private async Task RunSmartClientCredentialsJwtAsync()
    {
        if (!CanRunSmartClientCredentialsJwt())
            return;

        IsLoading = true;
        StatusMessage = "SMART：JWT client_credentials…";

        try
        {
            var r = await _tokenServer.ClientCredentialsWithJwtAsync(
                SmartScopes.Trim(),
                SmartPrivateKeyPem.Trim(),
                SmartClientId.Trim());

            if (r.Success && !string.IsNullOrEmpty(r.AccessToken))
            {
                BearerAccessToken = r.AccessToken;
                StatusMessage = "SMART client_credentials（JWT）成功。";
            }
            else
                StatusMessage = $"SMART client_credentials 失敗：{r.Error} {r.ErrorDescription}".Trim();
        }
        catch (Exception ex)
        {
            StatusMessage = $"SMART client_credentials 錯誤：{ex.Message}";
        }
        finally
        {
            IsLoading = false;
            NotifySmartCommands();
        }
    }

    [RelayCommand]
    private void ClearSmartSession()
    {
        _tokenServer.ClearSession();
        BearerAccessToken = "";
        SmartSessionPatient = "";
        StatusMessage = "SMART session 已清除。";
        NotifySmartCommands();
    }

    partial void OnQueryResultChanged(string value)
    {
        CopyQueryResultCommand.NotifyCanExecuteChanged();
        SaveQueryResultCommand.NotifyCanExecuteChanged();
    }

    partial void OnShowQueryResultAsTreeChanged(bool value) =>
        OnPropertyChanged(nameof(ResultViewToggleLabel));

    /// <summary>非空時顯示 patient context 摘要（供 Avalonia 綁定）。</summary>
    public string SmartPatientSummary =>
        string.IsNullOrWhiteSpace(SmartSessionPatient) ? "" : $"SMART patient context: {SmartSessionPatient.Trim()}";

    partial void OnSmartSessionPatientChanged(string value) =>
        OnPropertyChanged(nameof(SmartPatientSummary));

    partial void OnCanSaveResultChanged(bool value)
    {
        SaveQueryResultCommand.NotifyCanExecuteChanged();
    }

    private void OnErrorOccurred(object? sender, ErrorInfo errorInfo)
    {
        RunOnUi(() =>
        {
            StatusMessage = errorInfo.Message;

            if (errorInfo.Severity >= ErrorSeverity.Error)
                ValidationErrors.Add(errorInfo.Message);
        });
    }

    private void OnProgressChanged(object? sender, ProgressInfo progressInfo)
    {
        RunOnUi(() =>
        {
            StatusMessage = progressInfo.Message;
            IsLoading = !progressInfo.IsCompleted && !progressInfo.IsCancelled && progressInfo.Error == null;
        });
    }

    private void OnOperationCompleted(object? sender, ProgressInfo progressInfo)
    {
        RunOnUi(() =>
        {
            StatusMessage = progressInfo.Message;
            IsLoading = false;
        });
    }

    private void OnOperationFailed(object? sender, ProgressInfo progressInfo)
    {
        RunOnUi(() =>
        {
            StatusMessage = progressInfo.Message;
            IsLoading = false;

            if (progressInfo.Error != null)
                _errorHandlingService.HandleError(progressInfo.Error, progressInfo.Operation);
        });
    }

    [RelayCommand]
    private async Task ConnectToServerAsync()
    {
        if (IsLoading) return;

        ValidationErrors.Clear();

        var connectionValidation = _validationService.ValidateConnectionSettings(ServerUrl);
        if (!connectionValidation.IsValid)
        {
            foreach (var error in connectionValidation.Errors)
                ValidationErrors.Add(error);
            _errorHandlingService.HandleError("Invalid connection settings", ErrorSeverity.Warning, "ConnectToServer");
            StatusMessage = "Invalid server URL";
            return;
        }

        foreach (var warning in connectionValidation.Warnings)
            _errorHandlingService.HandleError(warning, ErrorSeverity.Warning, "ConnectToServer");

        var operationId = _progressService.StartOperation("ConnectToServer", "Starting connection...");

        try
        {
            _cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(_options.RequestTimeoutSeconds));

            _progressService.UpdateProgress(operationId, "Validating server capability...", 25);
            var capabilityValidation = await _validationService.ValidateServerCapabilityAsync(ServerUrl, _cancellationTokenSource.Token);
            if (!capabilityValidation.IsValid)
            {
                foreach (var error in capabilityValidation.Errors)
                    _errorHandlingService.HandleError(error, ErrorSeverity.Error, "ServerCapability");
                _progressService.FailOperation(operationId, new InvalidOperationException("Server capability validation failed"));
                return;
            }

            _progressService.UpdateProgress(operationId, "Connecting to server...", 50);
            var capability = await _fhirQueryService.ConnectToServerAsync(ServerUrl, _cancellationTokenSource.Token);

            if (capability != null)
            {
                IsConnected = true;

                SupportOAuth = _fhirQueryService.SupportOAuth;
                TokenUrl = _fhirQueryService.TokenUrl ?? "";
                AuthorizeUrl = _fhirQueryService.AuthorizeUrl ?? "";
                NotifySmartCommands();

                _progressService.UpdateProgress(operationId, "Loading supported resources...", 75);
                await LoadSupportedResourcesAsync();

                _progressService.CompleteOperation(operationId, "Connected successfully");
                _errorHandlingService.HandleError($"Successfully connected to {ServerUrl}", ErrorSeverity.Info, "ConnectToServer");
            }
            else
            {
                IsConnected = false;
                _progressService.FailOperation(operationId, new InvalidOperationException("Failed to retrieve server capability statement"));
            }
        }
        catch (OperationCanceledException)
        {
            _progressService.CancelOperation(operationId, $"Connection timed out after {_options.RequestTimeoutSeconds} seconds");
            _errorHandlingService.HandleError($"Connection to {ServerUrl} timed out after {_options.RequestTimeoutSeconds} seconds", ErrorSeverity.Warning, "ConnectToServer");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to connect to server: {ServerUrl}", ServerUrl);
            IsConnected = false;
            _progressService.FailOperation(operationId, ex, $"Connection failed: {ex.Message}");
            _errorHandlingService.HandleError(ex, "ConnectToServer", new Dictionary<string, object> { { "ServerUrl", ServerUrl } });
        }
        finally
        {
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
        }
    }

    [RelayCommand]
    private async Task SelectResourceAsync()
    {
        if (string.IsNullOrEmpty(SelectedResource) || !IsConnected)
            return;

        try
        {
            IsLoading = true;
            StatusMessage = $"Loading parameters for {SelectedResource}...";

            DraftSearchParameters.Clear();
            SelectedSearchParameter = null;

            await LoadSearchParametersAsync(SelectedResource);
            await LoadSearchIncludesAsync(SelectedResource);
            await LoadSearchRevIncludesAsync(SelectedResource);

            Modifying.RefreshElementChoices(SelectedResource);

            StatusMessage = $"Loaded parameters for {SelectedResource}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load resource parameters: {ResourceName}", SelectedResource);
            StatusMessage = $"Failed to load parameters: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>WinForms：由 UI 先 <c>SyncParametersToViewModel</c> 再呼叫；僅依目前 <see cref="QueryParameters"/>／<see cref="ModifyingParameters"/> 組 URL。</summary>
    [RelayCommand]
    private void BuildQuery()
    {
        if (!ValidateQueryBuildBasics())
            return;

        try
        {
            ComposeQueryUrlFromCurrentCollections();
            CanExecuteQuery = true;
            StatusMessage = "Query built successfully";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to build query");
            StatusMessage = $"Failed to build query: {ex.Message}";
        }
    }

    /// <summary>Avalonia：合併草稿條件、<c>_include</c>／<c>_revinclude</c> 勾選後組出 <see cref="QueryUrl"/>。</summary>
    [RelayCommand]
    private void BuildComposedQuery()
    {
        if (!ValidateQueryBuildBasics())
            return;

        try
        {
            QueryParameters.Clear();
            ModifyingParameters.Clear();

            foreach (var line in DraftSearchParameters)
            {
                if (!string.IsNullOrWhiteSpace(line))
                    AddQueryParameter(line.Trim());
            }

            var inc = FormatIncludeSegment(IncludeTakeAll, IncludeChoices, "include");
            if (!string.IsNullOrEmpty(inc))
                AddQueryParameter(inc);

            var rev = FormatIncludeSegment(RevIncludeTakeAll, RevIncludeChoices, "revinclude");
            if (!string.IsNullOrEmpty(rev))
                AddQueryParameter(rev);

            MergeAdvancedFluentSegments();

            foreach (var line in Modifying.ToParameterStrings())
                AddModifyingParameter(line);

            ComposeQueryUrlFromCurrentCollections();
            CanExecuteQuery = true;
            StatusMessage = "Query built successfully";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to build composed query");
            StatusMessage = $"Failed to build query: {ex.Message}";
        }
    }

    private bool ValidateQueryBuildBasics()
    {
        if (string.IsNullOrEmpty(SelectedResource))
        {
            StatusMessage = "Please select a resource type";
            return false;
        }

        var baseUrl = ServerUrl.TrimEnd('/').Trim();
        if (string.IsNullOrEmpty(baseUrl))
        {
            StatusMessage = "Please set FHIR server URL";
            return false;
        }

        return true;
    }

    private void ComposeQueryUrlFromCurrentCollections()
    {
        var baseUrl = ServerUrl.TrimEnd('/').Trim();
        var segments = new List<string>(QueryParameters.Count + ModifyingParameters.Count);
        void AppendSplitParameters(string? raw)
        {
            if (string.IsNullOrEmpty(raw)) return;
            foreach (var piece in raw.Split('&', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                if (piece.Length > 0)
                    segments.Add(EncodeQueryParameterSegment(piece));
            }
        }

        foreach (var p in QueryParameters)
            AppendSplitParameters(p);
        foreach (var p in ModifyingParameters)
            AppendSplitParameters(p);

        var resourcePath = $"{baseUrl}/{SelectedResource.Trim()}";
        QueryUrl = segments.Count == 0 ? resourcePath : $"{resourcePath}?{string.Join("&", segments)}";
    }

    private static string? FormatIncludeSegment(bool takeAll, ObservableCollection<SelectableIncludeRow> rows, string includeParameterName)
    {
        List<string> tmp = [];
        foreach (var row in rows)
        {
            if (takeAll || row.IsSelected)
                tmp.Add($"_{includeParameterName}={row.Value}");
        }

        return tmp.Count == 0 ? null : string.Join("&", tmp);
    }

    /// <summary>將進階 Fluent 片段併入 <see cref="QueryParameters"/>／<see cref="ModifyingParameters"/>（對齊 WinForms ApplyAdvancedSearchParameters）。</summary>
    private void MergeAdvancedFluentSegments()
    {
        var builder = _queryBuilderFactory();
        builder.ForResource(SelectedResource!.Trim());
        var result = Advanced.BuildResultControlParameters();
        AdvancedSearchApplicator.Apply(builder, result, Advanced.Chains, Advanced.Composites, Advanced.Filters);
        var qs = builder.BuildQueryString();
        if (string.IsNullOrEmpty(qs))
            return;

        foreach (var piece in qs.Split('&', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var p = piece.Trim();
            if (p.Length == 0)
                continue;
            if (p.StartsWith('_'))
                AddModifyingParameter(p);
            else
                AddQueryParameter(p);
        }
    }

    [RelayCommand(CanExecute = nameof(CanAddTypedSearchClause))]
    private void AddTypedSearchClause()
    {
        var name = SelectedSearchParameter?.Name?.Trim();
        var type = SelectedSearchParameter?.Type?.Trim().ToLowerInvariant();
        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(type))
            return;

        var suffix = SearchParameterComposition.ComposeSuffixForSearchParamType(type, TypedSearch);
        if (suffix == null)
        {
            StatusMessage = $"Unsupported or unknown search parameter type: {type}";
            return;
        }

        DraftSearchParameters.Add($"{name}{suffix}");
        StatusMessage = $"Added typed clause for {name}";
    }

    private bool CanAddTypedSearchClause() =>
        SelectedSearchParameter != null &&
        !string.IsNullOrWhiteSpace(SelectedSearchParameter.Name);

    [RelayCommand]
    private void TryAddAdvancedChain() =>
        StatusMessage = Advanced.TryAddPendingChain() ? "Chain parameter added." : "Enter chain path and value.";

    [RelayCommand]
    private void TryAddAdvancedComposite() =>
        StatusMessage = Advanced.TryAddPendingComposite() ? "Composite parameter added." : "Enter composite name and at least two comma-separated components.";

    [RelayCommand]
    private void TryAddAdvancedFilter() =>
        StatusMessage = Advanced.TryAddPendingFilter() ? "Filter expression added." : "Enter a filter expression.";

    [RelayCommand]
    private void ClearAdvancedSearch()
    {
        Advanced.ClearAll();
        StatusMessage = "Advanced search cleared.";
    }

    [RelayCommand(CanExecute = nameof(CanAddSearchParameterHint))]
    private void AddSelectedSearchParameterHint()
    {
        if (SelectedSearchParameter == null)
            return;
        DraftSearchParameters.Add($"{SelectedSearchParameter.Name}=");
    }

    private bool CanAddSearchParameterHint() => SelectedSearchParameter != null;

    partial void OnSelectedSearchParameterChanged(SearchParamModel? value)
    {
        AddSelectedSearchParameterHintCommand.NotifyCanExecuteChanged();
        AddTypedSearchClauseCommand.NotifyCanExecuteChanged();
        RefreshTypedEditorVisibility();
    }

    private void RefreshTypedEditorVisibility()
    {
        var k = SelectedSearchParameter?.Type?.Trim().ToLowerInvariant() ?? "";
        EditorUnsupportedVisible = false;
        EditorStringVisible = k == "string";
        EditorTokenVisible = k == "token";
        EditorUriVisible = k == "uri";
        EditorReferenceVisible = k == "reference";
        EditorNumberVisible = k == "number";
        EditorDateVisible = k == "date";
        EditorQuantityVisible = k == "quantity";
        if (!EditorStringVisible && !EditorTokenVisible && !EditorUriVisible && !EditorReferenceVisible
            && !EditorNumberVisible && !EditorDateVisible && !EditorQuantityVisible && !string.IsNullOrEmpty(k))
            EditorUnsupportedVisible = true;
    }

    [RelayCommand]
    private void AddPendingDraftLine()
    {
        if (string.IsNullOrWhiteSpace(PendingDraftLine))
            return;
        DraftSearchParameters.Add(PendingDraftLine.Trim());
        PendingDraftLine = string.Empty;
    }

    [RelayCommand(CanExecute = nameof(CanRemoveSelectedDraft))]
    private void RemoveSelectedDraftFromList()
    {
        if (string.IsNullOrEmpty(SelectedDraftParameter))
            return;
        var i = DraftSearchParameters.IndexOf(SelectedDraftParameter);
        if (i >= 0)
            DraftSearchParameters.RemoveAt(i);
        SelectedDraftParameter = null;
    }

    private bool CanRemoveSelectedDraft() => !string.IsNullOrEmpty(SelectedDraftParameter);

    [RelayCommand]
    private void ClearAllDraftSearchParameters()
    {
        DraftSearchParameters.Clear();
        SelectedDraftParameter = null;
        RemoveSelectedDraftFromListCommand.NotifyCanExecuteChanged();
    }

    partial void OnSelectedDraftParameterChanged(string? value)
    {
        RemoveSelectedDraftFromListCommand.NotifyCanExecuteChanged();
    }

    /// <summary>將單一 query 片段 <c>name=value</c> 編碼（value 依 RFC 3986 規則）。</summary>
    private static string? NormalizeBearerToken(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return null;

        var t = raw.Trim();
        if (t.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            t = t[7..].Trim();

        return string.IsNullOrEmpty(t) ? null : t;
    }

    private static string EncodeQueryParameterSegment(string segment)
    {
        var eq = segment.IndexOf('=');
        if (eq <= 0)
            return segment;
        var name = segment[..eq];
        var value = segment[(eq + 1)..];
        return $"{name}={Uri.EscapeDataString(value)}";
    }

    [RelayCommand]
    private async Task ExecuteQueryAsync()
    {
        if (string.IsNullOrEmpty(QueryUrl) || !IsConnected)
            return;

        var complexityValidation = _validationService.ValidateQueryComplexity(QueryUrl);
        foreach (var warning in complexityValidation.Warnings)
            _errorHandlingService.HandleError(warning, ErrorSeverity.Warning, "QueryExecution");

        if (!complexityValidation.IsValid)
        {
            foreach (var error in complexityValidation.Errors)
                _errorHandlingService.HandleError(error, ErrorSeverity.Error, "QueryValidation");
            StatusMessage = "Query validation failed";
            return;
        }

        var operationId = _progressService.StartOperation("ExecuteQuery", "Preparing query...");

        try
        {
            _cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(_options.RequestTimeoutSeconds * 2));

            _progressService.UpdateProgress(operationId, "Executing FHIR query...", 50);
            var result = await _fhirQueryService.ExecuteQueryAsync(QueryUrl, NormalizeBearerToken(BearerAccessToken), _cancellationTokenSource.Token);

            _progressService.UpdateProgress(operationId, "Processing results...", 90);

            await RunOnUiAsync(() =>
            {
                if (!string.IsNullOrEmpty(result))
                {
                    QueryResult = result;
                    CanSaveResult = true;
                }
                else
                {
                    QueryResult = string.Empty;
                    CanSaveResult = false;
                }
            }).ConfigureAwait(false);

            if (!string.IsNullOrEmpty(result))
            {
                _progressService.CompleteOperation(operationId, "Query executed successfully");
                _errorHandlingService.HandleError($"Query executed successfully: {QueryUrl}", ErrorSeverity.Info, "QueryExecution");
            }
            else
            {
                _progressService.CompleteOperation(operationId, "Query returned no results");
                _errorHandlingService.HandleError("Query returned empty result", ErrorSeverity.Warning, "QueryExecution");
            }
        }
        catch (OperationCanceledException)
        {
            _progressService.CancelOperation(operationId, "Query execution timed out");
            _errorHandlingService.HandleError($"Query execution timed out: {QueryUrl}", ErrorSeverity.Warning, "QueryExecution");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to execute query: {QueryUrl}", QueryUrl);
            _progressService.FailOperation(operationId, ex, $"Query execution failed: {ex.Message}");
            _errorHandlingService.HandleError(ex, "QueryExecution", new Dictionary<string, object> { { "QueryUrl", QueryUrl } });
        }
        finally
        {
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
        }
    }

    [RelayCommand]
    private void CancelOperation()
    {
        _cancellationTokenSource?.Cancel();

        var activeOperations = _progressService.GetActiveOperations();
        foreach (var operation in activeOperations)
            _progressService.CancelOperation(operation.Id, "User cancelled");

        StatusMessage = "Operation cancelled";
        IsLoading = false;
    }

    private bool CanCopyQueryResult() => _clipboardText != null && !string.IsNullOrEmpty(QueryResult);

    [RelayCommand(CanExecute = nameof(CanCopyQueryResult))]
    private async Task CopyQueryResultAsync()
    {
        if (_clipboardText == null || string.IsNullOrEmpty(QueryResult))
            return;
        await _clipboardText.SetTextAsync(QueryResult);
        StatusMessage = "Result copied to clipboard.";
    }

    private bool CanSaveQueryResult() =>
        _filePickerSave != null && CanSaveResult && !string.IsNullOrEmpty(QueryResult);

    [RelayCommand(CanExecute = nameof(CanSaveQueryResult))]
    private async Task SaveQueryResultAsync()
    {
        if (_filePickerSave == null || string.IsNullOrEmpty(QueryResult))
            return;

        var suggested = BuildSuggestedJsonFileName();
        var path = await _filePickerSave.PickSaveFilePathAsync(suggested);
        if (path != null)
            await File.WriteAllTextAsync(path, QueryResult);
    }

    private string BuildSuggestedJsonFileName()
    {
        var r = string.IsNullOrWhiteSpace(SelectedResource) ? "result" : SelectedResource.Trim();
        foreach (var c in Path.GetInvalidFileNameChars())
            r = r.Replace(c, '_');
        return $"{r}_{DateTime.Now:yyyyMMdd}.json";
    }

    [RelayCommand]
    private void ClearQueryUrlField()
    {
        QueryUrl = string.Empty;
        CanExecuteQuery = false;
    }

    [RelayCommand]
    private void ClearQueryResultField()
    {
        QueryResult = string.Empty;
        QueryResultTreeRoots.Clear();
        CanSaveResult = false;
        StatusMessage = "Result cleared.";
    }

    [RelayCommand]
    private void ClearValidationErrors()
    {
        ValidationErrors.Clear();
        OnPropertyChanged(nameof(HasValidationErrors));
    }

    private bool CanModifyingPickElement() => !string.IsNullOrEmpty(Modifying.SelectedElementHint);

    [RelayCommand(CanExecute = nameof(CanModifyingPickElement))]
    private void ModifyingAddElement() => Modifying.AddSelectedToElements();

    [RelayCommand(CanExecute = nameof(CanModifyingPickElement))]
    private void ModifyingAddSort() => Modifying.AddSelectedToSort();

    [RelayCommand]
    private void ModifyingClearElements() => Modifying.Elements.Clear();

    [RelayCommand]
    private void ModifyingClearSort() => Modifying.SortLines.Clear();

    [RelayCommand]
    private void ModifyingResetForm()
    {
        Modifying.ResetForm();
        StatusMessage = "Modifying options reset.";
    }

    [RelayCommand]
    private void ToggleQueryResultTree()
    {
        if (!ShowQueryResultAsTree)
        {
            QueryResultTreeRoots.Clear();
            foreach (var root in JsonTreeBuilder.BuildRootsFromJsonText(QueryResult))
                QueryResultTreeRoots.Add(root);

            ShowQueryResultAsTree = true;
            StatusMessage = QueryResultTreeRoots.Count > 0 ? "Tree view (parsed JSON)." : "Tree view: empty or invalid JSON — check raw text.";
        }
        else
        {
            ShowQueryResultAsTree = false;
            StatusMessage = "Text view.";
        }
    }

    private bool CanCopyTokenUrl() => _clipboardText != null && !string.IsNullOrWhiteSpace(TokenUrl);

    [RelayCommand(CanExecute = nameof(CanCopyTokenUrl))]
    private async Task CopyTokenUrlAsync()
    {
        if (_clipboardText == null || string.IsNullOrWhiteSpace(TokenUrl))
            return;

        await _clipboardText.SetTextAsync(TokenUrl);
        StatusMessage = "Token URL copied.";
    }

    partial void OnTokenUrlChanged(string value) => CopyTokenUrlCommand.NotifyCanExecuteChanged();

    [RelayCommand]
    private void OpenAuthorizeUrl()
    {
        if (string.IsNullOrWhiteSpace(AuthorizeUrl))
            return;

        TryOpenUrlInBrowser(AuthorizeUrl);
    }

    [RelayCommand]
    private void OpenTokenUrl()
    {
        if (string.IsNullOrWhiteSpace(TokenUrl))
            return;

        TryOpenUrlInBrowser(TokenUrl);
    }

    private static void TryOpenUrlInBrowser(string url)
    {
        try
        {
            Process.Start(new ProcessStartInfo { FileName = url, UseShellExecute = true });
        }
        catch
        {
            // 忽略：無預設瀏覽器或 URL 無效
        }
    }

    private async Task LoadSupportedResourcesAsync()
    {
        var resources = await _fhirQueryService.GetSupportedResourcesAsync().ConfigureAwait(false);
        var ordered = resources?.OrderBy(r => r).ToList() ?? new List<string>();
        await RunOnUiAsync(() =>
        {
            SupportedResources.Clear();
            foreach (var resource in ordered)
                SupportedResources.Add(resource);
        }).ConfigureAwait(false);
    }

    private async Task LoadSearchParametersAsync(string resourceName)
    {
        var parameters = await _fhirQueryService.GetSearchParametersAsync(resourceName).ConfigureAwait(false);
        if (parameters == null)
            return;

        var list = parameters.OrderBy(p => p.Name).ToList();
        await RunOnUiAsync(() =>
        {
            SearchParameters.Clear();
            foreach (var param in list)
                SearchParameters.Add(param);
        }).ConfigureAwait(false);
    }

    private async Task LoadSearchIncludesAsync(string resourceName)
    {
        var includes = await _fhirQueryService.GetSearchIncludeAsync(resourceName).ConfigureAwait(false);
        if (includes == null)
            return;

        var arr = includes.ToArray();
        await RunOnUiAsync(() =>
        {
            SearchIncludes.Clear();
            IncludeChoices.Clear();
            foreach (var include in arr)
            {
                SearchIncludes.Add(include);
                IncludeChoices.Add(new SelectableIncludeRow(include));
            }
        }).ConfigureAwait(false);
    }

    private async Task LoadSearchRevIncludesAsync(string resourceName)
    {
        var revIncludes = await _fhirQueryService.GetSearchRevIncludeAsync(resourceName).ConfigureAwait(false);
        if (revIncludes == null)
            return;

        var arr = revIncludes.ToArray();
        await RunOnUiAsync(() =>
        {
            SearchRevIncludes.Clear();
            RevIncludeChoices.Clear();
            foreach (var revInclude in arr)
            {
                SearchRevIncludes.Add(revInclude);
                RevIncludeChoices.Add(new SelectableIncludeRow(revInclude));
            }
        }).ConfigureAwait(false);
    }

    public void AddQueryParameter(string parameter)
    {
        if (!string.IsNullOrEmpty(parameter) && !QueryParameters.Contains(parameter))
            QueryParameters.Add(parameter);
    }

    public void RemoveQueryParameter(string parameter)
    {
        QueryParameters.Remove(parameter);
    }

    public void AddModifyingParameter(string parameter)
    {
        if (!string.IsNullOrEmpty(parameter) && !ModifyingParameters.Contains(parameter))
            ModifyingParameters.Add(parameter);
    }

    public void RemoveModifyingParameter(string parameter)
    {
        ModifyingParameters.Remove(parameter);
    }

    public IFhirQueryBuilder GetQueryBuilder()
    {
        return _queryBuilderFactory();
    }

    public void AddStringParameter(string parameterName, string value, SearchModifier modifier = SearchModifier.None)
    {
        try
        {
            _queryBuilderFactory().ForResource(SelectedResource).WhereString(parameterName, value, modifier);
            var parameter = $"{parameterName}={value}";
            if (modifier != SearchModifier.None)
                parameter = $"{parameterName}:{modifier.ToString().ToLower()}={value}";
            AddQueryParameter(parameter);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add string parameter: {ParameterName}", parameterName);
            ValidationErrors.Add($"Failed to add parameter: {ex.Message}");
        }
    }

    public void AddDateParameter(string parameterName, DateTime value, SearchPrefix prefix = SearchPrefix.None)
    {
        try
        {
            _queryBuilderFactory().ForResource(SelectedResource).WhereDate(parameterName, value, prefix);
            var prefixStr = prefix != SearchPrefix.None ? GetPrefixString(prefix) : "";
            var parameter = $"{parameterName}={prefixStr}{value:yyyy-MM-dd}";
            AddQueryParameter(parameter);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add date parameter: {ParameterName}", parameterName);
            ValidationErrors.Add($"Failed to add parameter: {ex.Message}");
        }
    }

    public void AddTokenParameter(string parameterName, string code, string? system = null)
    {
        try
        {
            _queryBuilderFactory().ForResource(SelectedResource).WhereToken(parameterName, code, system);
            var parameter = string.IsNullOrEmpty(system) ?
                $"{parameterName}={code}" :
                $"{parameterName}={system}|{code}";
            AddQueryParameter(parameter);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add token parameter: {ParameterName}", parameterName);
            ValidationErrors.Add($"Failed to add parameter: {ex.Message}");
        }
    }

    private static string GetPrefixString(SearchPrefix prefix)
    {
        return prefix switch
        {
            SearchPrefix.Equal => "eq",
            SearchPrefix.NotEqual => "ne",
            SearchPrefix.GreaterThan => "gt",
            SearchPrefix.LessThan => "lt",
            SearchPrefix.GreaterEqual => "ge",
            SearchPrefix.LessEqual => "le",
            SearchPrefix.StartsAfter => "sa",
            SearchPrefix.EndsBefore => "eb",
            SearchPrefix.Approximate => "ap",
            _ => ""
        };
    }

    /// <summary>Avalonia ComboBox：對齊 WinForms cobDate_prefix。</summary>
    public string[] DateSearchPrefixes { get; } = ["eq", "ne", "gt", "lt", "ge", "le", "sa", "eb", "ap"];

    /// <summary>Avalonia ComboBox：對齊 WinForms cobNumber_prefix。</summary>
    public string[] NumberSearchPrefixes { get; } = ["gt", "lt", "ge", "le", "sa", "eb"];

    /// <summary>Avalonia ComboBox：對齊 WinForms cobQuantity_prefix。</summary>
    public string[] QuantitySearchPrefixes { get; } = ["eq", "ne", "gt", "lt", "ge", "le", "sa", "eb", "ap"];

    public string[] TotalModes { get; } = ["none", "estimate", "accurate"];

    public string[] SummaryModes { get; } = ["true", "false", "text", "data", "count"];
}
