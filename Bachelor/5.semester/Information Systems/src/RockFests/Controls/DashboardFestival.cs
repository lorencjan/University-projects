using DotVVM.Framework.Binding;
using DotVVM.Framework.Controls;
using RockFests.BL.Model;

namespace RockFests.Controls
{
    public class DashboardFestival : DotvvmMarkupControl
    {
        [MarkupOptions(Required = true)]
        public FestivalDto Festival
        {
            get { return (FestivalDto) GetValue(FestivalProperty); }
            set { SetValue(FestivalProperty, value); }
        }
        public static readonly DotvvmProperty FestivalProperty
            = DotvvmProperty.Register<FestivalDto, DashboardFestival>(c => c.Festival);

        [MarkupOptions(Required = true)]
        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }
        public static readonly DotvvmProperty HeaderProperty
            = DotvvmProperty.Register<string, DashboardFestival>(c => c.Header);

        public string EmptyText
        {
            get { return (string)GetValue(EmptyTextProperty); }
            set { SetValue(EmptyTextProperty, value); }
        }
        public static readonly DotvvmProperty EmptyTextProperty
            = DotvvmProperty.Register<string, DashboardFestival>(c => c.EmptyText);
    }
}
