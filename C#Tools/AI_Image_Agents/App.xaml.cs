namespace AI_Image_Agents
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var appShell = new AppShell();

            var window = new Window(appShell)
            {
                Width = 600,    
                Height = 600     
            };

            return window;
        }
    }
}
