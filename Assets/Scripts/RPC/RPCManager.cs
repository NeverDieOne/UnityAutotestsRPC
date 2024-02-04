using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using AustinHarris.JsonRpc;
using UnityEngine;


class NetworkEvent {
	public string clientRequest;
	public string serverReply;
	public AutoResetEvent serverReplied = new(false);
	public NetworkEvent(string clientRequest) {
		this.clientRequest = clientRequest;
	}
}


public class RPCManager : MonoBehaviour {
	[Tooltip("Network port to listen on.")]
	public int ListenPort = 9000;

	[Tooltip("Network address to listen on. If not sure, put 'localhost'")]
	public string ListenAddress = "localhost";

	[Tooltip("Ensure the application runs in background. If you set this to false, you might wonder why your python scripts appears to hang")]
	public bool AutoEnableRunInBackground = true;

    readonly bool blockingListen = false;
	bool isDedicated;
	volatile bool isEnabled = true;

	HttpListener listener;
	readonly BlockingCollection<NetworkEvent> networkEvents = new();
	private void Awake() {
		if (AutoEnableRunInBackground) {
			Application.runInBackground = true;
			Debug.Log("Enabled Application.runInBackground");
		} else {
			Debug.Log("Warning: NOT enabling Application.runInBackground. This means you will need to switch Unity to foreground for things to run.");
		}
	}

	void Start() {
		new RpcMethods();
	}

    [Obsolete]
    public bool IsDedicated() {
		return Screen.currentResolution.refreshRate == 0;
	}

    [Obsolete]
    public void OnEnable() {
		isEnabled = true;
		isDedicated = IsDedicated();

		listener = new HttpListener();
		listener.Prefixes.Add($"http://{ListenAddress}:{ListenPort}/");

		listener.Start();
		Task.Run(ListenLoop);
	}

	public void OnDisable() {
		isEnabled = false;
		Debug.Log("shutting down listener");
		if(listener is null) {
			return;
		}
		listener.Stop();
		listener.Abort();
		listener.Close();
	}

	void FixedUpdate() {
		if (networkEvents.Count > 0 || blockingListen || isDedicated) {
			NetworkEvent networkEvent = networkEvents.Take();
			networkEvent.serverReply = JsonRpcProcessor.ProcessSync(
				Handler.DefaultSessionId(), networkEvent.clientRequest, null);
			networkEvent.serverReplied.Set();
		}
	}

	void HandleRequest(HttpListenerContext context) {
		HttpListenerRequest req = context.Request;

		string bodyText;
		using(var reader = new StreamReader(req.InputStream, req.ContentEncoding)) {
			bodyText = reader.ReadToEnd();
		}

		NetworkEvent networkEvent = new(bodyText);
		networkEvents.Add(networkEvent);
		networkEvent.serverReplied.WaitOne();
		string res = networkEvent.serverReply;

		using HttpListenerResponse resp = context.Response;
		resp.Headers.Set("Content-Type", "application/json");

		byte[] buffer = Encoding.UTF8.GetBytes(res);
		resp.ContentLength64 = buffer.Length;

		using Stream ros = resp.OutputStream;
		ros.Write(buffer, 0, buffer.Length);
	}

	public void ListenOnce() {
		if(listener is null) {
			return;
		}
		HttpListenerContext context = listener.GetContext();
		HandleRequest(context);
	}

	void ListenLoop() {
		while (true) {
			try {
				ListenOnce();
			} catch(ObjectDisposedException) {
				if (isEnabled) {
					try {
						listener?.Abort();
					} catch(Exception e) {
                        Debug.Log(e);
					}
					try {
						listener = new HttpListener();
						listener.Prefixes.Add($"http://{ListenAddress}:{ListenPort}/");

						listener.Start();
					} catch(Exception e) {
						throw e;
					}
				} else {
					// shutdown
					break;
				}
			} catch(System.Net.HttpListenerException e) {
				if(e.Message == "Listener closed") {
					break;
				}
			} catch(System.Threading.ThreadAbortException) {
				break;
			} catch(Exception e) {
				Debug.Log(e);
			}
		}
	}
}