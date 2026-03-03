using System;

namespace Navi.Core.Exceptions
{
    /// <summary>
    /// Exception khi không tìm thấy công đoạn
    /// </summary>
    public class ProcessNotFoundException : Exception
    {
        public int ProcessId { get; }
        
        public ProcessNotFoundException(int processId) 
            : base($"Không tìm thấy công đoạn với ID: {processId}")
        {
            ProcessId = processId;
        }
        
        public ProcessNotFoundException(int processId, string message) 
            : base(message)
        {
            ProcessId = processId;
        }
        
        public ProcessNotFoundException(int processId, string message, Exception innerException) 
            : base(message, innerException)
        {
            ProcessId = processId;
        }
    }
}
