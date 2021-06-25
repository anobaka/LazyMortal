namespace Bootstrap.Components.Notification.RequestModels
{
    public abstract class SearchRequestModel
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
