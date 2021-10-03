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
    //op��ͷ�ǲ�����
    private byte op0 = 0;//����
    private byte op1 = 1;//������Ϣ
    private byte op2 = 2;//ϵͳ��Ϣ�����û���
    private byte op6 = 6;//����˱�����
    public GameObject chatRoomPage;
    public GameObject chatRoom;
    void Start()
    {
        //ConnectToServer();
    }


    void ConnectToServer()
    {
        this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //��������
        try {
            this.socket.Connect(new IPEndPoint(IPAddress.Parse(this.ip), this.port));
        }
        catch (Exception ex) {
            Debug.Log(ex);
        }
        
    }

    //�����û���
    public byte[] nameMessage(String userName) {
        byte[] op = new byte[] {0,0};
        byte[] data = Encoding.UTF8.GetBytes(userName);
        byte[] pack = new byte[data.Length + op.Length];
        op.CopyTo(pack, 0);
        data.CopyTo(pack, op.Length);
        Debug.Log("username package generated, length is: "+pack.Length);
        return pack;
    }

    //�����û�������
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
            if (buff[1] == op0) { //�˴�֤�����û�ע���û����ɹ�
                Debug.Log("�û���ע��ɹ���");
                //�����ǳ�����ת��
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
