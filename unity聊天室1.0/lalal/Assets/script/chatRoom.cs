using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.Net;
using System.Text;
using UnityEngine.UI;
using System.Threading;

public class chatRoom : MonoBehaviour
{
    private byte[] data = new byte[1024];
    public InputField inputcontext;
    public string ip;
    public int port;
    private Socket TcpClient;
    string getmsg, oldmessage;
    private Thread thread;
    public Text te;
    // Use this for initialization
    void Start()
    {
        ConnectToServer();

    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (getmsg != null && getmsg != oldmessage)
        {
            te.text += getmsg + "\n";
            oldmessage = getmsg;
        }
        */
        if (getmsg != null )
        {
            te.text += getmsg + "\n";
            //oldmessage = getmsg;
            getmsg = null;
        }
        
        if (Input.GetKeyUp(KeyCode.Return)) {
            ButtonOnClickEvent();
        }

        
    }
    void ConnectToServer()
    {
        TcpClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //发起连接
        TcpClient.Connect(new IPEndPoint(IPAddress.Parse(ip), port));

        //创建新线程来接受消息
        thread = new Thread(GetmessageFromServer);
        thread.Start();
    }

    public void SendMessageToServer(string message)
    {
        byte[] data = Encoding.UTF8.GetBytes(message);
        TcpClient.Send(data);
    }


    public void GetmessageFromServer()
    {

        while (true)
        {
            //在接收数据之前  判断一下socket连接是否断开
            if (TcpClient.Connected == false)
            {
                Debug.Log("disconnect");
                TcpClient.Close();
                break;//跳出循环 终止线程的执行
            }
            TcpClient.ReceiveTimeout = 100000;
            Debug.Log(TcpClient.Connected);
            int getservermsglength = TcpClient.Receive(data);
            if (getservermsglength==0) {
                Debug.Log("disconnect");
                break; 
            }
            getmsg = Encoding.UTF8.GetString(data, 0, getservermsglength);
            Debug.Log(getmsg);

        }
    }

    public void ButtonOnClickEvent()
    {
        //Debug.Log("button is clicked");
        string getmessages = inputcontext.text;

        if (getmessages != "")
        {
            SendMessageToServer(getmessages);
            inputcontext.text = null;
            //Debug.Log(getmessages);
        }
        //else { Debug.Log("no word"); }
    }
    //该函数只有打包之后才有用，平时运行无效
    public void quit() {
        
        Application.Quit();
    }
}
