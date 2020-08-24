using MakeMagic.DTOs;
using MakeMagic.MakeMagicApi;
using MakeMagic.MakeMagicApi.Models;
using MakeMagic.Models;
using MakeMagic.Persistence;
using MakeMagic.Services;
using MakeMagic.Utils;
using Moq;
using MySqlX.XDevAPI.Common;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MakeMagic.Tests.Unit
{
    public class CharacterEditorTests
    {


        [Fact]
        public async Task CreationShouldPropagateApiCallErrorIfThereWasAny()
        {
            var houseId = "hehehe";
            var characterDTO = Mock.Of<CharacterDTO>(c => c.House == houseId);

            var exceptedApiResult = Result<HouseModel>.Failed(ErrorLevel.UnrecoverableError, "Erro Api");

            var makeMagicApiClientMock = Mock.Of<MakeMagicApiClient>(mmac => mmac.GetHouse(houseId) == Task.FromResult(exceptedApiResult));

            var characterEditor = new CharacterEditor(makeMagicApiClientMock, Mock.Of<CharactersRepository>());
            var result = await characterEditor.Create(characterDTO);

            Assert.True(result.Error);
            Assert.Equal(exceptedApiResult.ErrorLevel, result.ErrorLevel);
            Assert.Contains(exceptedApiResult.ErrorMessage, result.ErrorMessage);
        }

        [Fact]
        public async Task CreationShouldFailWithRecoverableErrorIfHouseIsInvalid()
        {
            var houseId = "hehehe";
            var characterDTO = Mock.Of<CharacterDTO>(c => c.House == houseId);

            var apiResult = Result<HouseModel>.Success(null);

            var makeMagicApiClientMock = Mock.Of<MakeMagicApiClient>(mmac => mmac.GetHouse(houseId) == Task.FromResult(apiResult));
            

            var characterEditor = new CharacterEditor(makeMagicApiClientMock, Mock.Of<CharactersRepository>());
            var result = await characterEditor.Create(characterDTO);

            Assert.True(result.Error);
            Assert.Equal(ErrorLevel.RecoverableError, result.ErrorLevel);
            Assert.Contains("O id da casa da informado não corresponde a nenhuma casa conhecida.", result.ErrorMessage);
        }

        [Fact]
        public async Task CreationShouldReturnEntityInsertedWithId()
        {
            var idAssigned = 1;
            var name = "hahaha";
            var houseId = "hehehe";
            var school = "hihihi";
            var role = "hohoho";
            var patronus = "huhuhu";
            var characterDTO = new CharacterDTO
            {
                Name = name,
                House = houseId,
                School = school,
                Role = role,
                Patronus = patronus
            };
            var houseModel = new HouseModel
            {
                Id = houseId
            };
            var characterEntityMock = Mock.Of<Character>(c => c.Id == idAssigned && c.Name == name && c.House == houseId && c.School == school && c.Role == role && c.Patronus == patronus);

            var apiResult = Result<HouseModel>.Success(houseModel);

            var makeMagicApiClientMock = Mock.Of<MakeMagicApiClient>(mmac => mmac.GetHouse(houseId) == Task.FromResult(apiResult));
            var characterRepository = Mock.Of<CharactersRepository>(cr =>
                cr.Insert(It.Is<Character>(c => c.Name == name && c.House == houseId && c.School == school && c.Role == role && c.Patronus == patronus)) == Task.FromResult(idAssigned) &&
                cr.Get(idAssigned) == Task.FromResult(characterEntityMock));

            var characterEditor = new CharacterEditor(makeMagicApiClientMock, characterRepository);
            var result = await characterEditor.Create(characterDTO);

            Assert.False(result.Error);
            Assert.Equal(characterEntityMock, result.Value);
        }

        [Fact]
        public async Task UpdateShouldPropagateApiCallErrorIfThereWasAny()
        {
            var houseId = "hehehe";
            var characterDTO = Mock.Of<CharacterDTO>(c => c.House == houseId);
            var characterFromDb = Mock.Of<Character>(c => c.House == houseId);

            var exceptedApiResult = Result<HouseModel>.Failed(ErrorLevel.UnrecoverableError, "Erro Api");

            var makeMagicApiClientMock = Mock.Of<MakeMagicApiClient>(mmac => mmac.GetHouse(houseId) == Task.FromResult(exceptedApiResult));

            var characterEditor = new CharacterEditor(makeMagicApiClientMock, Mock.Of<CharactersRepository>());
            var result = await characterEditor.Update(characterFromDb, characterDTO);

            Assert.True(result.Error);
            Assert.Equal(exceptedApiResult.ErrorLevel, result.ErrorLevel);
            Assert.Contains(exceptedApiResult.ErrorMessage, result.ErrorMessage);
        }

        [Fact]
        public async Task UpdateShouldFailWithRecoverableErrorIfHouseIsInvalid()
        {
            var houseId = "hehehe";
            var characterDTO = Mock.Of<CharacterDTO>(c => c.House == houseId);

            var characterFromDb = Mock.Of<Character>(c => c.House == houseId);

            var apiResult = Result<HouseModel>.Success(null);

            var makeMagicApiClientMock = Mock.Of<MakeMagicApiClient>(mmac => mmac.GetHouse(houseId) == Task.FromResult(apiResult));


            var characterEditor = new CharacterEditor(makeMagicApiClientMock, Mock.Of<CharactersRepository>());
            var result = await characterEditor.Update(characterFromDb, characterDTO);

            Assert.True(result.Error);
            Assert.Equal(ErrorLevel.RecoverableError, result.ErrorLevel);
            Assert.Contains("O id da casa da informado não corresponde a nenhuma casa conhecida.", result.ErrorMessage);
        }

        [Fact]
        public async Task UpdateShouldAlterPropertiesOfEntity()
        {            
            var name = "hahaha";
            var houseId = "hehehe";
            var school = "hihihi";
            var role = "hohoho";
            var patronus = "huhuhu";
            var newCharacterInfoDTO = new CharacterDTO
            {
                Name = name,
                House = houseId,
                School = school,
                Role = role,
                Patronus = patronus
            };
            var houseModel = new HouseModel
            {
                Id = houseId
            };
            var characterEntityMock = new Mock<Character>();            

            var apiResult = Result<HouseModel>.Success(houseModel);

            var makeMagicApiClientMock = Mock.Of<MakeMagicApiClient>(mmac => mmac.GetHouse(houseId) == Task.FromResult(apiResult));
            var characterRepository = Mock.Of<CharactersRepository>(cr => cr.Update(characterEntityMock.Object) == Task.FromResult(true));

            var characterEditor = new CharacterEditor(makeMagicApiClientMock, characterRepository);
            var result = await characterEditor.Update(characterEntityMock.Object, newCharacterInfoDTO);

            Assert.False(result.Error);
            Assert.Equal(characterEntityMock.Object, result.Value);
            characterEntityMock.Verify(c => c.Update(newCharacterInfoDTO.Name, newCharacterInfoDTO.Role, newCharacterInfoDTO.School, newCharacterInfoDTO.House, newCharacterInfoDTO.Patronus),
                Times.Once());
        }


        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task DeletionShouldPropagateOperationStatusFromDeletionInDB(bool successful)
        {
            var character = Mock.Of<Character>(c => c.Id == 1);

            var characterRepository = Mock.Of<CharactersRepository>(cr => cr.Delete(character) == Task.FromResult(successful));

            var characterEditor = new CharacterEditor(Mock.Of<MakeMagicApiClient>(), characterRepository);
            var result = await characterEditor.Delete(character);

            Assert.NotEqual(successful, result.Error);
        }
        
    }
}
