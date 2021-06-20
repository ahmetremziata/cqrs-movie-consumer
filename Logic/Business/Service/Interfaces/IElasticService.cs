namespace Logic.Business.Service.Interfaces
{
    public interface IElasticService
    {
        void InsertMovie(string movieActivatedEvent);
        void DeleteMovie(string movieDeactivatedEvent);
    }
}