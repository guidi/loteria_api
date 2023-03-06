
# API da loteria da caixa

API simples que obtém os resultados das loterias da caixa.

Compatível com as seguintes loterias: "megasena", "quina", "lotofacil", "lotomania", "duplasena", "timemania", "diadesorte", "federal", "loteca", "supersete", "maismilionaria"

Caso queira apenas utilizar a API ela está disponível no endereço: https://api.guidi.dev.br/loteria, para utilizar basta fazer um GET na url passando o código da loteria e o concurso.

Exemplo para obter o último concurso da megasena: https://api.guidi.dev.br/loteria/megasena/ultimo

Exemplo para obter o concurso 2000 da megasena: https://api.guidi.dev.br/loteria/megasena/2000


FAQ:

Quando é feita a carga de dados?
- Não tem carga de dados, os dados são obtidos online, exceto se já existirem no banco de dados da API.

Como faço para rodar o código na minha máquina?
- Faça o git clone, abra a solution no visual studio (tem que ter o .NET 7), altere a configuração do banco de dados no appsettings.json e execute o projeto.

Você aceita pull request?
- Sim, fique a vontade para enviar.

Onde está hospedada a API?
- Digital Ocean

Tem algum limite de requests nessa API?
- Por enquanto não, somente se eu ver que estão abusando, aí eu limito no api gateway.


