using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using MakeMagic.DTOs;
using MakeMagic.MakeMagicApi;
using MakeMagic.Models;
using MakeMagic.Persistence;
using MakeMagic.Services;
using MakeMagic.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MySqlX.XDevAPI.Common;

namespace MakeMagic.Controllers
{
    [Route("api/characters")]
    [ApiController]
    public class CharactersController : ControllerBase
    {

        private readonly CharacterEditor _characterEditor;
        private readonly CharactersRepository _characterRepository;

        public CharactersController(CharacterEditor characterEditor, CharactersRepository characterRepository)
        {
            _characterEditor = characterEditor;
            _characterRepository = characterRepository;
        }

        /// <summary>
        /// Para uniformizar o retorno de errros para o usuário, sobrescreveremos o comportamento padrão do <see cref="BadRequest(ModelStateDictionary)"/> para retornar uma lista com as
        /// mensagens de erros apenas.
        /// </summary>
        /// <param name="modelState"></param>
        /// <returns></returns>
        public override BadRequestObjectResult BadRequest(ModelStateDictionary modelState) => BadRequest(modelState.SelectMany(ms => ms.Value.Errors.Select(er => er.ErrorMessage)));

        /// <summary>
        /// Ulitário para aplicação de padrão de resposta retornada ao usuário.
        /// - Se <paramref name="result"/> tiver erro e for do tipo <see cref="ErrorLevel.UnrecoverableError"/>, retorna status de erro servidor;
        /// - Se <paramref name="result"/> tiver erro e for do tipo <see cref="ErrorLevel.RecoverableError"/>, retorna status de erro do cliente com uma mensagem
        /// que deve dar uma dica ao usuário de como fazer a requisição ser bem sucedida
        /// - Se  <paramref name="result"/> for bem sucedida, dispara a criação do status de sucesso informado.
        /// </summary>
        /// <returns>resposta que será enviada ao usuário</returns>
        private IActionResult HandleResponseDefault<T>(Result<T> result, Func<IActionResult> onResultSuccessful)
        {
            if (result.Error)
            {
                if (result.ErrorLevel == Utils.ErrorLevel.UnrecoverableError)
                {
                    //Importante logar esses erros
                    return StatusCode(500);
                }
                else
                    return BadRequest(new[] { result.ErrorMessage });

            }
            return onResultSuccessful();
        }

        [HttpPost]
        public async Task<IActionResult> Post(CharacterDTO newCharacter)
        {            
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var characterCreationResult = await _characterEditor.Create(newCharacter);

            return HandleResponseDefault(characterCreationResult,
                () => CreatedAtAction(nameof(Get), new { id = characterCreationResult.Value.Id }, characterCreationResult.Value));
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string house)
        {
            if (string.IsNullOrWhiteSpace(house))
                return Ok(await _characterRepository.Get());
            else
                return Ok(await _characterRepository.Get(house));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {

            var character = await _characterRepository.Get(id);
            if (character is null)
                return NotFound();
            return Ok(character);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, CharacterDTO character)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var characterFromDb = await _characterRepository.Get(id);
            if (characterFromDb is null)
                return NotFound();
            var characterUpdateResult = await _characterEditor.Update(characterFromDb, character);
            return HandleResponseDefault(characterUpdateResult, () => NoContent());
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {

            var characterFromDb = await _characterRepository.Get(id);
            if (characterFromDb is null)
                return NotFound();
            var characterDeletionResult = await _characterEditor.Delete(characterFromDb);

            return HandleResponseDefault(characterDeletionResult, () => NoContent());
        }

    }
}
