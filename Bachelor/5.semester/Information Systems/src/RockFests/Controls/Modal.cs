using DotVVM.Framework.Binding;
using DotVVM.Framework.Binding.Expressions;
using DotVVM.Framework.Controls;

namespace RockFests.Controls
{
    public class Modal : DotvvmMarkupControl
    {
        [MarkupOptions(Required = true)]
        public string Header
        {
            get { return (string) GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }
        public static readonly DotvvmProperty HeaderProperty
            = DotvvmProperty.Register<string, Modal>(c => c.Header);

        [MarkupOptions(Required = true)]
        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }
        public static readonly DotvvmProperty MessageProperty
            = DotvvmProperty.Register<string, Modal>(c => c.Message);

        [MarkupOptions(Required = true)]
        public bool IsShowed
        {
            get { return (bool)GetValue(IsShowedProperty); }
            set { SetValue(IsShowedProperty, value); }
        }
        public static readonly DotvvmProperty IsShowedProperty
            = DotvvmProperty.Register<bool, Modal>(c => c.IsShowed);

        public bool HasConfirmation
        {
            get { return (bool)GetValue(HasConfirmationProperty); }
            set { SetValue(HasConfirmationProperty, value); }
        }
        public static readonly DotvvmProperty HasConfirmationProperty
            = DotvvmProperty.Register<bool, Modal>(c => c.HasConfirmation);

        public Command ConfirmCommand
        {
            get { return (Command)GetValue(ConfirmCommandProperty); }
            set { SetValue(ConfirmCommandProperty, value); }
        }
        public static readonly DotvvmProperty ConfirmCommandProperty
            = DotvvmProperty.Register<Command, Modal>(c => c.ConfirmCommand);
    }
}
