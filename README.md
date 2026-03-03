# 📦 Sistema de Estoque API

Uma API RESTful desenvolvida em **ASP.NET Core** para gerenciamento de produtos e categorias de um sistema de estoque. Este projeto foi construído para aplicar conceitos sólidos de Back-end, modelagem de dados e boas práticas.

## 🚀 Tecnologias Utilizadas

* **C# / .NET** (ASP.NET Core Web API)
* **Entity Framework Core** (ORM)
* **SQL Server** (Banco de Dados Relacional)
* **Swagger** (Documentação e testes da API)

## ✨ Funcionalidades que eu implementei

* **CRUD Completo:** Operações de Criação, Leitura, Atualização e Exclusão de Produtos.
* **Relacionamentos (1:N):** Vínculo entre Tabelas (Um Produto pertence a uma Categoria).
* **Eager Loading:** Consultas otimizadas utilizando o `.Include()`.
* **Tratamento de Ciclo de Objeto:** Uso do `[JsonIgnore]` para evitar loops infinitos nas requisições.
* **Validações de Domínio:** Uso de *Data Annotations* para impedir preços negativos e campos vazios.
* **Migrations:** Gerenciamento do banco de dados versionado via código.