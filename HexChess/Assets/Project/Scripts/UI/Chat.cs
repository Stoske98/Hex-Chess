using Networking.Client;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Chat : MonoBehaviour
{
    public int max_messages = 50;
    public GameObject message_gameobject, chat_content;
    public TMP_InputField input;
    [SerializeField]
    List<Message> message_list = new List<Message>();
    public bool InGame = false;
    private Animator anim;
    private void Start()
    {
        anim = GetComponent<Animator>();
    }
    public void AddMessage(string text, string nickname)
    {
        if (message_list.Count >= max_messages)
        {
            Destroy(message_list[0].text_mesh.gameObject);
            message_list.Remove(message_list[0]);
        }

        Message new_message = new Message();
        new_message.text = "[" + nickname + "]" + " : " + text;
        new_message.nickname = nickname;

        new_message.text_mesh = Instantiate(message_gameobject, chat_content.transform).GetComponent<TextMeshProUGUI>();
        new_message.text_mesh.text = new_message.text;

        message_list.Add(new_message);

    }
    public void PopOut()
    {
        anim.Play("PopOut");

        AudioManager.Instance.OnClick();
    }
    public void PopIn()
    {
        anim.Play("PopIn");

        AudioManager.Instance.OnClick();

    }
    public void Idle()
    {
        anim.Play("Idle");

    }
    public void SendMessageRequest()
    {
        if(!string.IsNullOrEmpty(input.text))
        {
            if(!InGame)
            {
                NetChat_M request = new NetChat_M();
                request.message = input.text;
                Sender.TCP_SendToServer(request);
            }
            else
            {                
                NetChat request = new NetChat();
                request.message = input.text;
                Sender.TCP_SendToServer(request);                
            }
            input.text = "";
        }
    }
    [System.Serializable]
    public class Message
    {
        public string text;
        public TextMeshProUGUI text_mesh;
        public string nickname;
    }
}
