namespace Mandara.ProductGUI.Desks
{
    public class DatabaseActionResult
    {
        public bool Success { get; }
        public string Message { get; }

        public DatabaseActionResult(bool success, string msg)
        {
            Success = success;
            Message = msg;
        }
    }
}
