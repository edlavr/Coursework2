﻿using System;
using System.Collections;
using System.Collections.Generic;
using Ubiq.Messaging;
using Ubiq.XR;
using UnityEngine;

public class BigHandler : MonoBehaviour, INetworkComponent, INetworkObject
{
    public HashSet<Hand> movers = new HashSet<Hand>();

    public int nOfMoversNeeded;
    
    private NetworkContext context;
    NetworkId INetworkObject.Id => new NetworkId(123456789);

    public void MoverReady(Hand mover)
    {
        movers.Add(mover);
        context.SendJson(new Message(transform.position, mover, true));
    }

    
    private void Start()
    {
        context = NetworkScene.Register(this);
    }

    private void Update()
    {
        Move();
    }

    public void Move()
    {
        Debug.Log(movers.Count);
        if (movers.Count == nOfMoversNeeded)
        {
            Vector3 movement = Vector3.zero;
            foreach (var mover in movers)
            {
                movement += mover.transform.position;
            }

            movement /= nOfMoversNeeded;

            transform.localPosition = movement;
        }
    }

    private struct Message
    {
        public Vector3 Position;
        public Hand Movers;
        public bool IsGrasp;

        public Message(Vector3 position, Hand movers, bool isGrasp)
        {
            Movers = movers;
            Position = position;
            IsGrasp = isGrasp;
        }
    }
    
    public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
    {
        var msg = message.FromJson<Message>();
        transform.localPosition = msg.Position;
        if (msg.IsGrasp)
        {
            movers.Add(msg.Movers);
        }
        else
        {
            movers.Remove(msg.Movers);
        }
    }

}
