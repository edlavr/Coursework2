﻿using System;
using System.Collections;
using System.Collections.Generic;
using Ubiq.Messaging;
using Ubiq.XR;
using UnityEngine;

public class BigHandler : MonoBehaviour, INetworkComponent, INetworkObject
{
    public HashSet<MoveAndSyncBig> movers = new HashSet<MoveAndSyncBig>();

    public int nOfMoversNeeded;
    
    private NetworkContext context;
    NetworkId INetworkObject.Id => new NetworkId(123456789);

    public void MoverReady(MoveAndSyncBig mover)
    {
        movers.Add(mover);
        Move(mover);
    }
    
    public void MoverReleased(MoveAndSyncBig mover)
    {
        movers.Remove(mover);
    }
    
    private void Start()
    {
        context = NetworkScene.Register(this);
    }

    private void Update()
    {
        
    }

    public void Move(MoveAndSyncBig _mover)
    {
        Debug.Log(movers.Count);
        if (movers.Count == nOfMoversNeeded)
        {
            Vector3 movement = Vector3.zero;
            foreach (var mover in movers)
            {
                movement += mover.grasped.transform.position;
            }

            movement /= nOfMoversNeeded;

            transform.localPosition = movement;
            context.SendJson(new Message(transform.position, _mover));
        }
    }

    private struct Message
    {
        public Vector3 Position;
        public MoveAndSyncBig Movers;

        public Message(Vector3 position, MoveAndSyncBig movers)
        {
            Movers = movers;
            Position = position;
        }
    }
    
    public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
    {
        var msg = message.FromJson<Message>();
        transform.localPosition = msg.Position;
        movers.Add(msg.Movers);
    }

}
