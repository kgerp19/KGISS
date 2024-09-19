namespace KGERP.Utility
{
    public class ErrorMessage
    {
        public bool IsError { get; private set; }

        private string _message;
        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                IsError = false;
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _message = value;
                    IsError = true;
                }
            }
        }
    }
}
