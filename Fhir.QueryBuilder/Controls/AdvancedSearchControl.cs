using Fhir.QueryBuilder.AdvancedSearch;
using Fhir.QueryBuilder.QueryBuilders.FluentApi;
using Microsoft.Extensions.Logging;

namespace Fhir.QueryBuilder.Controls
{
    /// <summary>
    /// 進階搜尋控制項
    /// </summary>
    public partial class AdvancedSearchControl : UserControl
    {
        private readonly ILogger<AdvancedSearchControl>? _logger;
        private IFhirQueryBuilder? _queryBuilder;

        // 儲存進階參數
        private readonly List<ChainParameter> _chainParameters = new();
        private readonly List<CompositeParameter> _compositeParameters = new();
        private readonly List<string> _filterExpressions = new();
        private ResultControlParameters _resultControl = new();

        public event EventHandler<AdvancedParametersChangedEventArgs>? ParametersChanged;

        public AdvancedSearchControl()
        {
            InitializeComponent();
            InitializeControls();
        }

        public AdvancedSearchControl(ILogger<AdvancedSearchControl> logger) : this()
        {
            _logger = logger;
        }

        private void InitializeControls()
        {
            // 設定預設值
            cboTotal.SelectedIndex = 2; // "accurate"
            cboSummary.SelectedIndex = 1; // "false"
            
            // 設定工具提示
            SetupTooltips();
        }

        private void SetupTooltips()
        {
            var toolTip = new ToolTip();
            toolTip.SetToolTip(txtChainPath, "例如: patient.name, patient.organization.name");
            toolTip.SetToolTip(txtChainValue, "搜尋值，例如: John, General Hospital");
            toolTip.SetToolTip(txtCompositeParam, "複合參數名稱，例如: component-code-value-quantity");
            toolTip.SetToolTip(txtCompositeComponents, "用逗號分隔的組件，例如: http://loinc.org|8480-6,120,mmHg");
            toolTip.SetToolTip(txtFilterExpression, "FHIR Filter 表達式，例如: name co 'John' and birthDate ge 1990-01-01");
            toolTip.SetToolTip(txtElements, "要包含的元素，用逗號分隔，例如: id,name,birthDate");
        }

        public void SetQueryBuilder(IFhirQueryBuilder queryBuilder)
        {
            _queryBuilder = queryBuilder;
        }

        public void ApplyToQueryBuilder(IFhirQueryBuilder builder)
        {
            try
            {
                AdvancedSearchApplicator.Apply(builder, _resultControl, _chainParameters, _compositeParameters, _filterExpressions);

                _logger?.LogDebug("Applied {ChainCount} chain, {CompositeCount} composite, {FilterCount} filter parameters",
                    _chainParameters.Count, _compositeParameters.Count, _filterExpressions.Count);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error applying advanced parameters to query builder");
                MessageBox.Show($"Error applying advanced parameters: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAddChain_Click(object sender, EventArgs e)
        {
            try
            {
                var path = txtChainPath.Text.Trim();
                var value = txtChainValue.Text.Trim();

                if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(value))
                {
                    MessageBox.Show("Please enter both path and value for chaining parameter.", "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var chainParam = new ChainParameter { Path = path, Value = value };
                _chainParameters.Add(chainParam);
                
                lstChains.Items.Add($"{path} = {value}");
                
                // 清空輸入框
                txtChainPath.Clear();
                txtChainValue.Clear();
                
                OnParametersChanged();
                _logger?.LogDebug("Added chain parameter: {Path} = {Value}", path, value);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error adding chain parameter");
                MessageBox.Show($"Error adding chain parameter: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAddComposite_Click(object sender, EventArgs e)
        {
            try
            {
                var paramName = txtCompositeParam.Text.Trim();
                var componentsText = txtCompositeComponents.Text.Trim();

                if (string.IsNullOrEmpty(paramName) || string.IsNullOrEmpty(componentsText))
                {
                    MessageBox.Show("Please enter both parameter name and components.", "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var components = componentsText.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(c => c.Trim()).ToArray();

                if (components.Length < 2)
                {
                    MessageBox.Show("Composite parameter must have at least 2 components.", "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var compositeParam = new CompositeParameter 
                { 
                    ParameterName = paramName, 
                    Components = components 
                };
                _compositeParameters.Add(compositeParam);
                
                lstComposite.Items.Add($"{paramName} = {string.Join(" $ ", components)}");
                
                // 清空輸入框
                txtCompositeParam.Clear();
                txtCompositeComponents.Clear();
                
                OnParametersChanged();
                _logger?.LogDebug("Added composite parameter: {ParameterName} with {ComponentCount} components", 
                    paramName, components.Length);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error adding composite parameter");
                MessageBox.Show($"Error adding composite parameter: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAddFilter_Click(object sender, EventArgs e)
        {
            try
            {
                var expression = txtFilterExpression.Text.Trim();

                if (string.IsNullOrEmpty(expression))
                {
                    MessageBox.Show("Please enter a filter expression.", "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _filterExpressions.Add(expression);
                lstFilters.Items.Add(expression);
                
                // 清空輸入框
                txtFilterExpression.Clear();
                
                OnParametersChanged();
                _logger?.LogDebug("Added filter expression: {Expression}", expression);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error adding filter expression");
                MessageBox.Show($"Error adding filter expression: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClearAll_Click(object sender, EventArgs e)
        {
            try
            {
                _chainParameters.Clear();
                _compositeParameters.Clear();
                _filterExpressions.Clear();
                _resultControl = new ResultControlParameters();

                lstChains.Items.Clear();
                lstComposite.Items.Clear();
                lstFilters.Items.Clear();

                txtCount.Clear();
                txtOffset.Clear();
                txtElements.Clear();
                cboTotal.SelectedIndex = 2; // "accurate"
                cboSummary.SelectedIndex = 1; // "false"

                OnParametersChanged();
                _logger?.LogDebug("Cleared all advanced parameters");
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error clearing advanced parameters");
                MessageBox.Show($"Error clearing parameters: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void lstChains_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && lstChains.SelectedIndex >= 0)
            {
                var index = lstChains.SelectedIndex;
                _chainParameters.RemoveAt(index);
                lstChains.Items.RemoveAt(index);
                OnParametersChanged();
            }
        }

        private void lstComposite_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && lstComposite.SelectedIndex >= 0)
            {
                var index = lstComposite.SelectedIndex;
                _compositeParameters.RemoveAt(index);
                lstComposite.Items.RemoveAt(index);
                OnParametersChanged();
            }
        }

        private void lstFilters_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && lstFilters.SelectedIndex >= 0)
            {
                var index = lstFilters.SelectedIndex;
                _filterExpressions.RemoveAt(index);
                lstFilters.Items.RemoveAt(index);
                OnParametersChanged();
            }
        }

        private void OnParametersChanged()
        {
            // 更新結果控制參數
            UpdateResultControlParameters();
            
            // 觸發事件
            ParametersChanged?.Invoke(this, new AdvancedParametersChangedEventArgs
            {
                ChainParameters = _chainParameters.ToList(),
                CompositeParameters = _compositeParameters.ToList(),
                FilterExpressions = _filterExpressions.ToList(),
                ResultControl = _resultControl
            });
        }

        private void UpdateResultControlParameters()
        {
            // Count
            if (int.TryParse(txtCount.Text, out var count) && count > 0)
                _resultControl.Count = count;
            else
                _resultControl.Count = null;

            // Offset
            if (int.TryParse(txtOffset.Text, out var offset) && offset >= 0)
                _resultControl.Offset = offset;
            else
                _resultControl.Offset = null;

            // Total
            _resultControl.Total = cboTotal.SelectedItem?.ToString();

            // Summary
            _resultControl.Summary = cboSummary.SelectedItem?.ToString();

            // Elements
            if (!string.IsNullOrWhiteSpace(txtElements.Text))
            {
                _resultControl.Elements = txtElements.Text
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(e => e.Trim())
                    .ToArray();
            }
            else
            {
                _resultControl.Elements = null;
            }
        }

        public bool HasAdvancedParameters()
        {
            return _chainParameters.Count > 0 || 
                   _compositeParameters.Count > 0 || 
                   _filterExpressions.Count > 0 ||
                   _resultControl.Count.HasValue ||
                   _resultControl.Offset.HasValue ||
                   !string.IsNullOrEmpty(_resultControl.Total) ||
                   !string.IsNullOrEmpty(_resultControl.Summary) ||
                   _resultControl.Elements?.Length > 0;
        }

        public void Reset()
        {
            btnClearAll_Click(this, EventArgs.Empty);
        }
    }
}
