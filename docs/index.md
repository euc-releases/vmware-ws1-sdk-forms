# Omnissa Workspace ONE SDK for Xamarin.Forms

This documentation will cover the [installation](#installation), [setup](#setup) and [usage](#usage) of the Omnissa Workspace ONE SDK for Xamarin Forms.

Workspace ONE SDK for Xamarin Forms is dependent on [AWSDK](https://www.nuget.org/packages/AWSDK/) version 22.5.0 or higher. For detailed information about the Workspace ONE SDK and managing internal apps, See the [Omnissa Developer Portal SDKs](https://developer.omnissa.com/sdks/) page and navigate to the appropriate area.

## Installation

The SDK should be installed using **Nuget** package manager.

**WorkspaceOne.Forms** is the package to be added in the Xamarin Forms app to enable it with Workspace One SDK funtionalities.The package provides interfaces to initialize / setup and enable the WS1 SDK security features.

Add this package to your Xamarin.Forms project and to your iOS and Android project of the Xamarin.Forms app as well.

Add the appropriate packages to your solution for each app project. Then continue to the [setup](#setup) step for [Android](#android) and [iOS](#ios).

## Setup

Before using the Workspace ONE SDK, just like many other Xamarin Forms packages it's dependencies need to be initialized first. In addition to adding the Forms package, each target platform needs to add the SDKs package for that specific platform as well. If you target only one of the two supported platforms, skip all steps for the one you don't support. 

For Android platform along with Workspace SDK Forms package, add the below packages if not already present:

1. Xamarin.AndroidX.Legacy.Support.V4 (v1.0.0.2)
2. Xamarin.AndroidX.Browser (v1.2.0.2)
3. Xamarin.GooglePlayServices.Base (v71.1610.x)
4. Xamarin.GooglePlayServices.Safetynet (v71.1600.x)

### iOS

#### Procedure

1. In the `AppDelegate.cs`'s `FinishedLaunching` (just where most other packages get initialized as well) add the following code `WorkspaceOne.iOS.WorkspaceOne.Init("wsoexample");`. 

```c
    public override bool FinishedLaunching(UIApplication app, NSDictionary options)
    {
        global::Xamarin.Forms.Forms.Init();

        WorkspaceOne.iOS.WorkspaceOne.Init("wsoexample");//Replace "wsoexample" with the name of your iOS app url scheme

        //APNS Registeration
        if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
        {
            UNUserNotificationCenter.Current.RequestAuthorization(UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound,
                                                                               (granted, error) =>
                            InvokeOnMainThread(UIApplication.SharedApplication.RegisterForRemoteNotifications));
        }
        else if (UIDevice.CurrentDevice.CheckSystemVersion (8, 0)) 
        {
            var pushSettings = UIUserNotificationSettings.GetSettingsForTypes(
                        UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound,
                        new NSSet());

            UIApplication.SharedApplication.RegisterUserNotificationSettings(pushSettings);
            UIApplication.SharedApplication.RegisterForRemoteNotifications();
        } 
        else
        {
            UIRemoteNotificationType notificationTypes = UIRemoteNotificationType.Alert | UIRemoteNotificationType.Badge | UIRemoteNotificationType.Sound;
            UIApplication.SharedApplication.RegisterForRemoteNotificationTypes(notificationTypes);
        }
        LoadApplication(new App());

        return base.FinishedLaunching(app, options);
    }
```

Replace *"wsoexample"* with the name of your iOS app url scheme. See [Using Workspace ONE SDK Profiles to Deploy SDK Features to Apps](https://docs.omnissa.com/bundle/SDKandAppMgmtVSaaS/page/UEMSDKProfilesRequirements.html) about details for the app url scheme.

2. Ensure WorkspaceOne.OnActivated() is called.

```c
    public override void OnActivated(UIApplication uiApplication)
    {
        WorkspaceOne.iOS.WorkspaceOne.OnActivated();
    }
```

3. Implement the code to handle the callback from the Workspace ONE Intelligent Hub or Container app. Code for Handling Open Url:

```c
    public override bool HandleOpenURL(UIApplication application, NSUrl url)
    {
        return WorkspaceOne.iOS.WorkspaceOne.HandleOpenUrl(url, "");
    }
```

4. Implement Remote Notification Delegate callbacks and register the APNS token with SDK. Code for token registeration with SDK:

```c
    //Delegate callbacks for Remote Notification
    public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
    {
        System.Diagnostics.Debug.WriteLine("Token: {0}", deviceToken.ToString());
        byte[] result = new byte[deviceToken.Length];
        Marshal.Copy(deviceToken.Bytes, result, 0, (int)deviceToken.Length);
        var token = BitConverter.ToString(result).Replace("-", ""); //Remove "-" character from token string
        System.Diagnostics.Debug.WriteLine("Token: {0}", token);
        WorkspaceOne.iOS.WorkspaceOne.regisgterToken(token); //Register for Push Notification with SDK
    }
        
    //Delegate callback for Remote Notification Failure
    public override void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
    {
        System.Diagnostics.Debug.WriteLine("Token Error: {0}", error.ToString());
    }
```

### Android

1. In the `MainActivity.cs`'s `OnCreate` (where most other packages get initialized as well) add the following code `WorkspaceOne.Android.WorkspaceOne.Instance.Init(this);` and `WorkspaceOne.Android.WorkspaceOne.Instance.OnCreate(savedInstanceState)`. 

For example:

```c
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity, SDKGatewayActivityDelegate.ICallback
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            System.Diagnostics.Debug.WriteLine($"{this.GetType()}  OnCreate(Bundle)");
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            Exception workspaceOneException = null;

            try
            {
                WorkspaceOne.Android.WorkspaceOne.Instance.Init(this);
                WorkspaceOne.Android.WorkspaceOne.Instance.OnCreate(savedInstanceState);

                

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine($"{this.GetType()} {e}");
                workspaceOneException = e;
            }

            var app = new App();

            LoadApplication(app);

            if (workspaceOneException != null)
            {
                app.MainPage.DisplayAlert($"Error: {workspaceOneException.GetType()}", workspaceOneException.Message, "Ok");
            }
        }
```

2. `MainActivity` should conform to `SDKGatewayActivityDelegate.ICallback` and implement the following method.

```c
        public void OnTimeOut(SDKBaseActivityDelegate p0)
        {
            System.Diagnostics.Debug.WriteLine($"{this.GetType()}  OnTimeOut(SDKBaseActivityDelegate)");
            App.Current.MainPage.DisplayAlert($"Error: {p0.GetType()}", "OnTimeOut", "Ok");
        }
```

3. Add respective methods calls  for WorkspaceOne instance in MainActivity methods `OnResume()`, `OnPause()`, `OnStart()`,`OnStop()`,`OnDestroy()`, `OnUserInteraction()`, `DispatchKeyEvent(KeyEvent e)`,`DispatchKeyShortcutEvent(KeyEvent e)`,`DispatchTouchEvent(MotionEvent ev)`,`DispatchTrackballEvent(MotionEvent ev)`

```c
        protected override void OnResume()
        {
            System.Diagnostics.Debug.WriteLine($"{this.GetType()}  OnResume())");
            base.OnResume();
            WorkspaceOne.Android.WorkspaceOne.Instance.OnResume();
        }

        protected override void OnPause()
        {
            base.OnPause();
            WorkspaceOne.Android.WorkspaceOne.Instance.OnPause();
        }

        protected override void OnStart()
        {
            base.OnStart();
            WorkspaceOne.Android.WorkspaceOne.Instance.OnStart();
        }

        protected override void OnStop()
        {
            base.OnStop();
            WorkspaceOne.Android.WorkspaceOne.Instance.OnStop();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            WorkspaceOne.Android.WorkspaceOne.Instance.OnDestroy();
        }
        public override void OnUserInteraction()
        {
            base.OnUserInteraction();
            WorkspaceOne.Android.WorkspaceOne.Instance.OnUserInteraction();
        }

        public override bool DispatchKeyEvent(KeyEvent e)
        {
            WorkspaceOne.Android.WorkspaceOne.Instance.DispatchKeyEvent(e);
            return base.DispatchKeyEvent(e);
        }

        public override bool DispatchKeyShortcutEvent(KeyEvent e)
        {
            WorkspaceOne.Android.WorkspaceOne.Instance.DispatchKeyShortcutEvent(e);
            return base.DispatchKeyShortcutEvent(e);
        }

        public override bool DispatchTouchEvent(MotionEvent ev)
        {
            WorkspaceOne.Android.WorkspaceOne.Instance.DispatchTouchEvent(ev);
            return base.DispatchTouchEvent(ev);
        }

        public override bool DispatchTrackballEvent(MotionEvent ev)
        {
            WorkspaceOne.Android.WorkspaceOne.Instance.DispatchTrackballEvent(ev);
            return base.DispatchTrackballEvent(ev);
        }
```
    
4. Add `WorkspaceOneApplication.cs`    with following content 

```c
        using System;
        using Android.App;
        using Android.Content;
        using Android.Runtime;
        using WorkspaceOne.Android;
        
        namespace WorkspaceOne.Example.Droid
        {
            [Application]
            public class Application : WorkspaceOneApplication
            {
                public Application(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
                {
                    global::Android.Util.Log.Debug(this.GetType().ToString(), "ctor(IntPtr, JniHandleOwnership)");
                }

                public override Intent MainActivityIntent
                {
                    get
                    {
                        return new Intent(AwAppContext, typeof(MainActivity));
                    }
                }

                public override void OnCreate()
                {
                    System.Diagnostics.Debug.WriteLine($"{this.GetType()} OnCreate()");
                    base.OnCreate(this);
                }

            
            }
        }
```

5. For Firebase Cloud Message support complete the  FCM Integration as per Firebase Console and then add following code.

```c
        [Service]
        [IntentFilter(new[] { "com.google.firebase.INSTANCE_ID_EVENT" })]
        public class FirebaseInstanceIDService: FirebaseMessagingService
        {
          
            public override void OnNewToken(string p0)
            {
                System.Console.WriteLine(p0);
                base.OnNewToken(p0);
               
                WorkspaceOne.Android.WorkspaceOne.regisgterToken(p0);

            }

            public override void OnMessageReceived(RemoteMessage p0)
            {
                base.OnMessageReceived(p0);

                var notification = p0.GetNotification();
                if (notification != null)
                {
                    WorkspaceOne.Android.WorkspaceOne.processMessage(notification.Title, notification.Body, null);
                }
            }
        }
```        
        
## Forms

Wherever reasonable in your Xamarin.Forms app when you would like to set up the SDK, get an instance of the `IWorkspaceOne` service and assign the delegate property. This can be done in your `App.xaml.cs` for example.

### Procedure 

1. Import the `WorkspaceOne.Forms` & `WorkspaceOne.Forms.Interfaces`  associated with `IAWSDKDelegate` to `App.xaml.cs`

Import `using` 

```c
    using WorkspaceOne.Forms;
    using WorkspaceOne.Forms.Interfaces;
```

2. Inside the class App( ) constructor add the following Constructor code:

```c
    public App()
    {
        InitializeComponent(); 
        var ws = DependencyService.Get<IWorkspaceOne>();
        ws.FormsDelegate = this;
        MainPage = new NavigationPage(new MainPage());
    }
```

3. Make sure the instance of the class you assign as the delegate implements `IAWSDKDelegate`.

## Delegate Implementation

```c
    public partial class App : Application, IAWSDKDelegate
```

Make class `App` conform to `IAWSDKDelegate` 

```c
    void IAWSDKDelegate.InitialCheckFinished(bool isChecked)
```

This delegate method is invoked when the SDK initializes. This method is ALWAYS called after the SDK passes through the initialization flow.

```c
    void IAWSDKDelegate.ProfilesReceived(AWProfile[] profiles)
```

This delegate method is invoked when settings of an SDK profile assigned to this application update on the Workspace ONE UEM console. It notifies the app that new settings are available. The profiles array contains the list of AWProfile objects that contain configuration payloads.

```c
    void IAWSDKDelegate.Unlock()
```

This delegate method is invoked immediately after you initiate a new SSO session by inputting the correct password/passcode.
    
```c
    void IAWSDKDelegate.Lock()
```

This method is invoked when the SSO session has expired and the SDK passcode input view is displayed. It is intended for use as an indicator of when a user no longer has to access the app. This lock allows the developer to implement the necessary logic to take the proper action for when the app is locked.
    
```c
    void IAWSDKDelegate.Wipe()
```

This method is invoked when the SDK identifies that the device has been wiped or unenrolled from the Workspace ONE UEM console. This method is also invoked when a user reaches the limit of failed passcode attempts defined in the SDK profile.
    
```c
    void IAWSDKDelegate.StopNetworkActivity(AWNetworkActivityStatus status)
```

This method is invoked when the device connects to an SSID that is blacklisted in the SDK profile.
    
```c
     void IAWSDKDelegate.ResumeNetworkActivity()
```

This method is invoked when the device connections to a valid SSID after network activity is already stopped.

## Usage

The following components are available in Xamarin.Forms.

### OpenInURL calls

Request the device to open the Uri.

```c
    WorkspaceOne.Forms.Device.OpenUri(Uri uri)
```

### On-Device Cryptography

To encrypt custom data and to secure storage data, use the data encryption API set to encrypt and decrypt data.

```c
    var ws = DependencyService.Get<IWorkspaceOne>();

    byte[] encryptedBytes = ws.EncodeAndEncrypt(bytes);
        
    byte[] bytes = ws.DecodeAndDecrypt(encryptedBytes);
```

Note that the data is not stored by SDK by default, App needs to handle the storage by itself.

### Send Log to Console

```c
    var wso = DependencyService.Get<IWorkspaceOne>().SharedInstance;
    var not = wso != null ? "not" : "";
    Debug.WriteLine($"[{this.GetType()}] wso is {not} null");
    wso?.sendLogs();
```

## DLP on iOS

On iOS, to enable the Workspace ONE SDK app with DLP restrictions follow the below instructions:

1. Create a bundle named AWSDKDefaults.
2. Create a PLIST named AWSDKDefaultSettings.plist and put it in the AWSDKDefaults bundle. 
3. In the PLIST, create a Boolean named AWClipboardEnabled and set it to YES.

    After you add the local flag, and your admin sets the default or custom SDK policies for these features in the console, the SDK enforces the restriction. It enforces it across your application’s user interfaces that use cut, copy, and paste in the listed classes and subclasses.

```
        UITextField 
        UITextView 
        UIWebView
        WKWebView
```
    
4. Enable Links for Workspace ONE Web - In the PLIST, create a dictionary named AWURLSchemeConfiguration.
5. Inside the AWURLSchemeConfiguration dictionary, create a new Boolean entry with the key name enabled and set the Boolean value to Yes.

      If you set the Boolean value to No, then the HTTP and HTTPS links open in Safari. If set to Yes, then your SDK app opens in Workspace ONE Web.
    
6. Enable Links for Workspace ONE Boxer - In the PLIST, create a dictionary named AWMailtoSchemeConfiguration.
7.  Configure the AWMailtoSchemeConfiguration dictionary, create a new Boolean entry with the key name as enabled and set the Boolean value to Yes.

        If you set the Boolean value as No, then MAILTO links open in the native mail. If set to Yes, then your SDK app looks to see if you enabled data loss prevention in the SDK profile.

            DLP Enabled – The app opens in Workspace ONE Boxer.
            DLP Disabled – The app opens in the iOS Mail app.

## Branding on iOS

1. To enable branding on iOS Forms app, create the AWSDKDefaults and add AWSDKDefaultSettings.plist inside the AWSDKDefaults bundle as in above steps. 
2. Add enteries as per below structure:
            
```c
            Root (Dictionary)
            - Branding (Dictionary)
                Colors (Dictionary)
                    EnableBranding (Boolean = YES)
                        PrimaryHighlight (Dictionary) 
                        Red (Number = 238)
                        Green (Number = 139) 
                        Blue (Number = 48) 
                        Alpha (Number = 255)
                AppLogo_1x (String = logoFileName) 
                AppLogo_2x (String = logoFileName) 
                SplashLogo_1x (String = splashLogoFileName) 
                SplashLogo_2x (String = splashLogoFileName)
```

## Branding on Android

Modify the styles.xml in the Android Project for static branding.
Following is the snippet for styles.xml
    
```c
    <?xml version="1.0" encoding="utf-8"?>
    <resources>
        <style name="SDKBaseTheme" parent="VisionTheme.DayNight">
            <!-- Set theme colors from http://www.google.com/design/spec/style/color.html#color-color-palette -->
            <!-- colorPrimary is used for the default action bar background -->
            <item name="colorPrimary">#2196F3</item>
            <!-- colorPrimaryDark is used for the status bar -->
            <item name="colorPrimaryDark">#1976D2</item>
            <!-- colorAccent is used as the default value for colorControlActivated
             which is used to tint widgets -->
            <item name="colorAccent">#FF4081</item>
            <!-- You can also set colorControlNormal, colorControlActivated
             colorControlHighlight and colorSwitchThumbNormal. -->
            <item name="android:datePickerDialogTheme">@style/AppCompatDialogStyle</item>
            <!-- Splash screen icon Branding change -->
            <item name="splashLogo">@drawable/appicon</item>
            <!-- Login screen icon Branding change -->
            <item name="awsdkLoginBrandingIcon">@drawable/appicon</item>
            <item name="awsdkApplicationColorPrimary">#FF4081</item>
        </style>
        <style name="AppTheme.NoActionBar">
            <item name="windowActionBar">false</item>
            <item name="windowNoTitle">true</item>
        </style>
        <style name="AppTheme.AppBarOverlay" parent="ThemeOverlay.AppCompat.Dark.ActionBar"/>
        <style name="AppTheme.PopupOverlay" parent="VisionTheme.DayNight"/>
    </resources>
```    
    
## Secure Preferences

Use the secure storage API set of functions to store key value pairs in encrypted storage.

```c
    var ws = DependencyService.Get<IWorkspaceOne>();
    ws.SecurePreferences.Put(key, value);
    var value = ws.SecurePreferences.Get(key, defaultValue);
```

## AWCopyEnabledWebView

The AWCopyEnabledWebView will render as regular WebView on iOS.

```c
    <?xml version="1.0" encoding="utf-8"?>
    <ContentPage xmlns="http://xamarin.com/schemas/2014/forms" 
        xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml" 
        xmlns:d="http://xamarin.com/schemas/2014/forms/design" 
        xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006" mc:Ignorable="d" 
        x:Class="WorkspaceOne.Example.MainPage" 
        xmlns:aw="clr-namespace:WorkspaceOne.Forms.UI;assembly=WorkspaceOne.Forms">
        <StackLayout VerticalOptions="CenterAndExpand" HorizontalOptions="Fill">
            <aw:AWCopyEnabledWebView Source="https://www.seamgen.com" HorizontalOptions="FillAndExpand" />
        </StackLayout>
    </ContentPage>
```

## AWEditText

The AWEditText will render as regular Editor on iOS.

```c
    <?xml version="1.0" encoding="utf-8"?>
    <ContentPage 
        xmlns="http://xamarin.com/schemas/2014/forms" 
        xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml" 
        xmlns:d="http://xamarin.com/schemas/2014/forms/design" 
        xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006" mc:Ignorable="d" 
        x:Class="WorkspaceOne.Example.MainPage" 
        xmlns:aw="clr-namespace:WorkspaceOne.Forms.UI;assembly=WorkspaceOne.Forms">
        <StackLayout VerticalOptions="CenterAndExpand" HorizontalOptions="Fill">
            <aw:AWEditor Text="Test" HeightRequest="80" />
        </StackLayout>
    </ContentPage>
```

## AWTextView

The AWTextView will render as regular Entry on iOS.

```c
    <?xml version="1.0" encoding="utf-8"?>
    <ContentPage 
        xmlns="http://xamarin.com/schemas/2014/forms" 
        xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml" 
        xmlns:d="http://xamarin.com/schemas/2014/forms/design" 
        xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006" mc:Ignorable="d" 
        x:Class="WorkspaceOne.Example.MainPage" 
        xmlns:aw="clr-namespace:WorkspaceOne.Forms.UI;assembly=WorkspaceOne.Forms">
        <StackLayout VerticalOptions="CenterAndExpand" HorizontalOptions="Fill">
            <aw:AWEntry Text="Test" HeightRequest="80" />
        </StackLayout>
    </ContentPage>
```
