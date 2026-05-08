using Fhir.QueryBuilder.AdvancedSearch;
using Fhir.QueryBuilder.Extension;
using Fhir.QueryBuilder.Platform;
using System.Data;
using System.Drawing;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading;
using Fhir.QueryBuilder.ViewModels;
using Microsoft.Extensions.Logging;
using Fhir.QueryBuilder.QueryBuilders.FluentApi;
using Fhir.QueryBuilder.Controls;

namespace Fhir.QueryBuilder
{
    public partial class NewMainForm : Form
    {
        private readonly MainViewModel _viewModel;
        private readonly ILogger<NewMainForm> _logger;

        string? _tokenUrl;
        string? _tokenValue;
        string? _oAuthValue;

        ModifyResult? modifyResult;
        readonly string _defaultUrl = new("https://server.fire.ly");

        public NewMainForm(MainViewModel viewModel, ILogger<NewMainForm> logger)
        {
            _viewModel = viewModel;
            _logger = logger;
            InitializeComponent();

            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            tabControl1.Font = Font;
            tab_LastControl.Font = Font;

            Load += NewMainForm_Load;
            Resize += (_, _) => ApplyResultPanelLayout();

            InitializeViewModel();
        }

        private void NewMainForm_Load(object? sender, EventArgs e)
        {
            _viewModel.SetUiSynchronizationContext(SynchronizationContext.Current);
            ApplyResultPanelLayout();
        }

        /// <summary>
        /// 右欄結果區（JSON／Tree）隨視窗拉伸，避免 Tree View 高度不足。
        /// </summary>
        private void ApplyResultPanelLayout()
        {
            const int resultLeft = 744;
            const int bottomReserve = 48;
            var marginRight = 16;
            var top = txt_Message.Top;
            var w = Math.Max(200, ClientSize.Width - resultLeft - marginRight);
            var h = Math.Max(200, ClientSize.Height - top - bottomReserve);
            txt_Message.SetBounds(resultLeft, top, w, h);
            trv_Message.SetBounds(resultLeft, top, w, h);
        }

        private void InitializeViewModel()
        {
            // Bind ViewModel properties to UI controls
            txt_FhirUrl.Text = _viewModel.ServerUrl;

            // Subscribe to ViewModel property changes
            _viewModel.PropertyChanged += ViewModel_PropertyChanged;

            // Initialize collections
            _viewModel.SupportedResources.CollectionChanged += SupportedResources_CollectionChanged;
            _viewModel.SearchParameters.CollectionChanged += SearchParameters_CollectionChanged;
            _viewModel.ValidationErrors.CollectionChanged += ValidationErrors_CollectionChanged;

            // Initialize advanced search control
            InitializeAdvancedSearchControl();
        }

        private void InitializeAdvancedSearchControl()
        {
            // Subscribe to advanced parameters changes
            advancedSearchControl.ParametersChanged += AdvancedSearchControl_ParametersChanged;
        }

        private void AdvancedSearchControl_ParametersChanged(object? sender, AdvancedParametersChangedEventArgs e)
        {
            // Update the search URL when advanced parameters change
            UpdateSearchUrl();
        }

        private void UpdateSearchUrl()
        {
            try
            {
                if (string.IsNullOrEmpty(_viewModel.SelectedResource))
                    return;

                // Build the complete query URL including advanced parameters
                SyncParametersToViewModel();
                _viewModel.BuildQueryCommand.Execute(null);
                Txt_SearchUrl.Text = _viewModel.QueryUrl;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating search URL");
            }
        }

        private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(() => ViewModel_PropertyChanged(sender, e));
                return;
            }

            switch (e.PropertyName)
            {
                case nameof(_viewModel.IsConnected):
                    UpdateConnectionStatus();
                    break;
                case nameof(_viewModel.IsLoading):
                    UpdateLoadingStatus();
                    break;
                case nameof(_viewModel.StatusMessage):
                    UpdateStatusMessage();
                    break;
                case nameof(_viewModel.QueryResult):
                    UpdateQueryResult();
                    break;
                case nameof(_viewModel.CanExecuteQuery):
                    Btn_Search.Enabled = _viewModel.CanExecuteQuery;
                    break;
                case nameof(_viewModel.CanSaveResult):
                    Btn_Save.Enabled = _viewModel.CanSaveResult;
                    Btn_Copy.Enabled = _viewModel.CanSaveResult;
                    break;
            }
        }

        private void SupportedResources_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(() => SupportedResources_CollectionChanged(sender, e));
                return;
            }

            Cob_Resource.Items.Clear();
            foreach (var resource in _viewModel.SupportedResources)
            {
                Cob_Resource.Items.Add(resource);
            }
        }

        private void SearchParameters_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(() => SearchParameters_CollectionChanged(sender, e));
                return;
            }

            Lib_CommonParameter.Items.Clear();
            foreach (var param in _viewModel.SearchParameters)
            {
                Lib_CommonParameter.Items.Add($"{param.Name} : {param.Type}");
            }
        }

        private void ValidationErrors_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(() => ValidationErrors_CollectionChanged(sender, e));
                return;
            }

            if (_viewModel.ValidationErrors.Any())
            {
                var errorMessage = string.Join("\n", _viewModel.ValidationErrors);
                MessageBox.Show(errorMessage, "Validation Errors", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void UpdateConnectionStatus()
        {
            // Update UI based on connection status
            Btn_ModifyingResult.Enabled = _viewModel.IsConnected;
        }

        private void UpdateLoadingStatus()
        {
            lab_Loading.Visible = _viewModel.IsLoading;
            Btn_Connect.Enabled = !_viewModel.IsLoading;
        }

        private void UpdateStatusMessage()
        {
            // You might want to add a status label to show messages
            _logger.LogInformation("Status: {StatusMessage}", _viewModel.StatusMessage);
        }

        private void UpdateQueryResult()
        {
            txt_Message.Text = _viewModel.QueryResult;
            if (!string.IsNullOrEmpty(_viewModel.QueryResult))
            {
                txt_Message.Visible = true;
                trv_Message.Visible = false;
                Btn_TreeView.Text = "Tree View";
            }
        }
        #region Event Method
        private void Btn_ModifyingResult_Click(object sender, EventArgs e)
        {
            Lib_ModifyingList.Items.Clear();
            if (modifyResult != null)
            {
                modifyResult.Activate();
                if (modifyResult.ShowDialog() == DialogResult.OK)
                {
                    // Jut get the result
                }

                Lib_ModifyingList.Items.AddRange(modifyResult.GetModifingQueryList().ToArray());
            }
        }



        private void Btn_TreeView_Click(object sender, EventArgs e)
        {
            if (trv_Message.Nodes.Count == 0 && (txt_Message.Text.StartsWith('[') || txt_Message.Text.StartsWith('{')))
            {
                JsonNode? jNode = JsonNode.Parse(txt_Message.Text);
                trv_Message.InitFromJsonNode(jNode);
            }
            if (Btn_TreeView.Text == "Tree View")
            {
                txt_Message.Visible = false;
                trv_Message.Visible = true;
                Btn_TreeView.Text = "Message View";
            }
            else
            {
                txt_Message.Visible = true;
                trv_Message.Visible = false;
                Btn_TreeView.Text = "Tree View";
            }

        }
        private async void Btn_Copy_Click(object sender, EventArgs e)
        {
            _viewModel.QueryResult = txt_Message.Text;
            await _viewModel.CopyQueryResultCommand.ExecuteAsync(null);
        }

        private async void Btn_Save_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txt_Message.Text))
            {
                MessageBox.Show("Search Resource first!");
                return;
            }

            _viewModel.QueryResult = txt_Message.Text;
            await _viewModel.SaveQueryResultCommand.ExecuteAsync(null);
        }

        private void Btn_Exit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private async void Btn_Search_Click(object sender, EventArgs e)
        {
            txt_Message.Text = "";
            trv_Message.Nodes.Clear();

            if (string.IsNullOrEmpty(Txt_SearchUrl.Text))
            {
                MessageBox.Show("Please create a search URL first.");
                return;
            }

            _viewModel.QueryUrl = Txt_SearchUrl.Text;
            await _viewModel.ExecuteQueryCommand.ExecuteAsync(null);
        }
        private async void Btn_Connect_Click(object sender, EventArgs e)
        {
            ResetControl();

            // Update ViewModel with current URL
            _viewModel.ServerUrl = txt_FhirUrl.Text;

            // Use ViewModel to connect
            await _viewModel.ConnectToServerCommand.ExecuteAsync(null);

            // Handle OAuth if supported
            if (_viewModel.SupportOAuth)
            {
                Btn_GetToken.Visible = true;
                txt_Message.Text = await WellKnownSmartConfig.FetchAsync(txt_FhirUrl.Text);
            }
            _tokenUrl = _viewModel.TokenUrl;
        }


        private void Btn_GetToken_Click(object sender, EventArgs e)
        {
            if (Btn_GetToken.Text == "View Token")
            {
                txt_Message.Text = _oAuthValue;
                txt_Message.Visible = true;
            }
            else
            {

                GetToken getToken = new(_tokenUrl);

                if (getToken.ShowDialog() == DialogResult.OK)
                {
                    // Just get the result
                }
                if (getToken.GetTokenValue() is string tValue)
                {
                    _oAuthValue = tValue;
                    txt_Message.Text = _oAuthValue;
                    var jObject = JsonNode.Parse(tValue);
                    if (jObject != null)
                    {
                        _tokenValue = jObject["access_token"]?.GetValue<string>();

                        SetupComboResource(jObject["scope"]?.GetValue<string>());
                    }
                    Btn_GetToken.Text = "View Token";
                }
                else
                {
                    txt_Message.Text = "Cannot get token";
                }
            }
        }
        private async void Cob_Resource_SelectedIndexChanged(object sender, EventArgs e)
        {
            ResetControl();
            string? resourceName = Cob_Resource.SelectedItem?.ToString();
            if (!string.IsNullOrEmpty(resourceName))
            {
                _viewModel.SelectedResource = resourceName;
                await _viewModel.SelectResourceCommand.ExecuteAsync(null);

                // Setup includes using ViewModel data
                SetupIncludeFromViewModel();

                Btn_ModifyingResult.Enabled = true;
                modifyResult = new(resourceName);
            }
        }

        private void SetupIncludeFromViewModel()
        {
            Clb_Include.Items.Clear();
            foreach (var include in _viewModel.SearchIncludes)
            {
                Clb_Include.Items.Add(include);
            }

            Clb_Revinclude.Items.Clear();
            foreach (var revInclude in _viewModel.SearchRevIncludes)
            {
                Clb_Revinclude.Items.Add(revInclude);
            }
        }
        private void Lib_CommonParameter_SelectedIndexChanged(object sender, EventArgs e)
        {
            lab_SearchField.Visible = true;
            lab_SerchType.Visible = true;
            string[]? pString = Lib_CommonParameter.SelectedItem?.ToString()?.Split(" : ");
            if (pString != null && pString.Length == 2)
            {
                ShowLabelItem(pString);
            }
            Btn_AddParameter.Enabled = true;
            CleanPanel();
        }
        private void Lib_ResourceParameter_SelectedIndexChanged(object sender, EventArgs e)
        {
            lab_SearchField.Visible = true;
            lab_SerchType.Visible = true;
            string[]? pString = Lib_ResourceParameter.SelectedItem?.ToString()?.Split(" : ");
            if (pString != null && pString.Length == 2)
            {
                ShowLabelItem(pString);
            }
            Btn_AddParameter.Enabled = true;
            CleanPanel();
        }



        private void lab_SerchType_TextChanged(object sender, EventArgs e)
        {
            SetupPanelvisible(false);
            switch (lab_SerchType.Text.Trim().ToLower())
            {
                case "date":
                    SetupPanelvisible(true, panDate);
                    break;
                case "number":
                    SetupPanelvisible(true, panNumber);
                    break;
                case "quantity":
                    SetupPanelvisible(true, panQuantity);
                    break;
                case "reference":
                    SetupPanelvisible(true, panReference);
                    break;
                case "string":
                    SetupPanelvisible(true, panString);
                    break;
                case "token":
                    SetupPanelvisible(true, panToken);
                    break;
                case "uri":
                    SetupPanelvisible(true, panUri);
                    break;
            }
        }
        private void Btn_AddParameter_Click(object sender, EventArgs e)
        {
            string oneParameter = "";
            if (lab_SerchType.Text != "")
            {
                switch (lab_SerchType.Text.Trim().ToLower())
                {
                    case "date":
                        oneParameter = GetDateParameter();
                        break;
                    case "number":
                        oneParameter = GetNumberParameter();
                        break;
                    case "quantity":
                        oneParameter = GetQuantityParameter();
                        break;
                    case "reference":
                        oneParameter = GetReferenceParameter();
                        break;
                    case "string":
                        oneParameter = GetStringParameter();
                        break;
                    case "token":
                        oneParameter = GetTokenParameter();
                        break;
                    case "uri":
                        oneParameter = GetUriParameter();
                        break;
                }
                Lib_ParameterList.Items.Add($"{lab_SearchField.Text}{oneParameter}");
                if (Lib_ParameterList.Items.Count > 0)
                {
                    Btn_RemoveParameter.Enabled = true;
                    Btn_RemoveAllParameter.Enabled = true;
                }
            }
        }
        private void Btn_RemoveParameter_Click(object sender, EventArgs e)
        {
            if (Lib_ParameterList.SelectedItem != null && Lib_ParameterList.SelectedItems.Count > 0)
            {
                Lib_ParameterList.Items.Remove(Lib_ParameterList.SelectedItem);
            }
            if (Lib_ParameterList.Items.Count == 0)
            {
                Btn_RemoveParameter.Enabled = false;
                Btn_RemoveAllParameter.Enabled = false;
            }
        }
        private void Btn_RemoveAllParameter_Click(object sender, EventArgs e)
        {
            if (Lib_ParameterList.Items.Count > 0)
            {
                Lib_ParameterList.Items.Clear();
            }
            if (Lib_ParameterList.Items.Count == 0)
            {
                Btn_RemoveAllParameter.Enabled = false;
                Btn_RemoveParameter.Enabled = false;
            }
        }
        private void Btn_CreateUrl_Click(object sender, EventArgs e)
        {
            // Sync current parameters to ViewModel
            SyncParametersToViewModel();

            // Use ViewModel to build query
            _viewModel.BuildQueryCommand.Execute(null);

            // Update UI with built query
            Txt_SearchUrl.Text = _viewModel.QueryUrl;
        }

        private void SyncParametersToViewModel()
        {
            // Clear existing parameters in ViewModel
            _viewModel.QueryParameters.Clear();
            _viewModel.ModifyingParameters.Clear();

            // Add parameters from UI lists
            foreach (string param in Lib_ParameterList.Items)
            {
                _viewModel.AddQueryParameter(param);
            }

            foreach (string param in Lib_ModifyingList.Items)
            {
                _viewModel.AddModifyingParameter(param);
            }

            // Add include parameters
            string? includeString = CheckInclude();
            if (!string.IsNullOrEmpty(includeString))
            {
                _viewModel.AddQueryParameter(includeString);
            }

            string? revincludeString = CheckRevInclude();
            if (!string.IsNullOrEmpty(revincludeString))
            {
                _viewModel.AddQueryParameter(revincludeString);
            }

            // Apply advanced search parameters using FluentApi
            ApplyAdvancedSearchParameters();
        }

        private void ApplyAdvancedSearchParameters()
        {
            try
            {
                // Get a fresh query builder instance
                var builder = _viewModel.GetQueryBuilder();

                if (builder != null && !string.IsNullOrEmpty(_viewModel.SelectedResource))
                {
                    // Set the resource type
                    builder.ForResource(_viewModel.SelectedResource);

                    // Apply advanced parameters
                    advancedSearchControl.ApplyToQueryBuilder(builder);

                    // Get the query string and add to ViewModel
                    var advancedQuery = builder.BuildQueryString();
                    if (!string.IsNullOrEmpty(advancedQuery))
                    {
                        // Parse the advanced query and add individual parameters
                        var parameters = advancedQuery.Split('&', StringSplitOptions.RemoveEmptyEntries);
                        foreach (var param in parameters)
                        {
                            if (param.StartsWith("_"))
                            {
                                _viewModel.AddModifyingParameter(param);
                            }
                            else
                            {
                                _viewModel.AddQueryParameter(param);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error applying advanced search parameters");
                MessageBox.Show($"Error applying advanced search parameters: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void Lib_ParameterList_SelectedIndexChanged(object sender, EventArgs e)
        {
            Btn_RemoveParameter.Enabled = true;
        }
        #endregion

        #region Private Methods
        private void CleanPanel()
        {
            CleanStringPanel();
            CleanDatePanel();
            CleanNumberPanel();
            CleanQuantityPanel();
            CleanReferencePanel();
            CleanTokenPanel();
            CleanUriPanel();
        }

        private void ResetControl()
        {
            txt_Message.Text = "";
            txt_Message.Visible = true;
            lab_SearchField.Text = "";
            lab_SerchType.Text = "";
            Lib_CommonParameter.Items.Clear();
            Lib_ResourceParameter.Items.Clear();
            Lib_ParameterList.Items.Clear();
            Lib_ModifyingList.Items.Clear();
            Clb_Include.Items.Clear();
            Clb_Revinclude.Items.Clear();
            Txt_SearchUrl.Text = "";
            Btn_AddParameter.Enabled = false;
            Btn_RemoveParameter.Enabled = false;
            trv_Message.Nodes.Clear();
            trv_Message.Visible = false;
            lab_SerchType.Visible = false;
            lab_SearchField.Visible = false;
            SetupPanelvisible(false);
        }

        private void SetupPanelvisible(bool toVisible, Panel? thePanel = null)
        {
            if (thePanel == null)
            {
                panDate.Visible = toVisible;
                panNumber.Visible = toVisible;
                panQuantity.Visible = toVisible;
                panReference.Visible = toVisible;
                panString.Visible = toVisible;
                panToken.Visible = toVisible;
                panUri.Visible = toVisible;
            }
            else
            {
                thePanel.Visible = toVisible;
            }

        }

        private void ShowLabelItem(string[] pString)
        {
            lab_SearchField.Text = pString[0];
            lab_SerchType.Text = pString[1];
        }
        private string? CheckInclude()
        {
            return GroupCheckedBoxItem("include", Ckb_includeAll.Checked);
        }
        private string? CheckRevInclude()
        {
            return GroupCheckedBoxItem("revinclude", Ckb_RevincludeAll.Checked);
        }
        private string? GroupCheckedBoxItem(string sourceType, bool takeAll)
        {
            List<string> tmp = [];
            var source = sourceType == "include" ? Clb_Include : Clb_Revinclude;
            if (takeAll)
            {
                for (int i = 0; i < source.Items.Count; i++)
                {
                    if (source.Items[i] != null)
                    {
                        tmp.Add($"_{sourceType}={source.Items[i]}");
                    }
                }
            }
            else
            {
                for (int i = 0; i < source.Items.Count; i++)
                {
                    if (source.GetItemChecked(i) && source.Items[i] != null)
                    {
                        tmp.Add($"_{sourceType}={source.Items[i]}");
                    }
                }
            }
            if (tmp.Count > 0)
            {
                return string.Join("&", tmp);
            }
            else
            {
                return null;
            }

        }
        private static string? CheckParameter(ListBox listBoxControl)
        {
            if (listBoxControl.Items.Count > 0)
            {
                List<string> tmp = [];
                for (int i = 0; i < listBoxControl.Items.Count; i++)
                {
                    if (listBoxControl.Items[i].ToString() is string oneQuery)
                    {
                        tmp.Add(oneQuery);
                    }
                }
                return string.Join("&", tmp);
            }
            return null;
        }
        #region Parameter Type
        private void SetupComboResource(string? scope)
        {
            List<string> scopeResource = [];
            var scopeList = scope?.Split(" ");
            if (scopeList != null)
            {
                foreach (var item in scopeList)
                {
                    int firstIndex = item.IndexOf("/");
                    int secondIndex = item.IndexOf(".");
                    scopeResource.Add(item.Substring(firstIndex + 1, secondIndex - firstIndex - 1));
                }
            }
            if (scopeResource.Count > 0)
            {
                Cob_Resource.Items.Clear();
                Cob_Resource.Items.AddRange(scopeResource.Distinct().ToArray());
            }
        }
        private void CleanStringPanel()
        {
            txtString.Text = "";
            grbString.Controls.OfType<RadioButton>().ToList().ForEach(x => x.Checked = false);
        }
        private void CleanUriPanel()
        {
            txtUri_url.Text = "";
            grbUri.Controls.OfType<RadioButton>().ToList().ForEach(x => x.Checked = false);
        }

        private void CleanTokenPanel()
        {
            txtToken_Code.Text = "";
            txtToken_SystemCode_Code.Text = "";
            txtToken_SystemCode_System.Text = "";
            grbToken.Controls.OfType<RadioButton>().ToList().ForEach(x => x.Checked = false);
        }

        private void CleanReferencePanel()
        {
            txtReference_id.Text = "";
            txtReference_version.Text = "";
            txtReference_TypeId_Type.Text = "";
            txtReference_TypeId_Id.Text = "";
            txtReference_url.Text = "";
            grbReference.Controls.OfType<RadioButton>().ToList().ForEach(x => x.Checked = false);
        }

        private void CleanQuantityPanel()
        {
            txtQuantity_number.Text = "";
            txtQuantity_nsc_code.Text = "";
            txtQuantity_nsc_number.Text = "";
            txtQuantity_nsc_system.Text = "";
            grbQuantity.Controls.OfType<RadioButton>().ToList().ForEach(x => x.Checked = false);
        }

        private void CleanNumberPanel()
        {
            txtNumber_value.Text = "";
            cobNumber_prefix.SelectedItem = null;
            grbNumber.Controls.OfType<RadioButton>().ToList().ForEach(x => x.Checked = false);
        }

        private void CleanDatePanel()
        {
            txtDate_value.Text = "";
            cobDate_prefix.SelectedItem = null;
            grbDate.Controls.OfType<RadioButton>().ToList().ForEach(x => x.Checked = false);
        }
        private string GetDateParameter()
        {
            StringBuilder sb = new();
            sb.Append(GetGroup(grbDate));

            if (cobDate_prefix.SelectedItem != null)
            {
                sb.Append($"{cobDate_prefix.SelectedItem.ToString()}");
            }
            if (!string.IsNullOrEmpty(txtDate_value.Text))
            {
                sb.Append($"{txtDate_value.Text}");
            }
            return sb.ToString();
        }

        private string GetNumberParameter()
        {
            StringBuilder sb = new();
            sb.Append(GetGroup(grbNumber));
            if (cobNumber_prefix.SelectedItem != null)
            {
                sb.Append($"{cobNumber_prefix.SelectedItem.ToString()}");
            }
            if (!string.IsNullOrEmpty(txtNumber_value.Text))
            {
                sb.Append($"{txtNumber_value.Text}");
            }
            return sb.ToString();
        }
        private string GetQuantityParameter()
        {
            StringBuilder sb = new();
            sb.Append(GetGroup(grbQuantity));
            if (cobQuantity_prefix.SelectedItem != null)
            {
                sb.Append($"{cobQuantity_prefix.SelectedItem.ToString()}");
            }
            if (!string.IsNullOrEmpty(txtQuantity_number.Text))
            {
                sb.Append($"{txtQuantity_number.Text}");
            }
            else if (!string.IsNullOrEmpty(txtQuantity_nsc_number.Text) && !string.IsNullOrEmpty(txtQuantity_nsc_system.Text) && !string.IsNullOrEmpty(txtQuantity_nsc_code.Text))
            {
                sb.Append($"{txtQuantity_nsc_number.Text}|{txtQuantity_nsc_system.Text}|{txtQuantity_nsc_code.Text}");
            }
            else
            {
                sb.Append($"{txtQuantity_nsc_number.Text}||{txtQuantity_nsc_code.Text}");
            }
            return sb.ToString();
        }
        private string GetReferenceParameter()
        {
            StringBuilder sb = new();
            sb.Append(GetGroup(grbReference));

            if (!string.IsNullOrEmpty(txtReference_id.Text))
            {
                sb.Append(txtReference_id.Text);
            }
            else if (!string.IsNullOrEmpty(txtReference_TypeId_Type.Text) && !string.IsNullOrEmpty(txtReference_TypeId_Id.Text))
            {
                sb.Append($"{txtReference_TypeId_Type.Text}/{txtReference_TypeId_Id.Text}");
                if(txtReference_version.Text != "")
                {
                    sb.Append($"/_history/{txtReference_version.Text}");
                }
            }
            else if (!string.IsNullOrEmpty(txtReference_url.Text))
            {
                sb.Append(txtReference_url.Text);
                if(txtReference_version.Text != "")
                {
                    sb.Append($"|{txtReference_version.Text}");
                }
            }
            else
            {
                sb.Append(string.Empty);
            }
            return sb.ToString();
        }
        private string GetStringParameter()
        {
            StringBuilder sb = new();
            sb.Append(GetGroup(grbString));
            sb.Append(txtString.Text);
            return sb.ToString();
        }
        private string GetTokenParameter()
        {
            StringBuilder sb = new();
            sb.Append(GetGroup(grbToken));
            if (txtToken_Code.Text != null)
            {
                sb.Append(txtToken_Code.Text);
            }
            else if (!string.IsNullOrEmpty(txtToken_SystemCode_Code.Text) && !string.IsNullOrEmpty(txtToken_SystemCode_System.Text))
            {
                sb.Append($"{txtToken_SystemCode_System.Text}|{txtToken_SystemCode_Code.Text}");
            }
            else if (!string.IsNullOrEmpty(txtToken_SystemCode_System.Text))
            {
                sb.Append($"{txtToken_SystemCode_System.Text}|");
            }
            else
            {
                sb.Append($"|{txtToken_SystemCode_Code.Text}");
            }
            return sb.ToString();
        }
        private string GetUriParameter()
        {
            StringBuilder sb = new();
            sb.Append(GetGroup(grbUri));
            if (!string.IsNullOrEmpty(txtUri_url.Text))
            {
                sb.Append(txtUri_url.Text);
            }
            return sb.ToString();
        }
        private static string GetGroup(GroupBox groupBox)
        {
            string result = "";
            var modifier = groupBox.Controls.OfType<RadioButton>().FirstOrDefault(x => x.Checked);
            if (modifier != null)
            {
                result = $":{modifier.Text}=";
            }
            else
            {
                result = "=";
            }
            return result;
        }
        #endregion



        #endregion


    }
}

