using Dapper;
using Dapper.Contrib.Extensions;
using MakeMagic.DTOs;
using MakeMagic.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace MakeMagic.Persistence
{
    public class CharactersRepository : IDisposable
    {

        private readonly IDbConnection _dbConnection;
        private bool disposedValue;

        public CharactersRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;            
        }

        //para testes
        protected CharactersRepository()
        {

        }


        public virtual Task<Character> Get(int id)
        {
            return _dbConnection.GetAsync<Character>(id);
        }

        public Task<IEnumerable<Character>> Get()
        {
            return _dbConnection.GetAllAsync<Character>();
        }

        public Task<IEnumerable<Character>> Get(string houseId)
        {
            return _dbConnection.QueryAsync<Character>("SELECT * FROM Characters WHERE house = @HouseId", new { HouseId = houseId });
        }

        public virtual Task<bool> Update(Character character)
        {
            return _dbConnection.UpdateAsync(character);
        }

        public virtual Task<int> Insert(Character character)
        {
            return _dbConnection.InsertAsync(character);
        }

        public virtual Task<bool> Delete(Character character)
        {
            return _dbConnection.DeleteAsync(character);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // Tarefa pendente: descartar o estado gerenciado (objetos gerenciados)
                }

                if (_dbConnection.State == ConnectionState.Open)
                    _dbConnection.Close();
                _dbConnection.Dispose();
                // Tarefa pendente: liberar recursos não gerenciados (objetos não gerenciados) e substituir o finalizador
                // Tarefa pendente: definir campos grandes como nulos
                disposedValue = true;
            }
        }

        // // Tarefa pendente: substituir o finalizador somente se 'Dispose(bool disposing)' tiver o código para liberar recursos não gerenciados
        ~CharactersRepository()
        {
            // Não altere este código. Coloque o código de limpeza no método 'Dispose(bool disposing)'
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // Não altere este código. Coloque o código de limpeza no método 'Dispose(bool disposing)'
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
