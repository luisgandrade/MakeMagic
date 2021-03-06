# MakeMagic

#### Candidato: Luís Gabriel de Andrade

## Docker:
- Em `docker-compose.yml`, substituir a variável de argumento `<<make_magic_api_key>>` com a chave da API;
- Buildar com `docker-compose build`;
- Rodar com `docker-compose up -d`;
- Aplicação estará ouvindo na porta 8080 do host.


**Observação: como há um tempo entre a inicialização do container e a inicialização do MySQL é possível que a verificação inicial do banco pela aplicação falhe. Foi implementado um workaround que faz dez tentativas de conectar ao banco num intervalo de dez segundos. Se ainda assim não funcionar, será necessário reiniciar o container da aplicação com `docker container start app`.**


## Endpoints

#### Criar aluno
```
POST /api/characters
```
**Descrição:** Cria um aluno com as informações passadas no corpo da requisição. Todos os parâmetros devem ser não nulos e ```house``` deve ser o identificador único da casa na Potter API.

**Corpo do objeto esperado**
```
{
    name: string,
    role: string,
    school: string,
    house: string,
    patronus: string
}
```

### Editar aluno

```
PUT /api/characters/{id}
```

**Descrição:** Altera os dados do aluno identificado pelo id informado com as informações passadas no corpo da requisição. Todos os parâmetros devem ser não nulos e ```house``` deve ser o identificador único da casa na Potter API.

**Corpo do objeto esperado**
```
{
    name: string,
    role: string,
    school: string,
    house: string,
    patronus: string
}
```

### Deletar aluno

```
DELETE /api/characters/{id}
```

**Descrição:** Delete o aluno identificado do banco de dados.

### Buscar aluno

```
GET /api/characters/{id}
```

**Descrição:** Busca o aluno identificado pelo id informado.

**Objeto retornado**
```
{
    id: int,
    name: string,
    role: string,
    school: string,
    house: string,
    patronus: string
}
```


```
GET /api/characters?{house?}
```

**Descrição:** Retorna todos os alunos cadastrados e os filtra opcionalmente pela casa identificada pelo parâmetro ```house``` se esta for informada. 

**Objeto retornado**
```
[{
    id: int,
    name: string,
    role: string,
    school: string,
    house: string,
    patronus: string
}]
```