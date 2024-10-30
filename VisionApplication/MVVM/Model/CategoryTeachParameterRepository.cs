using Microsoft.EntityFrameworkCore;
using VisionApplication.Model;

namespace VisionApplication.Model
{
    public class CategoryTeachParameterRepository
    {
        private readonly DatabaseContext _context;

        public CategoryTeachParameterRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<CategoryTeachParameter> AddAsync(CategoryTeachParameter entity)
        {
            _context.categoryTeachParametersModel.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<CategoryTeachParameter> GetByIdAsync(int cameraId)
        {
            return await _context.categoryTeachParametersModel
                //.Include(c => c.categoryVisionParameter) // Include related RectanglesModel if you have a navigation property
                .FirstOrDefaultAsync(c => c.cameraID == cameraId);
        }

        // Read (Get All)
        public async Task<IEnumerable<CategoryTeachParameter>> GetAllAsync()
        {
            return await _context.categoryTeachParametersModel
                //.Include(c => c.Rectangles) // Include related RectanglesModel if you have a navigation property
                .ToListAsync();
        }

        // Update
        public async Task UpdateAsync(CategoryTeachParameter entity)
        {
            _context.categoryTeachParametersModel.Update(entity);
            await _context.SaveChangesAsync();
        }

        // Delete
        public async Task DeleteAsync(int id)
        {
            var entity = await _context.categoryTeachParametersModel.FindAsync(id);
            if (entity != null)
            {
                _context.categoryTeachParametersModel.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

    }
}
