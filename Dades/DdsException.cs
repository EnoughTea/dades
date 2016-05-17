using System;
using System.Runtime.Serialization;

namespace Dades
{
    /// <summary> Represents errors that occur during DDS format parsing. </summary>
    [Serializable]
    public class DdsException : Exception
    {
        /// <summary> Initializes a new instance of the <see cref="DdsException" /> class. </summary>
        public DdsException()
        {
        }

        /// <summary> Initializes a new instance of the <see cref="DdsException" /> class. </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public DdsException(string message) : base(message)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DdsException" /> class with a specified error
        ///     message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">
        ///     The exception that is the cause of the current exception, or a null reference
        ///     (Nothing in Visual Basic) if no inner exception is specified.
        /// </param>
        public DdsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary> Initializes a new instance of the <see cref="DdsException" /> class with serialized data. </summary>
        /// <param name="info">
        ///     The <see cref="System.Runtime.Serialization.SerializationInfo" /> that holds the
        ///     serialized object data about the exception being thrown.
        /// </param>
        /// <param name="context">
        ///     The <see cref="System.Runtime.Serialization.StreamingContext" /> that contains
        ///     contextual information about the source or destination.
        /// </param>
        protected DdsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
