using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Text;
using UnityEngine.UI;
using System.Threading;

public class register : MonoBehaviour
{
    byte[] buff = new byte[1024];
    public InputField inputcontext;
    public Button button;
    string userName;
    private Socket socket;
    public InputField inputip;
    public InputField inputport;
    public string ip;
    public int port;
    //op开头是操作码
    private byte op0 = 0;//握手
    private byte op1 = 1;//发送信息
    private byte op2 = 2;//系统消息，无用户名
    private byte op6 = 6;//服务端报错码
    public GameObject chatRoomPage;
    public GameObject chatRoom;
    void Start()
    {
        //ConnectToServer();
    }


    void ConnectToServer()
    {
        this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //发起连接
        try {
            this.socket.Connect(new IPEndPoint(IPAddress.Parse(this.ip), this.port));
        }
        catch (Exception ex) {
            Debug.Log(ex);
        }
        
    }

    //生成用户名
    public byte[] nameMessage(String userName) {
        byte[] op = new byte[] {0,0};
        byte[] data = Encoding.UTF8.GetBytes(userName);
        byte[] pack = new byte[data.Length + op.Length];
        op.CopyTo(pack, 0);
        data.CopyTo(pack, op.Length);
        Debug.Log("username package generated, length is: "+pack.Length);
        return pack;
    }

    //发送用户名申请
    public void registration() {
        if (inputcontext.text!=null) {
            userName = inputcontext.text;
            inputcontext.text = null;
            if (inputip.text!="") {
                Debug.Log("ip has changed");
                this.ip = inputip.text;
            }
            if (inputport.text!="") {
                Debug.Log("port has changed");
                int.TryParse(inputport.text, out this.port);
            }
            ConnectToServer();
            byte[] userMessage = nameMessage(userName);
            
            //Debug.Log(userName);
            try {
                socket.Send(userMessage);
            }
            catch (Exception ex) {
                Debug.Log(ex);
            }
            int getservermsglength = socket.Receive(buff);
            //Debug.Log(getservermsglength+" , "+ userMessage.Length);
            if (buff[1] == op0) { //此处证明该用户注册用户名成功
                Debug.Log("用户名注册成功！");
                //这里是场景的转换
                this.GetComponent<Canvas>().enabled = false;
                chatRoom.GetComponent<chatRoom>().enabled = true;
                chatRoom.GetComponent<chatRoom>().initialConnect(this.socket,this.userName);
                chatRoomPage.GetComponent<Canvas>().enabled = true;
                this.GetComponent<register>().enabled = false;
                Destroy(this,1.0f);
            }
        }
    }
}
