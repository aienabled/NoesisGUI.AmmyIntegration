namespace NoesisAmmyPlatform
{
    using Ammy.Platforms;

    internal class NoesisTypeNames : PlatformTypeNames
    {
        public static readonly NoesisTypeNames Instance = new NoesisTypeNames();

        public override string Binding => "Noesis.Binding";

        public override string BindingBase => "Noesis.BindingBase";

        //public override string DashStyles => "Noesis.DashStyles";

        public override string DependencyObject => "Noesis.DependencyObject";

        public override string DependencyProperty => "Noesis.DependencyProperty";

        public override string FrameworkElement => "Noesis.FrameworkElement";

        //public override string ICommand => "Noesis.Input.ICommand";

        public override string ResourceDictionary => "Noesis.ResourceDictionary";

        public override string RoutedEvent => "Noesis.RoutedEvent";

        public override string SetterBase => "Noesis.SetterBase";

        public override string Style => "Noesis.Style";

        //public override string TextDecorations => "Noesis.TextDecorations";

        public override string Thickness => "Noesis.Thickness";

        public override string UIElement => "Noesis.UIElement";
    }
}