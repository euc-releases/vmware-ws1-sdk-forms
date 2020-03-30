# vmware-ws1-sdk-forms
# **VM**ware Workspace ONE SDK for Xamarin.Forms

This documentation will cover the [installation](#installation), [setup](#setup) and [usage](#usage) of the **vm**ware Workspace ONE SDK for Xamarin Forms.

<small>WorkspaceOne SDK for Xamarin Forms is dependent on AWSDK version 1.5.0 or higher.</small>

## Installation

The SDK should be installed using **Nuget**.

- **WorkspaceOne.Forms**: This is the package to be used in your Xamarin Forms app. It will provide interfaces for the initialization, setup and usage of the Workspace ONE module from your Xamarin Forms app.

Add this nuget package to your Xamarin.Forms project and to your iOS and Android project of the the Xamarin.Forms app as well.

Add the appropriate packages to your solution for each app project. Then continue to the [setup](#setup) step for [Android](#android) and [iOS](#ios).

## Setup

Before using the Workspace ONE SDK, just like many other Xamarin Forms packages it's dependencies need to be initialized first. In addition to adding the Forms package, each target platform needs to add the SDKs package for that specific platform as well. If you target only one of the two supported platforms, skip all steps for the one you don't support.

### iOS
## Procedure
1. In the `AppDelegate.cs`'s `FinishedLaunching` (just where most other packages get initialized as well) add the following code `WorkspaceOne.iOS.WorkspaceOne.Init("wsoexample");`. 

For example like this:

    public override bool FinishedLaunching(UIApplication app, NSDictionary options)
    {
        global::Xamarin.Forms.Forms.Init();

        WorkspaceOne.iOS.WorkspaceOne.Init("wsoexample");//Replace "wsoexample" with the name of your iOS app url scheme

        LoadApplication(new App());

        return base.FinishedLaunching(app, options);
    }

Replace *"wsoexample"* with the name of your iOS app url scheme. See [the documentation for the WorkspaceOne SDK for iOS](https://docs.vmware.com/en/VMware-Workspace-ONE-UEM/services/VMware-Workspace-ONE-SDK-for-iOS-(Swift)/GUID-AWT-INITIALIZESDK.html#GUID-AWT-INITIALIZESDK) about details for the app url scheme.

2. Ensure WorkspaceOne.OnActivated() is called.

Something like this

    public override void OnActivated(UIApplication uiApplication)
    {
        WorkspaceOne.iOS.WorkspaceOne.OnActivated();
    }

3. Implement the code to handle the callback from the Workspace ONE Intelligent Hub or Container app.

Code for Handling Open Url

    public override bool HandleOpenURL(UIApplication application, NSUrl url)
    {
        return WorkspaceOne.iOS.WorkspaceOne.HandleOpenUrl(url, "");
    }


### Android

1. In the `MainActivity.cs`'s `OnCreate` (where most other packages get initialized as well) add the following code `WorkspaceOne.Android.WorkspaceOne.Instance.Init(this);` and `WorkspaceOne.Android.WorkspaceOne.Instance.OnCreate(savedInstanceState)`. 


For example:

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

2. `MainActivity` should conform to `SDKGatewayActivityDelegate.ICallback` and implement the following method.

        public void OnTimeOut(SDKBaseActivityDelegate p0)
        {
            System.Diagnostics.Debug.WriteLine($"{this.GetType()}  OnTimeOut(SDKBaseActivityDelegate)");
            App.Current.MainPage.DisplayAlert($"Error: {p0.GetType()}", "OnTimeOut", "Ok");
        }
        

3. Add respective methods calls  for WorkspaceOne instance in MainActivity methods `OnResume()`, `OnPause()`, `OnStart()`,`OnStop()`,`OnDestroy()`, `OnUserInteraction()`, `DispatchKeyEvent(KeyEvent e)`,`DispatchKeyShortcutEvent(KeyEvent e)`,`DispatchTouchEvent(MotionEvent ev)`,`DispatchTrackballEvent(MotionEvent ev)`
        
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
    
    
4. Add `WorkspaceOneApplication.cs`    with following content 

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
                    AWSDKDelegate.OnCreate(this);
                }

            
            }
        }
### Forms

Wherever reasonable in your Xamarin.Forms app when you would like to set up the SDK, get an instance of the `IWorkspaceOne` service and assign the delegate property. This can be done in your `App.xaml.cs` for example.
## Procedure 
1. Import the `WorkspaceOne.Forms` & `WorkspaceOne.Forms.Interfaces`  associated with `IAWSDKDelegate` to `App.xaml.cs`

Import `using` 

    using WorkspaceOne.Forms;
    using WorkspaceOne.Forms.Interfaces;

2. Inside the class App( ) constructor add the following 

Contructor code 

    public App()
    {
            InitializeComponent(); 
            var ws = DependencyService.Get<IWorkspaceOne>();
            ws.FormsDelegate = this;
            MainPage = new NavigationPage(new MainPage());
    }

3. Make sure the instance of the class you assign as the delegate implements `IAWSDKDelegate`.

## Delegate Implementation

    public partial class App : Application, IAWSDKDelegate
    
Make class `App` conform to `IAWSDKDelegate` 


    void IAWSDKDelegate.InitialCheckFinished(bool isChecked)
   
This delegate method is invoked when the SDK initializes. This method is ALWAYS called after the SDK passes through the initialization flow.


    void IAWSDKDelegate.ProfilesReceived(AWProfile[] profiles)

This delegate method is invoked when settings of an SDK profile assigned to this application update on the Workspace ONE UEM console. It notifies the app that new settings are available. The profiles array contains the list of AWProfile objects that contain configuration payloads.


    void IAWSDKDelegate.Unlock()
    
This delegate method is invoked immediately after you initiate a new SSO session by inputting the correct password/passcode.
    
    
    void IAWSDKDelegate.Lock()
    
This method is invoked when the SSO session has expired and the SDK passcode input view is displayed. It is intended for use as an indicator of when a user no longer has to access the app. This lock allows the developer to implement the necessary logic to take the proper action for when the app is locked.
    
    
    void IAWSDKDelegate.Wipe()
    
This method is invoked when the SDK identifies that the device has been wiped or unenrolled from the Workspace ONE UEM console. This method is also invoked when a user reaches the limit of failed passcode attempts defined in the SDK profile.
    
    
    void IAWSDKDelegate.StopNetworkActivity(AWNetworkActivityStatus status)
    
This method is invoked when the device connects to an SSID that is blacklisted in the SDK profile.
    

     void IAWSDKDelegate.ResumeNetworkActivity()
     
This method is invoked when the device connections to a valid SSID after network activity is already stopped.

## Usage

The following components are available in Xamarin.Forms.

### OpenInURL calls

Request the device to open the Uri.

    WorkspaceOne.Forms.Device.OpenUri(Uri uri)

### On-Device Cryptography

To encrypt custom data and to secure storage data, use the data encryption API set to encrypt and decrypt data.

    var ws = DependencyService.Get<IWorkspaceOne>();

    byte[] encryptedBytes = ws.EncodeAndEncrypt(bytes);
        
    byte[] bytes = ws.DecodeAndDecrypt(encryptedBytes);

Note that the data is not stored by SDK by default, App needs to handle the storage by itself.

### DLP on iOS

On iOS, to enable the WSOne SDK app with DLP restrictions follow the below instructions:

## Procedure
1. Create a bundle named AWSDKDefaults.

2. Create a PLIST named AWSDKDefaultSettings.plist and put it in the AWSDKDefaults bundle. 
    
    ## Copy-Paste
3. In the PLIST, create a Boolean named AWClipboardEnabled and set it to YES.

    After you add the local flag, and your admin sets the default or custom SDK policies for these features in the console, the SDK enforces the restriction. It enforces it across your application’s user interfaces that use cut, copy, and paste in the listed classes and subclasses.

        UITextField 
        UITextView 
        UIWebView
        WKWebView
    
    ## Enable Links for Workspace ONE Web
4. In the PLIST, create a dictionary named AWURLSchemeConfiguration.

5. Inside the AWURLSchemeConfiguration dictionary, create a new Boolean entry with the key name enabled and set the Boolean value to Yes.

      If you set the Boolean value to No, then the HTTP and HTTPS links open in Safari. If set to Yes, then your SDK app opens in Workspace ONE Web.
    
    ## Enable Links for Workspace ONE Boxer
6. In the PLIST, create a dictionary named AWMailtoSchemeConfiguration.

7.  Configure the AWMailtoSchemeConfiguration dictionary, create a new Boolean entry with the key name as enabled and set the Boolean value to Yes.

        If you set the Boolean value as No, then MAILTO links open in the native mail. If set to Yes, then your SDK app looks to see if you enabled data loss prevention in the SDK profile.

            DLP Enabled – The app opens in Workspace ONE Boxer.
            DLP Disabled – The app opens in the iOS Mail app.

### Secure Preferences

Use the secure storage API set of functions to store key value pairs in encrypted storage.

    var ws = DependencyService.Get<IWorkspaceOne>();
    ws.SecurePreferences.Put(key, value);
    var value = ws.SecurePreferences.Get(key, defaultValue);

### AWCopyEnabledWebView

<small>*The AWCopyEnabledWebView will render as regular WebView on iOS.</small>

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

### AWEditText

<small>*The AWEditText will render as regular Editor on iOS.</small>

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

### AWTextView

<small>*The AWTextView will render as regular Entry on iOS.</small>

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
