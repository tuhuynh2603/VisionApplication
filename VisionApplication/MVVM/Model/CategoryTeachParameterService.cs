
using VisionApplication.Model;

namespace VisionApplication.Model
{
    public class CategoryTeachParameterService: IParameterService<CategoryTeachParameter>
    {
        private readonly CategoryTeachParameterRepository _repository;

        public CategoryTeachParameterService(CategoryTeachParameterRepository repository)
        {
            _repository = repository;
        }

        public async Task<CategoryTeachParameter> AddParameter(CategoryTeachParameter entity)
        {
            return await _repository.AddAsync(entity);
        }

        public async Task<CategoryTeachParameter> GetParameterById(int id, int nArea = 0)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<CategoryTeachParameter>> GetAllParameters()
        {
            return await _repository.GetAllAsync();
        }

        public async Task UpdateParameter(CategoryTeachParameter entity)
        {
            await _repository.UpdateAsync(entity);
        }

        public async Task DeleteParameter(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}
