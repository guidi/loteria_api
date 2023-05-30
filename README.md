
# API de resultados das loterias da caixa

[![Build and Publish Docker Image](https://github.com/guidi/loteria_api/actions/workflows/docker-image.yml/badge.svg)](https://github.com/guidi/loteria_api/actions/workflows/docker-image.yml)

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
     guidi/loteria-api:1.6
```     
