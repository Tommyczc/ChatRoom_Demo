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
        //��������
        TcpClient.Connect(new IPEndPoint(IPAddress.Parse(ip), port));

        //�������߳���������Ϣ
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
            //�ڽ�������֮ǰ  �ж�һ��socket�����Ƿ�Ͽ�
            if (TcpClient.Connected == false)
            {
                Debug.Log("disconnect");
                TcpClient.Close();
                break;//����ѭ�� ��ֹ�̵߳�ִ��
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
    //�ú���ֻ�д��֮������ã�ƽʱ������Ч
    public void quit() {
        
        Application.Quit();
    }
}
