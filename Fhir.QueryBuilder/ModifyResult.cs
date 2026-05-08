using System.Data;

namespace Fhir.QueryBuilder
{
    public partial class ModifyResult : Form
    {
        readonly List<string> _modifyingQuery = [];

        public ModifyResult(string? resourceName)
        {
            InitializeComponent();
            _modifyingQuery.Clear();
            var elements = GetResourceElement(resourceName);
            if (elements != null)
            {
                cob_ResourceElements.Items.AddRange(elements);
            }


        }

        public IEnumerable<string> GetModifingQueryList() => _modifyingQuery;

        #region Event Handlers
        private void Btn_Cancel_Click(object sender, EventArgs e)
        {
            rab_Contained_Both.Checked = false;
            rab_Contained_True.Checked = false;
            rab_Contained_False.Checked = false;
            rab_ContainedType_container.Checked = false;
            rab_ContainedType_contained.Checked = false;
            txt_Count_value.Text = "";
            txt_Maxresults_value.Text = "";
            rab_summary_false.Checked = false;
            rab_Summary_true.Checked = false;
            rab_summary_text.Checked = false;
            rab_Summary_data.Checked = false;
            rab_Summary_count.Checked = false;

        }

        private void Btn_AddToSort_Click(object sender, EventArgs e)
        {
            string result = rab_PrefixForSort.Checked ? $"-{cob_ResourceElements.SelectedItem?.ToString()}" : $"{cob_ResourceElements.SelectedItem?.ToString()}";
            lib_Sort.Items.Add(result);
        }

        private void Btn_AddToElements_Click(object sender, EventArgs e)
        {
            string reuslt = cob_ResourceElements.SelectedItem?.ToString() ?? "";
            lib_Elements.Items.Add(reuslt);
        }
        private void Btn_OK_Click(object sender, EventArgs e)
        {
            _modifyingQuery.Clear();
            var contained = grb_Contained.Controls.OfType<RadioButton>().FirstOrDefault(x => x.Checked);
            if (contained != null)
            {
                _modifyingQuery.Add($"_contained={contained.Text}");
            }
            var containedType = grb_ContainedType.Controls.OfType<RadioButton>().FirstOrDefault(x => x.Checked);
            if (containedType != null)
            {
                _modifyingQuery.Add($"_containedType={containedType.Text}");
            }
            if (txt_Count_value.Text != "")
            {
                _modifyingQuery.Add($"_count={txt_Count_value.Text}");
            }
            if (txt_Maxresults_value.Text != "")
            {
                _modifyingQuery.Add($"_maxresults={txt_Maxresults_value.Text}");
            }
            var summary = grb_Summary.Controls.OfType<RadioButton>().FirstOrDefault(x => x.Checked);
            if (summary != null)
            {
                _modifyingQuery.Add($"_summary={summary.Text}");
            }
            if (lib_Elements.Items.Count > 0)
            {
                _modifyingQuery.Add($"_elements={string.Join(",", lib_Elements.Items.Cast<string>())}");
            }
            if (lib_Sort.Items.Count > 0)
            {
                _modifyingQuery.Add($"_sort={string.Join(",", lib_Sort.Items.Cast<string>())}");
            }
            this.Visible = false;
        }
        private void Btn_ClearSort_Click(object sender, EventArgs e)
        {
            lib_Sort.Items.Clear();
        }
        private void Btn_ClearElement_Click(object sender, EventArgs e)
        {
            lib_Elements.Items.Clear();
        }
        #endregion
        #region  Pirvate Methods
        private static string[]? GetResourceElement(string? resourceName)
        {
            if (string.IsNullOrEmpty(resourceName))
                return null;
            // 無外部 ResourceFactory 時：常用於 _elements／排序提示的精簡後援清單
            return resourceName.Equals("Patient", StringComparison.OrdinalIgnoreCase)
                ? ["id", "identifier", "name", "birthDate", "gender", "address", "telecom"]
                : ["id", "meta"];
        }
        #endregion




    }
}
