using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Logik.Pw.Logik.Items
{
    public class PwEintrag : INotifyPropertyChanged, IDataErrorInfo
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Programmname darf nicht leer sein")]
        [MinLength(3, ErrorMessage = "Min 3 Stellen")]
        public string Programm { get; set; }

        public string Benutzer { get; set; }

        public string Passwort { get; set; }

        public DateTime Datum { get; set; }

        public string tmprndIndex { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;


        #region IDataErrorInfo Management
        public bool HasErrors => Errors.Any();
        private Dictionary<string, string> Errors { get; } = new Dictionary<string, string>();
        public string Error => string.Empty;
        public string this[string PropertyName]
        {
            get
            {
                CollectErrors();

                return Errors.ContainsKey(PropertyName) ? Errors[PropertyName] : string.Empty;
            }
        }
        private static List<PropertyInfo> _propertyInfos;
        protected List<PropertyInfo> PropertyInfos
        {
            get
            {
                if (_propertyInfos == null)
                {
                    Trace.TraceInformation("Sammel type Infos");
                    _propertyInfos =
                        GetType()
                        .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .Where(prop => prop.IsDefined(typeof(RequiredAttribute), true) || prop.IsDefined(typeof(MinLengthAttribute), true)) // DAtaAnnotation
                        .ToList();
                }
                return _propertyInfos;
            }
        }

        private void CollectErrors()
        {
            Errors.Clear();
            foreach (PropertyInfo prop in PropertyInfos)
            {
                var currentValue = prop.GetValue(this);
                var requiredAttr = prop.GetCustomAttribute<RequiredAttribute>();
                var minLenAttr = prop.GetCustomAttribute<MinLengthAttribute>();
                if (requiredAttr != null)
                {
                    if (string.IsNullOrEmpty(currentValue?.ToString() ?? string.Empty))
                    {
                        Errors.Add(prop.Name, requiredAttr.ErrorMessage);
                    }
                }
                if (minLenAttr != null)
                {
                    if ((currentValue?.ToString() ?? string.Empty).Length < minLenAttr.Length)  // ?? wenn das links von ?? nicht null ist dann dann links sonst rechts.
                    {
                        if (Errors.ContainsKey(prop.Name)) continue;
                        Errors.Add(prop.Name, minLenAttr.ErrorMessage);
                    }
                }
            }
        }
        #endregion
    }
}
