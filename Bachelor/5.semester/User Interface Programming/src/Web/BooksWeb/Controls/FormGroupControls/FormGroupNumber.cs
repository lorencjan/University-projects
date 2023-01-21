using DotVVM.Framework.Binding;
using DotVVM.Framework.Controls;

namespace BooksWeb.Controls.FormGroupControls
{
    public class FormGroupNumber : FormGroup
    {
        [MarkupOptions(Required = true)]
        public int Value
        {
            get { return (int) GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        public static readonly DotvvmProperty ValueProperty
            = DotvvmProperty.Register<int, FormGroupNumber>(c => c.Value);
    }
}
