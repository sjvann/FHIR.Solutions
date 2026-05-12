using FhirResourceCreator.Fhir;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;

namespace FhirResourceCreator.Models
{

    public class ResourceModel
    {
        const string RescourceFolderSub = "Sub";

        private readonly string? _RootNamespace;
        private string? _ResourceName;
        private readonly string? _FilePath;
        private readonly string? _SaveTo;
        private List<ElementRecord>? _Elements;

        [SupportedOSPlatform("windows")]
        public ResourceModel(string filePath, string saveTo, string? rootNamespace)
        {

            _FilePath = filePath;
            LoadElement();
            _SaveTo = $"{saveTo}\\{_ResourceName}{RescourceFolderSub}";
            _RootNamespace = rootNamespace;

        }

        /// <summary>
        /// Builds a resource model from pre-parsed elements (e.g. StructureDefinition.snapshot).
        /// </summary>
        public ResourceModel(string resourceName, string saveTo, string? rootNamespace, IEnumerable<ElementRecord> elements)
        {
            _ResourceName = resourceName;
            _Elements = elements.ToList();
            _SaveTo = $"{saveTo}\\{resourceName}{RescourceFolderSub}";
            _RootNamespace = rootNamespace;
            _FilePath = null;
        }

        #region Public Method
        public string? SaveTo => _SaveTo;
        public string? ResourceName => _ResourceName;

        public IReadOnlyList<ElementRecord>? Elements => _Elements; 
        public ElementRecord? GetElement(string path)
        {
            return _Elements?.FirstOrDefault(x => x.ThisPath == path);
        }
        public OneClassContent GetResourceContent()
        {
            StringBuilder sbp = new();
            StringBuilder sbc = new();
            StringBuilder sbs = new();
            var targets = _Elements?.Where(x => x.ParentPath == _ResourceName).Where(x=>!x.IsSkip);
            if(targets != null && targets.Any())
            {
                foreach (var item in targets)
                {
                    sbp.AppendLine(item.GetProperty());
                    sbc.AppendLine(item.GetConstructor());
                    sbs.AppendLine(item.GetSetup());
                }
            }

            return new OneClassContent()
            {
                PropertyString = sbp.ToString(),
                ConstructorString = sbc.ToString(),
                SetupString = sbs.ToString()
            };
        }

        public OneClassContent GetBackboneContent(string? parentPath)
        {
            StringBuilder sbp = new();
            StringBuilder sbc = new();
            StringBuilder sbs = new();
            string backboneNamespace = $"{_RootNamespace}.{parentPath}{RescourceFolderSub}";
            var target = _Elements?.Where(x => x.ParentPath is string itemParentPath && itemParentPath == parentPath).Where(x=>!x.IsSkip);
            if(target != null && target.Any())
            {
                foreach( var item in target)
                {
                    switch (item.KeywordType)
                    {
                        case KeywordCheckerType.ForBackbone:
                            sbp.AppendLine(item.GetProperty(backboneNamespace));
                            sbc.AppendLine(item.GetConstructor(backboneNamespace));
                            sbs.AppendLine(item.GetSetup(backboneNamespace));
                            break;
                        case KeywordCheckerType.ForComplex:
                            sbp.AppendLine(item.GetProperty(KeywordChecker.ComplexDataTypeNamespace));
                            sbc.AppendLine(item.GetConstructor(KeywordChecker.ComplexDataTypeNamespace));
                            sbs.AppendLine(item.GetSetup(KeywordChecker.ComplexDataTypeNamespace));
                            break;
                        default:
                            sbp.AppendLine(item.GetProperty());
                            sbc.AppendLine(item.GetConstructor());
                            sbs.AppendLine(item.GetSetup());
                            break;
                    }
                }
            }
            return new OneClassContent()
            {
                PropertyString = sbp.ToString(),
                ConstructorString = sbc.ToString(),
                SetupString = sbs.ToString()
            };
        }

        #endregion
        #region Private Method
        [SupportedOSPlatform("windows")]
        private void LoadElement()
        {
            List<ElementRecord> elementSet = new();
            string con = @"Provider=Microsoft.ACE.OLEDB.12.0;;Data Source=" + _FilePath + ";Extended Properties='Excel 12.0 Xml;HDR=YES;IMEX = 1;'";

            using (OleDbConnection connect = new(con))
            {
                connect.Open();
                OleDbCommand cmd = new("select * from [Elements$]", connect);
                using OleDbDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    if (dr[1] is string dt1 && dr[5] is string dt2 && dr[6] is string dt3)
                    {
                        if (dr[10] is string dt4)
                        {
                            ElementRecord elementRecord = new(dt1, dt2, dt3, dt4.Replace("\n", ""));
                            elementSet.Add(elementRecord);
                        }
                        else
                        {
                            _ResourceName = dt1;
                        }
                    }
                }
            }
            _Elements = elementSet;
        }
        #endregion
    }
}

