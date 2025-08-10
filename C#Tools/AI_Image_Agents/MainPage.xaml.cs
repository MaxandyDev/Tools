using AI_Image_Agents.Views;

namespace AI_Image_Agents
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();

            Loaded += (s, e) =>
            {
                var window = this.GetParentWindow();
                if (window != null)
                {
                    window.SizeChanged += OnSizeChanged;

                    UpdateLayoutSize(window.Width, window.Height);
                }
            };
        }

        private async void Btn1Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new LoginAgent());
        }

        private async void Btn2Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new PromptManager());
        }

        private void Btn3Clicked(object sender, EventArgs e)
        {
            #if ANDROID
                            Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
            #elif WINDOWS
                            System.Diagnostics.Process.GetCurrentProcess().CloseMainWindow();
            #elif MACCATALYST
                            System.Diagnostics.Process.GetCurrentProcess().CloseMainWindow();
            #elif IOS
            #else
                            System.Diagnostics.Process.GetCurrentProcess().Kill();
            #endif
        }

        private void OnSizeChanged(object sender, EventArgs e)
        { 
            if (sender is Window window)
            {
                UpdateLayoutSize(window.Width, window.Height);
            }
        }

        private void UpdateLayoutSize(double width, double height)
        {

            double BtnWidth = width * 0.3;
            double BtnHeight = height * 0.1;
            double fontSize = width * 0.032;
            double spacing = height * 0.06;

            Btn1.WidthRequest = BtnWidth;
            Btn1.HeightRequest = BtnHeight;
            Btn1.FontSize = fontSize;

            Btn2.WidthRequest = BtnWidth;
            Btn2.HeightRequest = BtnHeight;
            Btn2.FontSize = fontSize;

            Btn3.WidthRequest = BtnWidth;
            Btn3.HeightRequest = BtnHeight;
            Btn3.FontSize = fontSize;

            RootLayout.Spacing = spacing;
        }
    }
}