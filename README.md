
# API de resultados das loterias da caixa

[![Build and Publish Docker Image](https://github.com/guidi/loteria_api/actions/workflows/docker-image.yml/badge.svg)](https://github.com/guidi/loteria_api/actions/workflows/docker-image.yml)


API simples e GRATUITA que obtém os resultados das loterias da caixa.

Compatível com as seguintes loterias: "megasena", "quina", "lotofacil", "lotomania", "duplasena", "timemania", "diadesorte", "federal", "loteca", "supersete", "maismilionaria"

Caso queira apenas utilizar a API, ela está disponível no endereço: https://api.guidi.dev.br/loteria, para utilizar basta fazer um GET na url, passando o código da loteria e o concurso.

Exemplo para obter o último concurso da megasena: https://api.guidi.dev.br/loteria/megasena/ultimo

Exemplo para obter o concurso 2000 da megasena: https://api.guidi.dev.br/loteria/megasena/2000

## Atualização - 12/07/2026

Devido ao aumento no volume de scrapers realizando consultas automatizadas, foram aplicadas as seguintes medidas de proteção:

- Bloqueio de acessos originados de endereços IP fora do Brasil.
- Aplicação de rate limit nas consultas aos resultados das loterias.

Essas medidas visam preservar a disponibilidade da API para os usuários legítimos e garantir a estabilidade do serviço.

## Snapshot da base

Medição realizada em `2026-06-17` no banco de dados em produção.

Quantidade de resultados por loteria:

- `megasena`: `2993`
- `quina`: `6996`
- `federal`: `6037`
- `lotofacil`: `3655`
- `duplasena`: `2941`
- `lotomania`: `2908`
- `timemania`: `2364`
- `loteca`: `1240`
- `diadesorte`: `1198`
- `supersete`: `832`
- `maismilionaria`: `343`

Dezenas que mais saíram na Mega-Sena nessa medição:

- `10`: `352` vezes
- `53`: `342`
- `37`: `325`
- `05`: `324`
- `34`: `321`
- `33`: `320`
- `32`: `319`
- `38`: `319`
- `27`: `318`
- `46`: `317`
- `04`: `316`
- `17`: `316`
- `42`: `316`
- `30`: `315`
- `35`: `315`

Dezenas que mais saíram na Quina nessa medição:

- `04`: `502` vezes
- `26`: `487`
- `52`: `485`
- `44`: `477`
- `49`: `476`
- `31`: `470`
- `16`: `467`
- `29`: `467`
- `39`: `466`
- `05`: `465`
- `56`: `465`
- `42`: `463`
- `53`: `463`
- `15`: `461`
- `09`: `460`

Métricas da Federal nessa medição:

- `6037` resultados cadastrados
- `30185` bilhetes representados na base
- cada concurso da `federal` armazena `5` bilhetes de `6` dígitos

FAQ:

Quando é feita a carga de dados?
- Não tem carga de dados, os dados são obtidos online, exceto se já existirem no banco de dados da API, ou seja, no momento em que o concurso for publicado no site da caixa, ele também vai estar disponível na API.

Como faço para rodar o código na minha máquina?
- Faça o git clone, abra a solution no visual studio (tem que ter o .NET 10 SDK), altere a configuração do banco de dados no appsettings.json e execute o projeto.

Você aceita pull request?
- Sim, fique a vontade para enviar.

Onde está hospedada a API?
- Digital Ocean

Tem algum limite de requests nessa API?
- Sim. Atualmente a API aplica rate limit nas consultas aos resultados das loterias e aceita apenas requisições originadas de endereços IP do Brasil.

Qual a stack do projeto?
- Net 10 + Serilog + Seq + Polly + Entity Framework Core c/ Code First + MYSQL
- Docker
- Kong como API gateway
- UptimeRobot para Health Check
- Github Actions + WatchTower para CI/CD

Por que não escreveu testes para este projeto?
- Não vi necessidade pra esse caso.

Onde fica a imagem docker deste projeto?
- https://hub.docker.com/r/guidi/loteria-api, use sempre a versão mais recente.

Como faço para executar essa imagem?
- Execute o comando abaixo, lembre de definir na variável de ambiente a connection string do seu banco de dados, `SEQ_URL` caso queira enviar logs para o Seq, e de usar a tag mais recente do container.

```
sudo docker run -d \
	 -p 7047:7047 \
	 -p 5190:5190 \
     -e "ConnectionStrings:DefaultConnection"="Server=loteria_db-1;Port=3306;Database=loteria;Uid=loteria;Pwd=loteria;SslMode=Required" \
     -e "SEQ_URL"="http://seq:5341" \
     -e "SEQ_API_KEY"="" \
     -e "FORWARDED_NETWORKS"="172.18.0.0/16,172.19.0.0/16" \
     --name loteria-api \
     guidi/loteria-api:1.5
```

Se `SEQ_URL` nao estiver configurada, a aplicacao continua logando apenas no console.
Atrás de Cloudflare ou proxy reverso, `FORWARDED_NETWORKS` deve conter as redes confiáveis dos proxies para que o IP real do cliente seja resolvido corretamente.
