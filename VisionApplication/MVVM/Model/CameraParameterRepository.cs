
using VisionApplication.Model;
using Microsoft.EntityFrameworkCore;

namespace VisionApplication.MVVM.Model
{
    public class CameraParameterRepository
    {
        private readonly DatabaseContext _context;

        public CameraParameterRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<CameraParameter> AddAsync(CameraParameter entity)
        {
            _context.cameraParameterModel.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<CameraParameter> GetByIdAsync(int cameraId)
        {
            return await _context.cameraParameterModel
                //.Include(c => c.categoryVisionParameter) // Include related RectanglesModel if you have a navigation property
                .FirstOrDefaultAsync(c => c.cameraID == cameraId);
        }

        // Read (Get All)
        public async Task<IEnumerable<CameraParameter>> GetAllAsync()
        {
            return await _context.cameraParameterModel
                .ToListAsync();
        }

        // Update
        public async Task UpdateAsync(CameraParameter entity)
        {
            _context.cameraParameterModel.Update(entity);
            await _context.SaveChangesAsync();
        }

        // Delete
        public async Task DeleteAsync(int id)
        {
            var entity = await _context.cameraParameterModel.FindAsync(id);
            if (entity != null)
            {
                _context.cameraParameterModel.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
