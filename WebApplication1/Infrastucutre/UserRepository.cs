﻿
using WebApplication1.Domain.Entities;
    using WebApplication1.Domain.Interfaces;
    using Npgsql;
    using Microsoft.Extensions.Configuration;
    using WebApplication1.Domain.Entities;

    namespace WebApplication1.Infrastucutre
    {
        public class UserRepository : RepositoryBase<User>, IRepository<User>
        {
            public UserRepository(IConfiguration configuration) : base(configuration) { }

            public override async Task DeleteAsync(Guid id) =>
                await this.ExecuteSqlAsync($"DELETE FROM public.users WHERE id='{id}'");

            public override async Task<IEnumerable<User>> GetAllAsync() =>
                await this.ExecuteSqlReaderAsync("SELECT u.id, u.login, u.pass_hash FROM public.users u");

            public override async Task<User> GetByIdAsync(Guid id) =>
                (await this.ExecuteSqlReaderAsync($"SELECT u.id, u.login, u.pass_hash FROM public.users u WHERE u.id='{id}'")).SingleOrDefault();

            public override async Task<Guid> InsertAsync(User entity)
            {
                var newId = Guid.NewGuid();
                await this.ExecuteSqlAsync($"INSERT INTO public.users (id, login, pass_hash) VALUES ('{newId}', '{entity.Login}', '{entity.PassHash}')");
                return newId;
            }

            public override async Task UpdateAsync(Guid id, User entity)
            {
                await this.ExecuteSqlAsync($"UPDATE cr_bd.users SET login='{entity.Login}', pass_hash='{entity.PassHash}' WHERE id='{id}'");
            }

            protected override User GetEntityFromReader(NpgsqlDataReader reader)
            {
                return new User
                {
                    Id = Guid.Parse(reader["id"].ToString()),
                    Login = reader["login"].ToString(),
                    PassHash = reader["pass_hash"].ToString()
                };
            }
        }

    }
