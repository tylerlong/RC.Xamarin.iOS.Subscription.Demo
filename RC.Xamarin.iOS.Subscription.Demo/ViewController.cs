using System;
using System.Collections.Generic;
using System.Threading;
using Newtonsoft.Json;
using RingCentral.SDK;
using RingCentral.SDK.Http;
using RingCentral.Subscription;
using UIKit;

namespace RC.Xamarin.iOS.Subscription.Demo
{
	public partial class ViewController : UIViewController
	{
		public ViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			// Perform any additional setup after loading the view, typically from a nib.

			var sdk = new SDK (Config.AppKey, Config.AppSecret, Config.Server, "Test App", "1.0.0");
			var platform = sdk.GetPlatform ();
			platform.Authorize (Config.Username, Config.Extension, Config.Password, false);

			var sub = new SubscriptionServiceImplementation() { _platform = platform };
			sub.AddEvent("/restapi/v1.0/account/~/extension/~/message-store");
			sub.Subscribe((message) => {
				Console.WriteLine("Event Received");
			}, null, null);

			var dict = new Dictionary<string, dynamic> {
				{ "text", "hello world" },
				{ "from", new Dictionary<string, string> { { "phoneNumber", Config.Username} } },
				{ "to", new Dictionary<string, string>[] { new Dictionary<string, string> { { "phoneNumber", Config.Receiver } } } },
			};
			var request = new Request("/restapi/v1.0/account/~/extension/~/sms", JsonConvert.SerializeObject(dict));
			for(var i=0; i < 10; i++)
			{
				platform.Post(request);
				Thread.Sleep (30000);
			}
			sub.Remove();
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}
	}
}
