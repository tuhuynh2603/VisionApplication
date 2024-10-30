
using VisionApplication.Model;

namespace VisionApplication.Model
{
    public class CategoryVisionParameterService : IParameterService<CategoryVisionParameter>
    {
        private readonly CategoryVisionParameterRepository _repository;

        public CategoryVisionParameterService(CategoryVisionParameterRepository repository)
        {
            _repository = repository;
        }

        public async Task<CategoryVisionParameter> AddParameter(CategoryVisionParameter entity)
        {
            return await _repository.AddAsync(entity);
        }

        public async Task<CategoryVisionParameter> GetParameterById(int cameraID, int areaID = 0)
        {
            return await _repository.GetByIdAsync(cameraID, areaID);
        }

        public async Task<IEnumerable<CategoryVisionParameter>> GetAllParameters()
        {
            return await _repository.GetAllAsync();
        }

        public async Task UpdateParameter(CategoryVisionParameter entity)
        {
            await _repository.UpdateAsync(entity);
        }

        public async Task DeleteParameter(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}
