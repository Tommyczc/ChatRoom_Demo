using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Text;
using UnityEngine.UI;
using System.Threading;

public class chatRoom : MonoBehaviour
{
    private byte[] data = new byte[1024];
    public InputField inputcontext;
    public Socket TcpClient;
    //op��ͷ�ǲ�����
    private byte op0 = 0;//����
    private byte op1 = 1;//������Ϣ
    private byte op2 = 2;//ϵͳ��Ϣ�����û���
    private byte op6 = 6;//����˱�����
    string getmsg;
    private Thread thread;
    public Text te;
    public ScrollRect scrollRect;
    public String userName;
    private String tempName;
    private String tempMessage;
    private String tempSys;
    // Use this for initialization


    void Start()
    {
        tempMessage = "";
        tempName = "";
        tempSys = "";
        inputcontext.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        
        if (tempMessage != "" && tempName != "") {
            if (tempName == userName)
            {
                String temp =  "<color=red>" + tempName + "</color>: "+ tempMessage;
                te.text += temp + "\n";
                tempMessage = "";
                tempName = "";
            }
            else {
                String temp = "<color=blue>" + tempName + "</color>: " + tempMessage;
                te.text += temp + "\n";
                tempMessage = "";
                tempName = "";
            }
            //inputcontext.ActivateInputField();
            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 0f;
            Canvas.ForceUpdateCanvases();
        }
        
        


        if (Input.GetKeyUp(KeyCode.Return))
        {
            ButtonOnClickEvent(); 
            
        }   
        
    }
    void startChat()
    {
        //�������߳���������Ϣ
        thread = new Thread(GetmessageFromServer);
        thread.Start();
    }

    public void SendMessageToServer(string message)//��ʼ���������Ϣ 01 | userName | 0 | message
    {
        byte[] op = new byte[] {0,1};
        byte[] name = Encoding.UTF8.GetBytes(this.userName);
        byte[] mes = Encoding.UTF8.GetBytes(message);
        byte[] data = new byte[op.Length+name.Length+mes.Length+1];
        op.CopyTo(data, 0);
        name.CopyTo(data, op.Length);
        data[op.Length + name.Length] = op0;
        mes.CopyTo(data,op.Length+name.Length+1);
        try { 
            TcpClient.Send(data); 
        } catch (Exception ex) { 
        
        }
        
    }

    public void initialConnect(Socket s, String userName) {
        this.TcpClient = s;
        this.userName = userName;
        startChat();
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
            int length = TcpClient.Receive(data);
            //Debug.Log(length);
            if (length==0) {
                Debug.Log("disconnect");
                TcpClient.Close();
                break; 
            }
            if (data[1] == op1) {
                //Debug.Log("reciev a b mes from server");
                int i = 0;
                while (data[i + 2] != op0)
                {
                    i++;
                }
                int j = i + 2 + 1;
                while (data[j] != op0) {
                    j++;
                }
                j = j - i - 3;
                tempName = Encoding.UTF8.GetString(data, 2, i);
                tempMessage = Encoding.UTF8.GetString(data, i + 3, j);

            }
            else if (data[1]==op2) { 
                tempSys= Encoding.UTF8.GetString(data, 2, length-2);
            }
            data = new byte[1024];

        }
    }

    /// <summary>
    /// ��ȡ�ֽ�����, �����ϸ���
    /// </summary>
    /// <param name="srcBytes">Ҫ��ȡ���ֽ�����</param>
    /// <param name="startIndex">��ʼ��ȡλ�õ�����</param>
    /// <param name="length">Ҫ��ȡ���ֽڳ���</param>
    /// <returns>��ȡ����ֽ�����</returns>
    public byte[] SubByte(byte[] srcBytes, int startIndex, int length)
    {
        System.IO.MemoryStream bufferStream = new System.IO.MemoryStream();
        byte[] returnByte = new byte[] { };
        if (srcBytes == null) { return returnByte; }
        if (startIndex < 0) { startIndex = 0; }
        if (startIndex < srcBytes.Length)
        {
            if (length < 1 || length > srcBytes.Length - startIndex) { length = srcBytes.Length - startIndex; }
            bufferStream.Write(srcBytes, startIndex, length);
            returnByte = bufferStream.ToArray();
            bufferStream.SetLength(0);
            bufferStream.Position = 0;
        }
        bufferStream.Close();
        bufferStream.Dispose();
        return returnByte;
    }

    public void ButtonOnClickEvent()
    {
        string getmessages = inputcontext.text;
        if (getmessages != ""|| getmessages != null)
        {
            SendMessageToServer(getmessages);
            Debug.Log("type: " + getmessages.Length);
            inputcontext.text ="";
            //Debug.Log(getmessages);
        }
        else { Debug.Log("no word"); }
    }
    //�ú���ֻ�д��֮������ã�ƽʱ������Ч
    public void quit() {
        
        Application.Quit();
    }
}
