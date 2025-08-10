using AI_Image_Agents.Helper;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Shapes;

namespace AI_Image_Agents.Views;

public partial class LoginAgent : ContentPage
{
    private Dictionary<string, string> seleniumConf;
    private Dictionary<string, Dictionary<string, object>> agentsDict;

    private string loginURL;
    private string emailID;
    private string continueBtn;
    private string passwordID;
    private string loginBtn;

    private VerticalStackLayout agentLayout;
    private readonly List<Button> _agentBtnList = new();
    private readonly List<Button> _slowBtnList = new();
    private readonly List<Border> _agentLabelList = new();
    private readonly List<Image> _imageList = new();

    public LoginAgent()
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

        var parser = new JsonParser("Data/agents.json");

        seleniumConf = parser.GetSeleniumConfig();
        agentsDict = parser.GetAgents();

        SetSeleniumConf(seleniumConf);

        CreateAgents();
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

        double AgentBtnWidth = width * 0.35;
        double SlowBtnWidth = width * 0.3;
        double BtnHeight = height * 0.08;
        double fontSize = height * 0.035;
        double spacing = height * 0.06;


        foreach (var image in _imageList)
        {
            image.HeightRequest = BtnHeight;
            image.WidthRequest = BtnHeight;
        }

        foreach (var btn in _agentBtnList)
        {
            btn.HeightRequest = BtnHeight;
            btn.WidthRequest = AgentBtnWidth;
            btn.FontSize = fontSize;
        }

        foreach (var btn in _slowBtnList)
        {
            btn.HeightRequest = BtnHeight;
            btn.WidthRequest = SlowBtnWidth;
            btn.FontSize = fontSize;
        }

        foreach (var border in _agentLabelList)
        {
            if (border.Content is Label label)
            {
                label.HeightRequest = BtnHeight;
                label.WidthRequest = SlowBtnWidth;
                label.FontSize = fontSize;
            }
        }

    }

    private void SetSeleniumConf(Dictionary<string, string> seleniumConf)
    {
        loginURL = seleniumConf["loginURL"];
        emailID = seleniumConf["emailID"];
        continueBtn = seleniumConf["continueBtn"]; ;
        passwordID = seleniumConf["passwordID"]; ;
        loginBtn = seleniumConf["loginBtn"]; ;
    }

    private void CreateAgents()
    {

        agentLayout = new VerticalStackLayout
        {
            Style = (Style)Application.Current.Resources["vertLayout"],
        };

        foreach (var agent in agentsDict)
        {

            var horLayout = new HorizontalStackLayout
            {
                Style = (Style)Application.Current.Resources["horLayout"],
            };

            var image = new Image()
            {
                Source = "aiImage.png"
            };

            _imageList.Add(image);

            horLayout.Children.Add(image);

            var buttonAgent = new Button
            {
                Text = "Login Account " + agent.Key,
                CommandParameter = agent.Key,
                Style = (Style)Application.Current.Resources["loginAgentBtn"],
            };

            buttonAgent.Clicked += OnAgentButtonClicked;

            horLayout.Children.Add(buttonAgent);

            _agentBtnList.Add(buttonAgent);

            if (agent.Value.TryGetValue("slowAgent", out var slowAgentObj)
                    && slowAgentObj is bool slowAgent && !slowAgent)
            {
                var slowAgentBtn = new Button
                {
                    Text = "activate slow",
                    CommandParameter = agent.Key,
                    Style = (Style)Application.Current.Resources["loginSlowBtn"],
                };

                slowAgentBtn.Clicked += OnSlowAgentBtnClicked;

                horLayout.Children.Add(slowAgentBtn);
                _slowBtnList.Add(slowAgentBtn);
            }
            else
            {

                var border = new Border()
                {
                    StrokeShape = new RoundRectangle
                    {
                        CornerRadius = (CornerRadius)Application.Current.Resources["cornerRadiusLabels"]
                    },
                    Style = (Style)Application.Current.Resources["borderStyle"],
                    Content = new Label
                    {
                        Text = "Agent is slow",
                        Style = (Style)Application.Current.Resources["loginSlowLabel"],
                    }

                 };

                horLayout.Children.Add(border);
                _agentLabelList.Add(border); 
            }

            agentLayout.Children.Add(horLayout);
        }

        Content = new ScrollView { Content = agentLayout };
    }

    private void OnAgentButtonClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is string agentKey)
        {
            if (agentsDict.TryGetValue(agentKey, out var agentID))
            {
                agentID.TryGetValue("email", out object emailObj);
                agentID.TryGetValue("password", out object passwordObj);

                if (emailObj == null || passwordObj == null)
                {
                    DisplayAlert(
                    "Error Message",
                    "Dieser Agent hat keine Login Daten",
                    "OK"
                     );
                }
                else
                {
                    string email = emailObj.ToString();
                    string password = passwordObj.ToString();

                    var SeleniumDriver = new LoginAccount(email, password);

                    SeleniumDriver.Login(loginURL, emailID, continueBtn, passwordID, loginBtn);

                }
            }
        }
    }

    private void OnSlowAgentBtnClicked(object sender, EventArgs e)
    {
        if (sender is Button slowBtn && slowBtn.Parent is Layout parentLayout)
        {
            string agentID = slowBtn.CommandParameter as string;

            int index = parentLayout.Children.IndexOf(slowBtn);

            parentLayout.Children.Remove(slowBtn);

            var border = new Border
            {
                StrokeShape = new RoundRectangle
                {
                    CornerRadius = (CornerRadius)Application.Current.Resources["cornerRadiusLabels"]
                },
                Style = (Style)Application.Current.Resources["borderStyle"],
                Content = new Label
                {
                    Text = "Agent is slow",
                    Style = (Style)Application.Current.Resources["loginSlowLabel"]
                }
            };

            parentLayout.Children.Insert(index, border);
            _agentLabelList.Add(border);

            var window = this.GetParentWindow();
            if (window != null)
            {
                UpdateLayoutSize(window.Width, window.Height);
            }

            var parser = new JsonParser("Data/agents.json");

            parser.SetAgentSlow(agentID);
        }
    }
}