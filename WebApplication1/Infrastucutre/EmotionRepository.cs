using WebApplication1.Domain.Entities;
using WebApplication1.Domain.Interfaces;
using Npgsql;
using Microsoft.Extensions.Configuration;

namespace WebApplication1.Infrastucutre
{
    public class EmotionRepository : RepositoryBase<Emotion>, IRepository<Emotion>
    {
        public EmotionRepository(IConfiguration configuration) : base(configuration) { }

        public override async Task DeleteAsync(Guid id) =>
            await this.ExecuteSqlAsync($"DELETE FROM public.emotions WHERE id='{id}'");

        public override async Task<IEnumerable<Emotion>> GetAllAsync() =>
            await this.ExecuteSqlReaderAsync("SELECT e.id, e.state_date, e.emotion_type, e.user_id FROM public.emotions e");

        public override async Task<Emotion> GetByIdAsync(Guid id) =>
            (await this.ExecuteSqlReaderAsync($"SELECT e.id, e.state_date, e.emotion_type, e.user_id FROM public.emotions e WHERE e.id='{id}'")).SingleOrDefault();

        public override async Task<Guid> InsertAsync(Emotion entity)
        {
            var newId = Guid.NewGuid();
            await this.ExecuteSqlAsync($"INSERT INTO public.emotions (id, state_date, emotion_type, user_id) VALUES ('{newId}', '{entity.StateDate:yyyy-MM-dd}', '{entity.EmotionType}', '{entity.UserId}')");
            return newId;
        }

        public override async Task UpdateAsync(Guid id, Emotion entity)
        {
            await this.ExecuteSqlAsync($"UPDATE public.emotions SET state_date='{entity.StateDate:yyyy-MM-dd}', emotion_type='{entity.EmotionType}', user_id='{entity.UserId}' WHERE id='{id}'");
        }

        protected override Emotion GetEntityFromReader(NpgsqlDataReader reader)
        {
            return new Emotion
            {
                Id = Guid.Parse(reader["id"].ToString()),
                StateDate = DateTime.Parse(reader["state_date"].ToString()),
                EmotionType = reader["emotion_type"].ToString(),
                UserId = Guid.Parse(reader["user_id"].ToString())
            };
        }
    }

}
