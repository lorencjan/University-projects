using DotVVM.Framework.Binding;
using DotVVM.Framework.Binding.Expressions;
using DotVVM.Framework.Controls;

namespace RockFests.Controls
{
    public class DataSetUtils : DotvvmMarkupControl
    {
        [MarkupOptions(Required = true)]
        public string Text
        {
            get { return (string) GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public static readonly DotvvmProperty TextProperty
            = DotvvmProperty.Register<string, DataSetUtils>(c => c.Text);

        [MarkupOptions(Required = true)]
        public Command Refresh
        {
            get { return (Command)GetValue(RefreshProperty); }
            set { SetValue(RefreshProperty, value); }
        }
        public static readonly DotvvmProperty RefreshProperty
            = DotvvmProperty.Register<Command, DataSetUtils>(c => c.Refresh);

        [MarkupOptions(Required = true)]
        public Command Search
        {
            get { return (Command) GetValue(SearchProperty); }
            set { SetValue(SearchProperty, value); }
        }
        public static readonly DotvvmProperty SearchProperty
            = DotvvmProperty.Register<Command, DataSetUtils>(c => c.Search);
    }
}
