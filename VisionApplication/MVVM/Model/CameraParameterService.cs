
using VisionApplication.Model;

namespace VisionApplication.MVVM.Model
{
    public class CameraParameterService : IParameterService<CameraParameter>
    {
        private readonly CameraParameterRepository _repository;

        public CameraParameterService(CameraParameterRepository repository)
        {
            _repository = repository;
        }

        public async Task<CameraParameter> AddParameter(CameraParameter entity)
        {
            return await _repository.AddAsync(entity);
        }

        public async Task<CameraParameter> GetParameterById(int id, int nArea = 0)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<CameraParameter>> GetAllParameters()
        {
            return await _repository.GetAllAsync();
        }

        public async Task UpdateParameter(CameraParameter entity)
        {
            await _repository.UpdateAsync(entity);
        }

        public async Task DeleteParameter(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}
