namespace Common.Exceptions
{
    public class NotFoundException : ApplicationException
    {
        public NotFoundException(string name, object key)
            : base($"Không thể tìm thấy {name} ({key})")
        {
        }
    }
}