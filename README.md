
# API de resultados das loterias da caixa

[![Build and Publish Docker Image](https://github.com/guidi/loteria_api/actions/workflows/docker-image.yml/badge.svg)](https://github.com/guidi/loteria_api/actions/workflows/docker-image.yml)


API simples e GRATUITA que obtém os resultados das loterias da caixa.

Compatível com as seguintes loterias: "megasena", "quina", "lotofacil", "lotomania", "duplasena", "timemania", "diadesorte", "federal", "loteca", "supersete", "maismilionaria"

Caso queira apenas utilizar a API, ela está disponível no endereço: https://api.guidi.dev.br/loteria, para utilizar basta fazer um GET na url, passando o código da loteria e o concurso.

Exemplo para obter o último concurso da megasena: https://api.guidi.dev.br/loteria/megasena/ultimo

Exemplo para obter o concurso 2000 da megasena: https://api.guidi.dev.br/loteria/megasena/2000


FAQ:

Quando é feita a carga de dados?
- Não tem carga de dados, os dados são obtidos online, exceto se já existirem no banco de dados da API, ou seja, no momento em que o concurso for publicado no site da caixa, ele também vai estar disponível na API.

Como faço para rodar o código na minha máquina?
- Faça o git clone, abra a solution no visual studio (tem que ter o .NET 7), altere a configuração do banco de dados no appsettings.json e execute o projeto.

Você aceita pull request?
- Sim, fique a vontade para enviar.

Onde está hospedada a API?
- Digital Ocean

Tem algum limite de requests nessa API?
- Por enquanto não, somente se eu ver que estão abusando, aí eu limito no api gateway.

Qual a stack do projeto?
- Net Core 7 + Serilog + Polly + Entity Framework Core c/ Code First + MYSQL
- Docker
- Kong como API gateway
- UptimeRobot para Health Check
- Github Actions + WatchTower para CI/CD

Por que não escreveu testes para este projeto?
- Não vi necessidade pra esse caso.

Onde fica a imagem docker deste projeto?
- https://hub.docker.com/r/guidi/loteria-api, use sempre a versão mais recente.

Como faço para executar essa imagem?
- Execute o comando abaixo, lembre de definir na variável de ambiente a connection string do seu banco de dados e de usar a tag mais recente do container.

```
sudo docker run -d \
	 -p 7047:7047 \
	 -p 5190:5190 \
	 -e "ConnectionStrings:DefaultConnection"="Server=loteria_db-1;Port=3306;Database=loteria;Uid=loteria;Pwd=loteria;SslMode=Required" \
     --name loteria-api \
     guidi/loteria-api:1.5
```     
