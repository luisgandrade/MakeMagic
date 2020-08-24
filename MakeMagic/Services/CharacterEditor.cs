using MakeMagic.DTOs;
using MakeMagic.MakeMagicApi;
using MakeMagic.Models;
using MakeMagic.Persistence;
using MakeMagic.Utils;
using MySqlX.XDevAPI.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MakeMagic.Services
{
    /// <summary>
    /// Serviço para tarefas de edição de um personagem e suas devidas validações.
    /// </summary>
    public class CharacterEditor
    {

        private readonly MakeMagicApiClient _makeMagicApiClient;
        private readonly CharactersRepository _characterRepository;

        public CharacterEditor(MakeMagicApiClient makeMagicApiClient, CharactersRepository characterRepository)
        {
            _makeMagicApiClient = makeMagicApiClient;
            _characterRepository = characterRepository;
        }

        /// <summary>
        /// Verifica se o id da casa informada é válido.
        /// </summary>
        /// <param name="house">id da casa</param>
        /// <returns><c>true</c> se a casa é válida de acordo com a API. <c>false</c>, caso contrário.</returns>
        private async Task<Result<bool>> HouseIsValid(string house)
        {
            var existingHouseResult = await _makeMagicApiClient.GetHouse(house);
            if (existingHouseResult.Error)
                return Result<bool>.Failed(existingHouseResult.ErrorLevel, existingHouseResult.ErrorMessage);
            if (existingHouseResult.Value == null)
                return Result<bool>.Failed(ErrorLevel.RecoverableError, "O id da casa da informado não corresponde a nenhuma casa conhecida.");
            return Result<bool>.Success(existingHouseResult.Value != null);
        }

        /// <summary>
        /// Cria e persiste um novo personagem. O id da casa informado é validado contra os ids de casas na Potter API
        /// </summary>
        /// <param name="characterDTO">info do personagem a ser criado</param>
        /// <returns>personagem criado</returns>
        public async Task<Result<Character>> Create(CharacterDTO characterDTO)
        {
            if (characterDTO is null)
                throw new ArgumentNullException(nameof(characterDTO));

            var houseIsValidResult = await HouseIsValid(characterDTO.House);
            if (houseIsValidResult.Error)
                return Result<Character>.Failed(houseIsValidResult.ErrorLevel, houseIsValidResult.ErrorMessage);

            var character = new Character(characterDTO.Name, characterDTO.Role, characterDTO.School, characterDTO.House, characterDTO.Patronus);
            var id = await _characterRepository.Insert(character);
            var characterFromDb = await _characterRepository.Get(id);
            return Result<Character>.Success(characterFromDb);
        }

        /// <summary>
        /// Atualiza o personagem <paramref name="characterFromDb"/> com os dados contidos em <paramref name="characterDTO"/>. As mesmas validações
        /// que são feitas na criação do personagem são feitas aqui
        /// </summary>
        /// <param name="characterFromDb">entidade do personagem no banco</param>
        /// <param name="characterDTO">nova info do personagem</param>
        /// <returns>entidade do personagem com novos valores</returns>
        public async Task<Result<Character>> Update(Character characterFromDb, CharacterDTO characterDTO)
        {
            if (characterDTO is null)
                throw new ArgumentNullException(nameof(characterDTO));

            var houseIsValidResult = await HouseIsValid(characterDTO.House);
            if (houseIsValidResult.Error)
                return Result<Character>.Failed(houseIsValidResult.ErrorLevel, houseIsValidResult.ErrorMessage);

            
            characterFromDb.Update(characterDTO.Name, characterDTO.Role, characterDTO.School, characterDTO.House, characterDTO.Patronus);
            var updateDone = await _characterRepository.Update(characterFromDb);
            if (!updateDone)
                return Result<Character>.Failed(ErrorLevel.UnrecoverableError, $"Não atualizou o Character de id {characterFromDb.Id}");

            return Result<Character>.Success(characterFromDb);
        }

        /// <summary>
        /// Deleta o personagem <paramref name="characterFromDb"/> do banco.
        /// </summary>
        /// <param name="characterFromDb"></param>
        /// <returns></returns>
        public async Task<Result<bool>> Delete(Character characterFromDb)
        {   
            if (characterFromDb != null)
            {
                var deleteDone = await _characterRepository.Delete(characterFromDb);
                if(!deleteDone)
                    return Result<bool>.Failed(ErrorLevel.UnrecoverableError, $"Não deletou o Character de id {characterFromDb.Id}");
            }
            return Result<bool>.Success(true);
        }

    }
}
