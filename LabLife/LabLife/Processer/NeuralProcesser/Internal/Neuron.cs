using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LabLife.Processer.NeuralProcesser.Internal.Interface;

namespace LabLife.Processer.NeuralProcesser.Internal
{
    public class Neuron : Interface.IReceiverNeuron, Interface.ISenderNeuron
    {
        public List<KeyValuePair<IReceiverNeuron, TransferCoefficient>> m_Children;
        public List<ISenderNeuron> m_Parent;
        public List<Interface.INeurotransmitter> m_ReceivedTransmittersList;
        /// <summary>
        /// 
        /// </summary>
        private ExcitationIntensity m_EI;
        private Energy m_Energy;

        public Neuron(double Energy)
        {
            this.Initialize();
            this.m_Energy = new Internal.Energy(Energy);
        }

        public void Initialize()
        {
            this.m_Children = new List<KeyValuePair<IReceiverNeuron, TransferCoefficient>>();
            this.m_Parent = new List<ISenderNeuron>();
            this.m_ReceivedTransmittersList = new List<INeurotransmitter>();
            this.m_EI = new ExcitationIntensity(0, 1, 0.9);
        }


        /// <summary>
        /// 受け取ったものを自身の物質群に反映
        /// </summary>
        /// <param name="neurotransmitter"></param>
        public void Receive(INeurotransmitter neurotransmitter)
        {
            this.m_ReceivedTransmittersList.Add(neurotransmitter);
        }

        public void ConnectTo(IReceiverNeuron target, TransferCoefficient TC)
        {
            this.m_Children.Add(new KeyValuePair<IReceiverNeuron, TransferCoefficient>(target, TC));
            target.ConnectFrom(this);
        }
        /// <summary>
        /// 外部からの操作を禁ず
        /// </summary>
        /// <param name="SenderNeuron"></param>
        public void ConnectFrom(ISenderNeuron SenderNeuron)
        {
            this.m_Parent.Add(SenderNeuron);
        }

        /// <summary>
        /// 外部からの操作を禁ず
        /// </summary>
        /// <param name="ReceiverNeuron"></param>
        public void RemoveConnection(IReceiverNeuron ReceiverNeuron)
        {
            this.m_Children.RemoveAll(p => p.Key.Equals(ReceiverNeuron));
        }

        /// <summary>
        /// If own Excitation Intencity is High, to send transmitter to children;
        /// </summary>
        public void Update()
        {

            //受信した伝達物質のカウント
            foreach (var p in this.m_ReceivedTransmittersList)
            {
                foreach (var q in p.getParams())
                {
                    this.m_EI.add(q);
                }
            }
            this.m_ReceivedTransmittersList.Clear();

            //励起状態の判定
            if (this.m_EI.State == ExcitationState.High)
            {

                //伝達物質の作成
                Neurotransmitter nt = new Neurotransmitter(m_EI.Value);
                //エネルギーの消費
                this.m_Energy.Use(m_EI.Value);

                //送信
                foreach (var p in this.m_Children)
                {
                    this.Send(p.Key, p.Value.Cofficient(nt));
                }
            }

            //興奮状態の減衰
            this.m_EI.Reduce();

            //死亡判定 過労死
            if (!this.m_Energy.IsLeaving)
            {
                this.Die();
            }
            
        }

        /// <summary>
        /// 親に死亡を通知 削除される
        /// </summary>
        public void Die()
        {
            //親から自身を削除
            foreach (var p in this.m_Parent)
            {
                p.RemoveConnection(this);
            }
        }


        public ExcitationState GetCurrentState()
        {
            return this.m_EI.State;
        }

        public void Send(IReceiverNeuron receiverNeuron, INeurotransmitter neurotransmitter)
        {
            receiverNeuron.Receive(neurotransmitter);
        }

        //補給
        public void AddEnergy(double value)
        {
            this.m_Energy.Add(value);
        }

        public NeuronMode GetMode()
        {
            return NeuronMode.Normal;
        }
    }
}
