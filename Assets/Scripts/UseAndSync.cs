﻿using Ubiq.Messaging;
using Ubiq.XR;
using UnityEngine;

using PlayerNumber = System.Int32;

public class UseAndSync : MonoBehaviour, IUseable, INetworkComponent, INetworkObject
{
    public StateLight indicator;
    private AccessManager accessManager;

    public uint id;
    NetworkId INetworkObject.Id => new NetworkId(id);

    private NetworkContext context;

    void INetworkComponent.ProcessMessage(ReferenceCountedSceneGraphMessage message)
    {
        var msg = message.FromJson<Message>();
        Debug.Log(msg.State);
        indicator.ChangeState(msg.State);
    }

    private void Awake()
    {
        accessManager = GetComponent<AccessManager>();
    }

    private void Start()
    {
        context = NetworkScene.Register(this);
    }

    struct Message
    {
        public readonly bool State;

        public Message(bool aState)
        {
            State = aState;
        }
    }
    
    public void Use(Hand controller)
    {
        if (accessManager.available && !accessManager.locked)
        {
            bool state = !indicator.State;
            indicator.ChangeState(state);
            Message message = new Message(state);
            context.SendJson(message);
        }
    }

    public void UnUse(Hand controller)
    {
        
    }
}