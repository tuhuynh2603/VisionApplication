
namespace VisionApplication.Model
{
    public interface IParameterService<T>
    {
        Task<T> AddParameter(T entity);
        Task DeleteParameter(int id);
        Task<IEnumerable<T>> GetAllParameters();
        Task<T> GetParameterById(int cameraID, int areaID = 0);
        Task UpdateParameter(T entity);
    }
}