using System;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;
using static Aino.AinoMessage;
using Aino;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Assert = NUnit.Framework.Assert;
using System.Runtime.Serialization;
using Aino.Agents.Core;
using Aino.Agents.Core.Config;
using System.Threading;

namespace AinoTests
{
    [TestFixture]
    class SenderTest
    {
        private static AgentConfig validConfig;
        private IApiClient apiClient;
        private TransactionDataBuffer dataBuffer;
        private Sender _threadsender;

        //Todo: This needs work. For example .Net version of "when-then and verify.
        //[Test]
        public void TestRemainingDataIsSentOnShutdown()
        {
            const int trxCount = 10;
            TransactionDataBuffer dataBuffer = InitDataBuffer(trxCount);
            
            //when(apiClient.send(any(byte[].class))).thenReturn(apiResponse);
            byte[] anybytes = null;
            var result = apiClient.Send(anybytes);
            //Assert.That(result == IApiResponse)
            
            Sender sender = CreateSender();
            AddSenderThread();
            Thread thread = new Thread(new ThreadStart(AddSenderThread));
            thread.Start();
            sender.Stop();
            Thread.Sleep((int)1500);
            //verify(apiClient, times(trxCount)).send(any(byte[].class));
        }

        private void AddSenderThread()
        {
            Sender sender = CreateSender();
            _threadsender = sender;
        }

        private Sender CreateSender()
        {
            return new Sender(validConfig, dataBuffer, apiClient);
        }


        private TransactionDataBuffer InitDataBuffer(int trxCount)
        {
            TransactionDataBuffer dataBuffer = new TransactionDataBuffer(1);
            for (int i = 0; i < trxCount; i++)
            {

                Transaction transaction = new Transaction(validConfig);
                transaction.SetFromKey("app01");
                transaction.SetToKey("app02");
                transaction.SetStatus("success");
                dataBuffer.AddTransaction(TransactionSerializable.From(transaction));
            }

            return dataBuffer;
        }
    }
}