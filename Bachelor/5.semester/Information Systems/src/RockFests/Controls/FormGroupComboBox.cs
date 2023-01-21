using System.Collections.Generic;
using DotVVM.Framework.Binding;
using DotVVM.Framework.Controls;
using RockFests.DAL.Types;

namespace RockFests.Controls
{
    public class FormGroupComboBox : FormGroup
    {
        public object ValidationValue
        {
            get { return (object) GetValue(ValidationValueProperty); }
            set { SetValue(ValidationValueProperty, value); }
        }
        public static readonly DotvvmProperty ValidationValueProperty
            = DotvvmProperty.Register<object, FormGroupComboBox>(c => c.ValidationValue);

        [MarkupOptions(Required = true)]
        public int Selected
        {
            get { return (int)GetValue(SelectedProperty); }
            set { SetValue(SelectedProperty, value); }
        }
        public static readonly DotvvmProperty SelectedProperty
            = DotvvmProperty.Register<int, FormGroupComboBox>(c => c.Selected);

        [MarkupOptions(Required = true)]
        public List<KeyValue<int, string>> DataSource
        {
            get { return (List<KeyValue<int, string>>)GetValue(DataSourceProperty); }
            set { SetValue(DataSourceProperty, value); }
        }
        public static readonly DotvvmProperty DataSourceProperty
            = DotvvmProperty.Register<List<KeyValue<int, string>>, FormGroupComboBox>(c => c.DataSource);
    }
}
