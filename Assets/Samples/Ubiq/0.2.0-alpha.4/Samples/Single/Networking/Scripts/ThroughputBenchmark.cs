﻿using System;
using System.Security.Cryptography;
using Ubiq.Messaging;
using UnityEngine;

namespace Samples.Ubiq._0._2._0_alpha._4.Samples.Single.Networking.Scripts
{
    public class ThroughputBenchmark : MonoBehaviour, INetworkComponent, INetworkObject
    {
        private NetworkContext context;

        public int messagesPerFrame = 10;
        public int maxMessageSize = 100000;

        public int totalMessages = int.MaxValue;

        public int sent = 0;
        public int received = 0;

        public bool run = false;
        public bool corrupt = false;

        private SHA256 sha526;
        private const int hashLength = 32;

        public NetworkId Id { get; } = new NetworkId("a2a6-6b97-e7b6-6277");

        private void Awake()
        {
            sha526 = SHA256.Create();
        }

        private void Start()
        {
            context = NetworkScene.Register(this);
        }

        public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
        {
            var hash = sha526.ComputeHash(message.bytes, message.start, message.length - hashLength);
            var compare = new ReadOnlySpan<byte>(message.bytes, message.start + message.length - hashLength, hashLength).SequenceEqual(hash);
            if(!compare)
            {
                Debug.LogError("Message Corruption Detected");
            }
            received++;
        }

        // Update is called once per frame
        void Update()
        {
            if (run)
            {
                for (int i = 0; i < messagesPerFrame; i++)
                {
                    if(sent >= totalMessages)
                    {
                        return;
                    }

                    int length = UnityEngine.Random.Range(hashLength + 2, maxMessageSize);
                    var message = ReferenceCountedSceneGraphMessage.Rent(length);

                    var datalength = length - hashLength;

                    for (int j = 0; j < datalength; j++)
                    {
                        message.bytes[message.start + j] = (byte)UnityEngine.Random.Range(0, 255);
                    }

                    Array.Copy(sha526.ComputeHash(message.bytes, message.start, datalength), 0, message.bytes, message.start + datalength, hashLength);

                    if (corrupt)
                    {
                        for (int j = 0; j < datalength; j++)
                        {
                            message.bytes[message.start + j] = (byte)UnityEngine.Random.Range(0, 255);
                        }
                    }

                    context.Send(message);

                    sent++;
                }
            }
        }
    }
}