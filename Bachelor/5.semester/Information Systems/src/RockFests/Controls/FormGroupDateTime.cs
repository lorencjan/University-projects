using System;
using DotVVM.Framework.Binding;
using DotVVM.Framework.Controls;

namespace RockFests.Controls
{
    public class FormGroupDateTime : FormGroup
    {
        [MarkupOptions(Required = true)]
        public DateTime Value
        {
            get { return (DateTime) GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        public static readonly DotvvmProperty ValueProperty
            = DotvvmProperty.Register<DateTime, FormGroupDateTime>(c => c.Value);
    }
}
