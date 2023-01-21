using DotVVM.Framework.Binding;
using DotVVM.Framework.Controls;

namespace RockFests.Controls
{
    public class FormGroupText : FormGroup
    {
        [MarkupOptions(Required = true)]
        public string Value
        {
            get { return (string) GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        public static readonly DotvvmProperty ValueProperty
            = DotvvmProperty.Register<string, FormGroupText>(c => c.Value);
    }
}
