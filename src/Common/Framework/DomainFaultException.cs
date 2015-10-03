using System;
using System.Runtime.Serialization;

namespace EventualConsistency.Framework
{
    /// <summary>
    ///     Domain Fault
    /// </summary>
    [Serializable]
    public class DomainFaultException : Exception
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:DomainFaultException" /> class
        /// </summary>
        public DomainFaultException()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:DomainFaultException" /> class
        /// </summary>
        /// <param name="message">A <see cref="T:System.String" /> that describes the exception. </param>
        public DomainFaultException(string message) : base(message)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:DomainFaultException" /> class
        /// </summary>
        /// <param name="message">A <see cref="T:System.String" /> that describes the exception. </param>
        /// <param name="inner">The exception that is the cause of the current exception. </param>
        public DomainFaultException(string message, Exception inner) : base(message, inner)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:DomainFaultException" /> class
        /// </summary>
        /// <param name="context">The contextual information about the source or destination.</param>
        /// <param name="info">The object that holds the serialized object data.</param>
        protected DomainFaultException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            var faultType = info.GetValue("Fault-Type", typeof(Type));
            if (faultType != null)
            {
                Fault = (IDomainFault)info.GetValue(nameof(Fault), faultType as Type);
            }
        }

        /// <summary>
        ///     Get object data for instance being serialized
        /// </summary>
        /// <param name="info">Serialization information</param>
        /// <param name="context">Streaming context</param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            if (Fault != null)
            { 
                info.AddValue("Fault-Type", Fault.GetType());
                info.AddValue(nameof(Fault), Fault);
            }
        }
        /// <summary>
        ///     Domain Fault that occured
        /// </summary>
        /// <value>The fault.</value>
        public IDomainFault Fault { get; set; }
    }


    [Serializable]
    public class DomainFaultException<TFaultType> : DomainFaultException
        where TFaultType : IDomainFault
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:DomainFaultException" /> class
        /// </summary>
        public DomainFaultException()
        {
        }

        public DomainFaultException(TFaultType fault)
        {
            Fault = fault;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:DomainFaultException" /> class
        /// </summary>
        /// <param name="message">A <see cref="T:System.String" /> that describes the exception. </param>
        public DomainFaultException(string message) : base(message)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:DomainFaultException" /> class
        /// </summary>
        /// <param name="message">A <see cref="T:System.String" /> that describes the exception. </param>
        /// <param name="inner">The exception that is the cause of the current exception. </param>
        public DomainFaultException(string message, Exception inner) : base(message, inner)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:DomainFaultException" /> class
        /// </summary>
        /// <param name="context">The contextual information about the source or destination.</param>
        /// <param name="info">The object that holds the serialized object data.</param>
        protected DomainFaultException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <summary>
        ///     Domain Fault that occured
        /// </summary>
        /// <value>The fault.</value>
        public new TFaultType Fault
        {
            get { return (TFaultType) base.Fault; }
            set { base.Fault = value; }
        }
    }
}