using DotVVM.Framework.Binding;
using DotVVM.Framework.Controls;

namespace RockFests.Controls
{
    public abstract class FormGroup : DotvvmMarkupControl
    {
        [MarkupOptions(Required = true)]
        public string Label
        {
            get { return (string) GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }
        public static readonly DotvvmProperty LabelProperty
            = DotvvmProperty.Register<string, FormGroup>(c => c.Label);

        public bool Validate
        {
            get { return (bool) GetValue(ValidateProperty); }
            set { SetValue(ValidateProperty, value); }
        }
        public static readonly DotvvmProperty ValidateProperty
            = DotvvmProperty.Register<bool, FormGroup>(c => c.Validate, true);
        
        public bool Required
        {
            get { return (bool)GetValue(RequiredProperty); }
            set { SetValue(RequiredProperty, value); }
        }
        public static readonly DotvvmProperty RequiredProperty
            = DotvvmProperty.Register<bool, FormGroup>(c => c.Required, true);

        public string Width
        {
            get { return (string)GetValue(WidthProperty); }
            set { SetValue(WidthProperty, value); }
        }
        public static readonly DotvvmProperty WidthProperty
            = DotvvmProperty.Register<string, FormGroup>(c => c.Width, "100");

        public string InputWidth
        {
            get { return (string)GetValue(InputWidthProperty); }
            set { SetValue(InputWidthProperty, value); }
        }
        public static readonly DotvvmProperty InputWidthProperty
            = DotvvmProperty.Register<string, FormGroup>(c => c.InputWidth, "50");
    }
}
